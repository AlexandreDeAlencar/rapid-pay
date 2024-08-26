using System.Collections;
using ErrorOr;
using Bogus;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;
using RapidPay.CardManagement.UnitTests.MockedServices;
using static FakeCardRepository;

public class PayWithCreditCardCommandHandlerTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        var faker = new Faker();
        yield return new object[]
        {
            Guid.NewGuid(), 50, new FakeCardRepositoryWithError(), new FakeFeeRepositoryWithError(), "Unable to retrieve fee"
        };
        yield return new object[]
        {
            Guid.NewGuid(), 50, new FakeCardRepositoryWithError(), new FakeFeeRepository(), "Card not found"
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class PayWithCreditCardCommandHandlerTests
{
    private readonly FakeCardRepository _fakeRepository;
    private readonly FakeFeeRepository _fakeFeeRepository;
    private readonly FakeCardRepositoryWithError _fakeRepositoryWithError;
    private readonly FakeFeeRepositoryWithError _fakeFeeRepositoryWithError;
    private readonly FakeLogger<PayWithCreditCardCommandHandler> _fakeLogger;
    private readonly Faker _faker;

    public PayWithCreditCardCommandHandlerTests()
    {
        _fakeRepository = new FakeCardRepository();
        _fakeFeeRepository = new FakeFeeRepository();
        _fakeRepositoryWithError = new FakeCardRepositoryWithError();
        _fakeFeeRepositoryWithError = new FakeFeeRepositoryWithError();
        _fakeLogger = new FakeLogger<PayWithCreditCardCommandHandler>();
        _faker = new Faker();
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
            _faker.Finance.CreditCardNumber(),
            100,
            DateTime.Now,
            DateTime.Now,
            _faker.Person.FullName,
            _faker.Random.Guid().ToString(),
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

    [Theory]
    [ClassData(typeof(PayWithCreditCardCommandHandlerTestData))]
    public async Task Handle_ShouldReturnError_WhenRepositoriesFail(Guid cardId, decimal value, ICardRepository cardRepository, IFeeRepository feeRepository, string expectedErrorMessage)
    {
        // Arrange
        var handler = new PayWithCreditCardCommandHandler(cardRepository, feeRepository, _fakeLogger);
        var command = new PayWithCreditCardCommand(cardId, value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(expectedErrorMessage, result.FirstError.Description);
    }
}
