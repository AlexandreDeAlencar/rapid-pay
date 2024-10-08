﻿using System.Collections;
using ErrorOr;
using RapidPay.CardManagement.App.Fees.Command;
using RapidPay.CardManagement.Domain.Ports;
using RapidPay.CardManagement.UnitTests.MockedServices;

public class UpdateFeeCommandHandlerTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 1.5m, "Simulated error retrieving fee", new FakeFeeRepositoryWithError() };
        yield return new object[] { 1.5m, "Simulated error upserting fee", new FakeFeeRepositoryWithError() };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class UpdateFeeCommandHandlerTests
{
    private readonly FakeLogger<UpdateFeeCommandHandler> _fakeLogger;

    public UpdateFeeCommandHandlerTests()
    {
        _fakeLogger = new FakeLogger<UpdateFeeCommandHandler>();
    }

    [Theory]
    [InlineData(1.5, "Simulated error retrieving fee", true)]
    [InlineData(1.5, "Simulated error upserting fee", false)]
    public async Task Handle_ShouldReturnFailure_WhenOperationFails(decimal feeRate, string expectedErrorMessage, bool simulateRetrievalError)
    {
        // Arrange
        IFeeRepository fakeRepository = new FakeFeeRepositoryWithError();

        var handler = new UpdateFeeCommandHandler(fakeRepository, _fakeLogger);
        var command = new UpdateFeeCommand(feeRate, DateTime.UtcNow);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
    }

    [Theory]
    [ClassData(typeof(UpdateFeeCommandHandlerTestData))]
    public async Task Handle_ShouldReturnFailure_WhenOperationFails_ClassData(decimal feeRate, string expectedErrorMessage, IFeeRepository fakeRepository)
    {
        // Arrange
        var handler = new UpdateFeeCommandHandler(fakeRepository, _fakeLogger);
        var command = new UpdateFeeCommand(feeRate, DateTime.UtcNow);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
    }

    [Fact]
    public async Task Handle_ShouldUpdateFee_WhenFeeIsValid()
    {
        // Arrange
        var fakeRepository = new FakeFeeRepository();
        var handler = new UpdateFeeCommandHandler(fakeRepository, _fakeLogger);
        var initialFeeValue = fakeRepository.GetFee().Value.Value;
        var command = new UpdateFeeCommand(1.5m, DateTime.UtcNow);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsError);
        var updatedFee = fakeRepository.GetFee().Value;
        Assert.NotNull(updatedFee);

        // Check if the fee was multiplied by a random multiplier between 0 and 2
        Assert.True(updatedFee.Value >= 0 && updatedFee.Value <= initialFeeValue * 2);
    }
}
