using MediatR;
using Microsoft.AspNetCore.Identity.Data;

namespace OnlineStore.AuthService.Api.Schemas.Queries;

[ExtendObjectType("Query")]
public class AuthQuery
{
    private readonly IMediator _mediator;

    public AuthQuery(IMediator mediator)
    {
        _mediator = mediator;
    }

    //public async Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken)
    //{
    //    return await _mediator.Send(request, cancellationToken);
    //}
}