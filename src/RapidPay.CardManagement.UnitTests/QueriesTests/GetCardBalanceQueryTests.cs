using RapidPay.CardManagement.App.Cards.Queries;
using static FakeCardRepository;

public class GetCardBalanceQueryHandlerTests
{
    private readonly FakeCardRepository _fakeRepository;
    private readonly FakeLogger<GetCardBalanceQueryHandler> _fakeLogger;

    public GetCardBalanceQueryHandlerTests()
    {
        _fakeRepository = new FakeCardRepository();
        _fakeLogger = new FakeLogger<GetCardBalanceQueryHandler>();
    }

    [Fact]
    public async Task Handle_ShouldReturnCardBalance_WhenCardIsFound()
    {
        // Arrange
        var cardId = Guid.NewGuid();
        var handler = new GetCardBalanceQueryHandler(_fakeRepository, _fakeLogger);
        var query = new GetCardBalanceQuery(cardId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(0, result.Value.Balance);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenCardIsNotFound()
    {
        // Arrange
        var handler = new GetCardBalanceQueryHandler(_fakeRepository, _fakeLogger);
        var query = new GetCardBalanceQuery(Guid.Empty);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
    }
}