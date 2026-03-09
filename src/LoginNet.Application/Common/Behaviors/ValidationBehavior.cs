using FluentValidation;
using LoginNet.Application.Common.Interfaces;
using System.Reflection;

namespace LoginNet.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    // Here we have a challenge: TResponse might be Result<T> or something else.
                    // For this project, we primarily use Result<T>.
                    
                    if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var resultType = typeof(TResponse).GetGenericArguments()[0];
                        var failureMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, [typeof(string), typeof(Enum)]);
                        
                        // Join errors into a string
                        var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
                        
                        return (TResponse)failureMethod!.Invoke(null, [errorMessage, null])!;
                    }
                    
                    if (typeof(TResponse) == typeof(Result))
                    {
                        return (TResponse)(object)Result.Failure(string.Join("; ", failures.Select(f => f.ErrorMessage)));
                    }

                    throw new ValidationException(failures);
                }
            }
            return await next();
        }
    }
}
