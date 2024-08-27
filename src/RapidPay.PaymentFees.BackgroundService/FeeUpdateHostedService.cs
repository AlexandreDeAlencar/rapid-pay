using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.App.Fees.Command;

namespace RapidPay.PaymentFees.BackgroundService;
public class FeeUpdateHostedService : IHostedService, IDisposable
{
    private readonly ILogger<FeeUpdateHostedService> _logger;
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer;
    public FeeUpdateHostedService(ILogger<FeeUpdateHostedService> logger,
        IMediator mediator,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fee Update Hosted Service is starting.");

        _timer = new Timer(UpdateFee, null, TimeSpan.Zero, TimeSpan.FromHours(1));

        return Task.CompletedTask;
    }

    private async void UpdateFee(object state)
    {
        _logger.LogInformation("Fee Update Hosted Service is running.");

        try
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var random = new Random();
                decimal newFeeRate = (decimal)(random.NextDouble() * 2);
                DateTime effectiveDate = DateTime.UtcNow;

                var command = new UpdateFeeCommand(newFeeRate, effectiveDate);

                var result = await mediator.Send(command);

                if (result.IsError)
                {
                    _logger.LogWarning("Failed to update fee: {Errors}", result.Errors);
                }
                else
                {
                    _logger.LogInformation("Fee updated successfully. New Fee Rate: {NewFeeRate}, Effective Date: {EffectiveDate}", newFeeRate, effectiveDate);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the fee.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fee Update Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
