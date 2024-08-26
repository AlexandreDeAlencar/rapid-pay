using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace RapidPay.CardManagement.App.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IErrorOr
    {
        private readonly IValidator<TRequest>? _validator;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(IValidator<TRequest>? validator = null, ILogger<ValidationBehavior<TRequest, TResponse>> logger = null)
        {
            _validator = validator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validator is null)
            {
                _logger.LogInformation("No validator found for {RequestName}. Skipping validation.", typeof(TRequest).Name);
                return await next();
            }

            _logger.LogInformation("Validating request {RequestName}.", typeof(TRequest).Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (validationResult.IsValid)
            {
                _logger.LogInformation("Validation succeeded for request {RequestName}.", typeof(TRequest).Name);
                return await next();
            }

            _logger.LogWarning("Validation failed for request {RequestName}. Errors: {Errors}",
                typeof(TRequest).Name,
                string.Join(", ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));

            var errors = validationResult.Errors.ConvertAll(validationFailure => Error.Validation(validationFailure.PropertyName, validationFailure.ErrorMessage));
            return (dynamic)errors;
        }
    }
}
