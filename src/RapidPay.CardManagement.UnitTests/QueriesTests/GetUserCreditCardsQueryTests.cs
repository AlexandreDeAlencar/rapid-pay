using System.Collections;
using Bogus;
using RapidPay.CardManagement.App.Cards.Queries;
using RapidPay.CardManagement.Domain.Cards.Models;
using RapidPay.CardManagement.UnitTests.MockedServices;

public class GetUserCreditCardsQueryHandlerTestData : IEnumerable<object[]>
{
    private readonly Faker _faker = new Faker();

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _faker.Random.Guid(), true };
        yield return new object[] { Guid.Empty, false };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class GetUserCreditCardsQueryHandlerTests
{
    private readonly FakeCardRepository _fakeRepository;
    private readonly FakeLogger<GetUserCreditCardsQueryHandler> _fakeLogger;
    private readonly Faker _faker;

    public GetUserCreditCardsQueryHandlerTests()
    {
        _fakeRepository = new FakeCardRepository();
        _fakeLogger = new FakeLogger<GetUserCreditCardsQueryHandler>();
        _faker = new Faker();
    }

    [Theory]
    [ClassData(typeof(GetUserCreditCardsQueryHandlerTestData))]
    public async Task Handle_ShouldReturnCorrectResult_BasedOnUserCardExistence(Guid userId, bool cardsExist)
    {
        // Arrange
        if (cardsExist)
        {
            var card = Card.Create(
                _faker.Random.Guid(),
                _faker.Finance.CreditCardNumber(),
                _faker.Finance.Amount(),
                DateTime.Now,
                DateTime.Now,
                _faker.Person.FullName,
                userId.ToString(),
                DateTime.Now.AddYears(3)
            ).Value;

            await _fakeRepository.AddAsync(card);
        }

        var handler = new GetUserCreditCardsQueryHandler(_fakeRepository, _fakeLogger);
        var query = new GetUserCreditCardsQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        if (cardsExist)
        {
            Assert.False(result.IsError);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value.CreditCards);
        }
        else
        {
            Assert.True(result.IsError);
        }
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", "No credit cards found for the specified user.")]
    public async Task Handle_ShouldReturnNotFoundError_WhenUserIdIsInvalid(string userIdStr, string expectedErrorMessage)
    {
        // Arrange
        var userId = Guid.Parse(userIdStr);
        var handler = new GetUserCreditCardsQueryHandler(_fakeRepository, _fakeLogger);
        var query = new GetUserCreditCardsQuery(userId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
        Assert.Equal(expectedErrorMessage, error.Description);
    }
}
