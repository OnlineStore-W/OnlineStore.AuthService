using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.Abstractions;

public interface IEmailService
{
    public bool SendEmail(AuthUser user, string confirmationToken);
}
