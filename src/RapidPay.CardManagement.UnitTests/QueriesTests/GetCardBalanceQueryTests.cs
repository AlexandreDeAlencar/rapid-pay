using RapidPay.CardManagement.App.Cards.Queries;

public class GetCardBalanceQueryHandlerTests
{
    private readonly FakeCardRepository _fakeRepository;

    public GetCardBalanceQueryHandlerTests()
    {
        _fakeRepository = new FakeCardRepository();
    }

    [Fact]
    public async Task Handle_ShouldReturnCardBalance_WhenCardIsFound()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var handler = new GetCardBalanceQueryHandler(_fakeRepository);
        var query = new GetCardBalance(cardId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.Value.Balance);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenCardIsNotFound()
    {
        // Arrange
        var handler = new GetCardBalanceQueryHandler(_fakeRepository);
        var query = new GetCardBalance(Guid.Empty);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
    }
}