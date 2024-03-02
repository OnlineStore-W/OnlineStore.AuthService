using FluentValidation;
using MediatR;

namespace OnlineStore.AuthService.Core.Mediatr;

public class GenericPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public GenericPipelineBehavior()
    {
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await Validate(request, cancellationToken);

        var response = await next();
        return response;
    }

    private async Task Validate(TRequest request, CancellationToken cancellationToken)
    {
        var validatorType = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .SingleOrDefault(t => t.BaseType == typeof(AbstractValidator<TRequest>));

        if (validatorType != null)
        {
            var validator = (AbstractValidator<TRequest>)Activator.CreateInstance(validatorType);
            await validator.ValidateAndThrowAsync(request, cancellationToken);
        }
    }
}