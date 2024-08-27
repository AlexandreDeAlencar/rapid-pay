using System.Collections;
using Bogus;
using ErrorOr;
using RapidPay.CardManagement.App.Cards.Queries;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.UnitTests.MockedServices;

public class GetCardBalanceQueryHandlerTestData : IEnumerable<object[]>
{
    private readonly Faker _faker = new Faker();

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _faker.Random.Guid(), 0m, true };
        yield return new object[] { Guid.Empty, 0m, false };
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
        var query = new GetCreditCardBalanceQuery(cardId);

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
    [InlineData("00000000-0000-0000-0000-000000000000", "Card ID is required.")]
    public async Task Handle_ShouldReturnValidationError_WhenCardIdIsInvalid(string cardIdStr, string expectedErrorMessage)
    {
        // Arrange
        var cardId = Guid.Parse(cardIdStr);
        var validator = new GetCardBalanceQueryValidator();
        var query = new GetCreditCardBalanceQuery(cardId);

        // Act
        var validationResult = await validator.ValidateAsync(query);

        // Assert
        if (!validationResult.IsValid)
        {
            var errorMessage = validationResult.Errors.First().ErrorMessage;
            Assert.Equal(expectedErrorMessage, errorMessage);
        }
        else
        {
            var handler = new GetCardBalanceQueryHandler(_fakeRepository, _fakeLogger);
            var result = await handler.Handle(query, CancellationToken.None);
            Assert.False(result.IsError, "Expected validation to fail, but it passed.");
        }
    }
}
