using ErrorOr;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.Domain.Cards.Models;
using static FakeCardRepository;

public class PayWithCreditCardCommandHandlerTests
{
    private readonly FakeCardRepository _fakeRepository;
    private readonly FakeFeeRepository _fakeFeeRepository;
    private readonly FakeCardRepositoryWithError _fakeRepositoryWithError;
    private readonly FakeFeeRepositoryWithError _fakeFeeRepositoryWithError;
    private readonly FakeLogger<PayWithCreditCardCommandHandler> _fakeLogger;

    public PayWithCreditCardCommandHandlerTests()
    {
        _fakeRepository = new FakeCardRepository();
        _fakeFeeRepository = new FakeFeeRepository();
        _fakeRepositoryWithError = new FakeCardRepositoryWithError();
        _fakeFeeRepositoryWithError = new FakeFeeRepositoryWithError();
        _fakeLogger = new FakeLogger<PayWithCreditCardCommandHandler>();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCardIdIsEmpty()
    {
        // Arrange
        var handler = new PayWithCreditCardCommandHandler(_fakeRepository, _fakeFeeRepository, _fakeLogger);
        var command = new PayWithCreditCardCommand(Guid.Empty, 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Invalid card ID", result.FirstError.Description);
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

        var handler = new PayWithCreditCardCommandHandler(_fakeRepository, _fakeFeeRepository, _fakeLogger);
        var command = new PayWithCreditCardCommand(cardId, 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(new Success(), result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenFeeRepositoryFails()
    {
        // Arrange
        var handler = new PayWithCreditCardCommandHandler(_fakeRepository, _fakeFeeRepositoryWithError, _fakeLogger);
        var command = new PayWithCreditCardCommand(Guid.NewGuid(), 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal("Unable to retrieve fee", result.FirstError.Description);
    }
}
