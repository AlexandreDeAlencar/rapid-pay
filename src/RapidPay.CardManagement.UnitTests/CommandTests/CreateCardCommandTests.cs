using System.Collections;
using Bogus;
using RapidPay.CardManagement.App.Cards.Commands;
using RapidPay.CardManagement.UnitTests.MockedServices;
using static FakeCardRepository;

public class CreateCardCommandHandlerTestData : IEnumerable<object[]>
{
    private readonly Faker _faker = new Faker();

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { _faker.Person.FullName, _faker.Random.Guid().ToString(), true, typeof(Guid) }; // Success case
        yield return new object[] { _faker.Person.FullName, _faker.Random.Guid().ToString(), false, typeof(InvalidOperationException) }; // Error case
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class CreateCardCommandHandlerTests
{
    private readonly FakeCardRepository _fakeCardRepository;
    private readonly Faker _faker;
    private CreateCardCommandHandler _handler;

    public CreateCardCommandHandlerTests()
    {
        _fakeCardRepository = new FakeCardRepository();
        _faker = new Faker();
        _handler = new CreateCardCommandHandler(_fakeCardRepository, new FakeLogger<CreateCardCommandHandler>());
    }

    [Theory]
    [ClassData(typeof(CreateCardCommandHandlerTestData))]
    public async Task Handle_ShouldReturnCorrectResult_BasedOnRepositoryBehavior(string userName, string userId, bool simulateSuccess, Type expectedType)
    {
        // Arrange
        if (!simulateSuccess)
        {
            _handler = new CreateCardCommandHandler(new FakeCardRepositoryWithError(), new FakeLogger<CreateCardCommandHandler>());
        }
        var command = new CreateCard(userName, userId);

        // Act & Assert
        if (expectedType == typeof(Guid))
        {
            var result = await _handler.Handle(command, CancellationToken.None);
            Assert.IsType<Guid>(result.Value);
            if (simulateSuccess)
            {
                Assert.Single(_fakeCardRepository.AddedCards);
            }
        }
        else if (expectedType == typeof(InvalidOperationException))
        {
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });
        }
    }

    [Theory]
    [InlineData("", "User123", "Unable to create a new card")]
    [InlineData("JohnDoe", "", "Invalid userId")]
    public async Task Handle_ShouldReturnValidationError_WhenInputIsInvalid(string userName, string userId, string expectedErrorMessage)
    {
        // Arrange
        var command = new CreateCard(userName, userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
        Assert.Equal(expectedErrorMessage, error.Description);
    }
}
