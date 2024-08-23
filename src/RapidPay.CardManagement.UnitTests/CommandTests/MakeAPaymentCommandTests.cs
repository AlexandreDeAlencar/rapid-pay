using ErrorOr;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Fees.Services;
using static FakeCardRepository;

public class PayWithCreditCardCommandHandlerTests
{
    private readonly FakeCardRepository _fakeRepository;
    private readonly FakeCardRepositoryWithError _fakeRepositoryWithError;
    private readonly FeesExchangeService _feeService;

    public PayWithCreditCardCommandHandlerTests()
    {
        _fakeRepository = new FakeCardRepository();
        _fakeRepositoryWithError = new FakeCardRepositoryWithError();
        _feeService = new FeesExchangeService();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCardIdIsEmpty()
    {
        // Arrange
        var handler = new PayWithCreditCardCommandHandler(_fakeRepository, _feeService);
        var command = new PayWithCreditCardCommand(Guid.Empty, 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("invalid card id", result.FirstError.Description);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCardIsFound()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var card = Card.Create(
            cardId,
            "123456789012345",
            100,
            DateTime.Now,
            DateTime.Now,
            "JohnDoe",
            "User123",
            DateTime.Now.AddYears(3)
        ).Value;

        await _fakeRepository.AddAsync(card);

        var handler = new PayWithCreditCardCommandHandler(_fakeRepository, _feeService);
        var command = new PayWithCreditCardCommand(cardId, 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(result.Value, new Success());
    }
}
