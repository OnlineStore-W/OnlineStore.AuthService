using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.Handlers.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordRequest, ChangePasswordResponse>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly IServiceProvider serviceProvider;

    public ChangePasswordHandler(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        var serviceScope = this.serviceProvider.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope();
        _userManager = serviceScope.ServiceProvider.GetService<UserManager<AuthUser>>();
    }

    public async Task<ChangePasswordResponse> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
            throw new Exception("User does not exists!");

        user.PasswordExpiration = DateTime.UtcNow.AddYears(10);
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var msg = string.Join("\r\n", result.Errors.Select(x => x.Description));
            throw new Exception(msg);
        }
        await _userManager.UpdateAsync(user);

        return new ChangePasswordResponse(true);
    }
}
