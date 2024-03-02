using MediatR;

namespace OnlineStore.AuthService.Core.Handlers.ConfirmEmail;

public class ConfirmEmailRequest : IRequest<ConfirmEmailResponse>
{
    public string Email { get; set; }
    public string ConfirmationToken { get; set; }
}
