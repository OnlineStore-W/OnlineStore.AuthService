using MediatR;
using OnlineStore.AuthService.Core.Handlers.RegisterAdmin;
using OnlineStore.AuthService.Core.Mediatr;
using System.Reflection;

namespace OnlineStore.AuthService.Api.Extentions;

public static class MediatRConfigurator
{
    public static void SetMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(RegisterAdminResponse).GetTypeInfo().Assembly));
        builder.Services.AddSingleton<IMediator, Mediator>();
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(GenericPipelineBehavior<,>));
    }
}