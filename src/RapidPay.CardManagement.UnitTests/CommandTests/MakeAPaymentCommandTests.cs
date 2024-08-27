using System.Collections;
using ErrorOr;
using Bogus;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.Domain.Ports;
using RapidPay.CardManagement.UnitTests.MockedServices;

public class PayWithCreditCardCommandHandlerTests
{
    private readonly FakeLogger<PayWithCreditCardCommandHandler> _fakeLogger;
    private readonly Faker _faker;

    public PayWithCreditCardCommandHandlerTests()
    {
        _fakeLogger = new FakeLogger<PayWithCreditCardCommandHandler>();
        _faker = new Faker();
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCardIdIsEmpty()
    {
        // Arrange
        var handler = new PayWithCreditCardCommandHandler(new FakeCardRepository(), new FakeFeeRepository(), _fakeLogger);
        var command = new PayWithCreditCardCommand(Guid.Empty, 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCardIsFound()
    {
        // Arrange
        var cardId = _faker.Random.Guid();
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

        var fakeCardRepository = new FakeCardRepository();
        await fakeCardRepository.AddAsync(card);

        var handler = new PayWithCreditCardCommandHandler(fakeCardRepository, new FakeFeeRepository(), _fakeLogger);
        var command = new PayWithCreditCardCommand(cardId, 50);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(new Success(), result.Value);
    }
}
