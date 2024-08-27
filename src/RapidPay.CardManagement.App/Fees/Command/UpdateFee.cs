using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using RapidPay.CardManagement.Domain.Fees.Models;
using RapidPay.CardManagement.Domain.Ports;

namespace RapidPay.CardManagement.App.Fees.Command;

public record UpdateFeeCommand(decimal FeeRate, DateTime EffectiveDate) : IRequest<ErrorOr<Success>>;

public class UpdateFeeCommandValidator : AbstractValidator<UpdateFeeCommand>
{
    public UpdateFeeCommandValidator()
    {
        RuleFor(x => x.FeeRate)
            .GreaterThan(0).WithMessage("Fee rate must be greater than zero.");

        RuleFor(x => x.EffectiveDate)
            .NotEmpty().WithMessage("Effective date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Effective date cannot be in the future.");
    }
}

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

        Fee fee = null;
        var feeResult = _feeRepository.GetFee();

        if (feeResult.IsError)
        {
            _logger.LogWarning("Fee retrieval failed. Errors: {Errors}", feeResult.Errors);
            Fee.Create(Guid.NewGuid(), request.FeeRate, request.EffectiveDate)
            .Match<ErrorOr<Fee>>(
                value => fee = value,
                errors =>
                {
                    _logger.LogWarning("Fee retrieval failed. Errors: {Errors}", errors);
                    return errors;
                });
        }
        else
        {
            fee = feeResult.Value;
        }

        var updateResult = fee.Update(request.FeeRate, request.EffectiveDate);

        if (updateResult.IsError)
        {
            _logger.LogWarning("Fee update failed. Errors: {Errors}", updateResult.Errors);
            return updateResult.Errors;
        }

        var saveResult = _feeRepository.UpsertFee(fee);

        if (saveResult.IsError)
        {
            _logger.LogError("Failed to save the updated fee. Errors: {Errors}", saveResult.Errors);
            return saveResult.Errors;
        }

        _logger.LogInformation("Fee updated and saved successfully.");

        return new Success();
    }
}
