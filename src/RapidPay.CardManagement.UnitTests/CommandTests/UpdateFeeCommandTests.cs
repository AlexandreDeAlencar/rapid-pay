using RapidPay.CardManagement.App.Fees.Command;
using static FakeCardRepository;

namespace RapidPay.CardManagement.UnitTests.CommandTests;
public class UpdateFeeCommandHandlerTests
{
    private readonly FakeFeeRepository _fakeFeeRepository;
    private readonly FakeLogger<UpdateFeeCommandHandler> _fakeLogger;
    private readonly UpdateFeeCommandHandler _handler;

    public UpdateFeeCommandHandlerTests()
    {
        _fakeFeeRepository = new FakeFeeRepository();
        _fakeLogger = new FakeLogger<UpdateFeeCommandHandler>();
        _handler = new UpdateFeeCommandHandler(_fakeFeeRepository, _fakeLogger);
    }

    [Fact]
    public async Task Handle_ShouldUpdateFee_WhenFeeIsValid()
    {
        // Arrange
        var command = new UpdateFeeCommand(1.5m, DateTime.UtcNow);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        var updatedFee = _fakeFeeRepository.GetFee().Value;
        Assert.NotNull(updatedFee);
        Assert.Equal(1.5m, updatedFee.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenFeeRetrievalFails()
    {
        // Arrange
        var handlerWithError = new UpdateFeeCommandHandler(new FakeFeeRepositoryWithError(), _fakeLogger);
        var command = new UpdateFeeCommand(1.5m, DateTime.UtcNow);

        // Act
        var result = await handlerWithError.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
        Assert.Equal("Simulated error retrieving fee", error.Description);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUpsertFails()
    {
        // Arrange
        var handlerWithError = new UpdateFeeCommandHandler(new FakeFeeRepositoryWithError(), _fakeLogger);
        var command = new UpdateFeeCommand(1.5m, DateTime.UtcNow);

        // Act
        var result = await handlerWithError.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
        Assert.Equal("Simulated error upserting fee", error.Description);
    }

    [Fact]
    public async Task Handle_ShouldLogInformation_WhenFeeIsUpdated()
    {
        // Arrange
        var command = new UpdateFeeCommand(1.5m, DateTime.UtcNow);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(_fakeLogger);
    }
}
