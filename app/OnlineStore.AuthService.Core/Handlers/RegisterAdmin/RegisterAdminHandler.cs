using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.AuthService.Core.Constants;
using OnlineStore.AuthService.Models;
using OnlineStore.AuthService.Core.Abstractions;

namespace OnlineStore.AuthService.Core.Handlers.RegisterAdmin;

public class RegisterAdminHandler : IRequestHandler<RegisterAdminRequest, RegisterAdminResponse>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IServiceProvider serviceProvider;
    private readonly IEmailService _emailService;

    public RegisterAdminHandler(
        IServiceProvider serviceProvider,
        IEmailService emailService
        )
    {
        _emailService = emailService;
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        var serviceScope = this.serviceProvider.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope();
        _userManager = serviceScope.ServiceProvider.GetService<UserManager<AuthUser>>();
        _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
    }

    public async Task<RegisterAdminResponse> Handle(RegisterAdminRequest request, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.FindByNameAsync(request.Username);
        if (userExists != null)
            throw new Exception("User already exists!");

        var admins = await _userManager.GetUsersInRoleAsync(UserRoles.Admin);
        if (admins.Any())
        {
            throw new Exception("Admin is already registered!");
        }

        var users = _userManager.Users;
        if (users.Any())
        {
            throw new Exception("DB is not empty!");
        }

        var user = new AuthUser()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Username,
            PasswordExpiration = DateTime.UtcNow.AddYears(10)
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var msg = string.Join("\r\n", result.Errors.Select(x => x.Description));
            throw new Exception(msg);
        }

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        }
        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        }

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        _emailService.SendEmail(user, confirmationToken);

        return new RegisterAdminResponse("New user created succesfully.");
    }
}
