using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(v => v.Errors).Where(v => v != null).ToList();

            if (failures.Count > 0)
            {
                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}
