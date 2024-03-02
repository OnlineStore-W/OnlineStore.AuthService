using MediatR;
using OnlineStore.AuthService.Core.Handlers.Login;
using LoginRequest = OnlineStore.AuthService.Core.Handlers.Login.LoginRequest;

namespace OnlineStore.AuthService.Api.Schemas.Queries;

[ExtendObjectType("Query")]
public class AuthQuery
{
    private readonly IMediator _mediator;

    public AuthQuery(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(request, cancellationToken);
    }
}