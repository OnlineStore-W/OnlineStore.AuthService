using MediatR;

namespace OnlineStore.AuthService.Core.Handlers.Login;

public class LoginRequest : IRequest<LoginResponse>
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}
