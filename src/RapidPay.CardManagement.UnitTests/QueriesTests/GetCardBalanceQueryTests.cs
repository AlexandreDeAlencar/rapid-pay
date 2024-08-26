using System.Collections;
using Bogus;
using RapidPay.CardManagement.App.Cards.Queries;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.UnitTests.MockedServices;

public class GetCardBalanceQueryHandlerTestData : IEnumerable<object[]>
{
    private readonly Faker _faker = new Faker();

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _faker.Random.Guid(), 0m, true };  // Card found
        yield return new object[] { Guid.Empty, 0m, false };           // Card not found
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class GetCardBalanceQueryHandlerTests
{
    private readonly FakeCardRepository _fakeRepository;
    private readonly FakeLogger<GetCardBalanceQueryHandler> _fakeLogger;

    public GetCardBalanceQueryHandlerTests()
    {
        _fakeRepository = new FakeCardRepository();
        _fakeLogger = new FakeLogger<GetCardBalanceQueryHandler>();
    }

    [Theory]
    [ClassData(typeof(GetCardBalanceQueryHandlerTestData))]
    public async Task Handle_ShouldReturnCorrectResult_BasedOnCardExistence(Guid cardId, decimal expectedBalance, bool cardExists)
    {
        // Arrange
        if (cardExists)
        {
            var card = Card.Create(
                cardId,
                new Faker().Finance.CreditCardNumber(),
                expectedBalance,
                DateTime.Now,
                DateTime.Now,
                new Faker().Person.FullName,
                new Faker().Random.Guid().ToString(),
                DateTime.Now.AddYears(3)
            ).Value;

            await _fakeRepository.AddAsync(card);
        }

        var handler = new GetCardBalanceQueryHandler(_fakeRepository, _fakeLogger);
        var query = new GetCardBalanceQuery(cardId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        if (cardExists)
        {
            Assert.False(result.IsError);
            Assert.Equal(expectedBalance, result.Value.Balance);
        }
        else
        {
            Assert.True(result.IsError);
        }
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", "Invalid card ID")]
    public async Task Handle_ShouldReturnValidationError_WhenCardIdIsInvalid(string cardIdStr, string expectedErrorMessage)
    {
        // Arrange
        var cardId = Guid.Parse(cardIdStr);
        var handler = new GetCardBalanceQueryHandler(_fakeRepository, _fakeLogger);
        var query = new GetCardBalanceQuery(cardId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
        Assert.Equal(expectedErrorMessage, error.Description);
    }
}
