using RapidPay.CardManagement.App.Cards.Commands;
using ErrorOr;
using static FakeCardRepository;

public class CreateCardCommandHandlerTests
{
    private readonly FakeCardRepository _fakeCardRepository;
    private CreateCardCommandHandler _handler;

    public CreateCardCommandHandlerTests()
    {
        _fakeCardRepository = new FakeCardRepository();
        _handler = new CreateCardCommandHandler(_fakeCardRepository, new FakeLogger<CreateCardCommandHandler>());
    }

    [Fact]
    public async Task Handle_ShouldReturnCreated_WhenCardIsSuccessfullyCreated()
    {
        // Arrange
        var command = new CreateCard("JohnDoe", "User123");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsType<Guid>(result.Value);
        Assert.Single(_fakeCardRepository.AddedCards);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenCardCreationFails()
    {
        // Arrange
        var command = new CreateCard("JohnDoe", "User123");

        // Use the error-producing repository
        _handler = new CreateCardCommandHandler(new FakeCardRepositoryWithError(), new FakeLogger<CreateCardCommandHandler>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _handler.Handle(command, CancellationToken.None);
        });

        // Additional assertions if necessary
        Assert.Equal("Simulated error during card creation", exception.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserNameIsNullOrEmpty()
    {
        // Arrange
        var command = new CreateCard("", "User123"); // Empty UserName

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
        Assert.Equal("Unable to create a new card", error.Description);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenUserIdIsNullOrEmpty()
    {
        // Arrange
        var command = new CreateCard("JohnDoe", ""); // Empty UserId

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
        var error = result.FirstError;
        Assert.NotNull(error);
        Assert.Equal("Invalid userId", error.Description);
    }
}
