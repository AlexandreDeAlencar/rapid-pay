using ErrorOr;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.Domain.Fees.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.Application.Fees.Commands;

public record UpdateFeeCommand(decimal FeeRate, DateTime EffectiveDate) : IRequest<ErrorOr<Success>>;

public class UpdateFeeCommandHandler : IRequestHandler<UpdateFeeCommand, ErrorOr<Success>>
{
    private readonly IFeeRepository _feeRepository;
    private readonly ILogger<UpdateFeeCommandHandler> _logger;

    public UpdateFeeCommandHandler(IFeeRepository feeRepository, ILogger<UpdateFeeCommandHandler> logger)
    {
        _feeRepository = feeRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(UpdateFeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting fee update. New Fee Rate: {FeeRate}, Effective Date: {EffectiveDate}", request.FeeRate, request.EffectiveDate);

        var feeResult = _feeRepository.GetFee();

        if (feeResult.IsError)
        {
            _logger.LogWarning("Fee retrieval failed. Errors: {Errors}", feeResult.Errors);
            return feeResult.Errors;
        }

        var fee = feeResult.Value;

        var updateResult = fee.Update(request.FeeRate, request.EffectiveDate);

        if (updateResult.IsError)
        {
            _logger.LogWarning("Fee update failed. Errors: {Errors}", updateResult.Errors);
            return updateResult.Errors;
        }

        var saveResult = _feeRepository.UpdateFee(fee);

        if (saveResult.IsError)
        {
            _logger.LogError("Failed to save the updated fee. Errors: {Errors}", saveResult.Errors);
            return saveResult.Errors;
        }
        
        _logger.LogInformation("Fee updated and saved successfully.");
        
        return new Success();
    }
}
