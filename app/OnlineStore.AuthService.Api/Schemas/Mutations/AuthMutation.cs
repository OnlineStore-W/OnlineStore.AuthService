using HotChocolate.Authorization;
using MediatR;

namespace OnlineStore.AuthService.Api.Schemas.Mutations;

[ExtendObjectType("Mutation")]
public class AuthMutation
{
    private readonly IMediator _mediator;

    public AuthMutation(IMediator mediator)
    {
        _mediator = mediator;
    }

    //[Authorize(Roles = new[] { UserRoles.Admin })]
    //public async Task<RegisterManagerResponse> RegisterManager(RegisterManagerRequest request, CancellationToken cancellationToken)
    //{
    //    return await _mediator.Send(request, cancellationToken);
    //}

    //public async Task<RegisterAdminResponse> RegisterAdmin(RegisterAdminRequest request, CancellationToken cancellationToken)
    //{
    //    return await _mediator.Send(request, cancellationToken);
    //}

    //public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    //{
    //    return await _mediator.Send(request, cancellationToken);
    //}

    //public async Task<ConfirmEmailResponse> ConfirmEmail(ConfirmEmailRequest request, CancellationToken cancellationToken)
    //{
    //    return await _mediator.Send(request, cancellationToken);
    //}
}
