using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.AuthService.Core.Constants;
using OnlineStore.AuthService.Models;
using System.Net.Mail;
using System.Net;
using OnlineStore.AuthService.Core.Abstractions;

namespace OnlineStore.AuthService.Core.Handlers.RegisterManager;

public class RegisterManagerHandler : IRequestHandler<RegisterManagerRequest, RegisterManagerResponse>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IServiceProvider serviceProvider;
    private readonly IEmailService _emailService;

    public RegisterManagerHandler(
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

    public async Task<RegisterManagerResponse> Handle(RegisterManagerRequest request, CancellationToken cancellationToken)
    {
        var userExists = await _userManager.FindByNameAsync(request.Username);
        if (userExists != null)
            throw new Exception("User already exists!");

        var emailExists = await _userManager.FindByEmailAsync(request.Email);
        if (emailExists != null)
            throw new Exception("User with this email is already registered!");

        AuthUser user = new()
        {
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Username,
            PasswordExpiration = DateTime.UtcNow.AddDays(-1)
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var msg = string.Join("\r\n", result.Errors.Select(x => x.Description));
            throw new Exception(msg);
        }

        if (!await _roleManager.RoleExistsAsync(UserRoles.Manager))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));
        if (!await _roleManager.RoleExistsAsync(UserRoles.Manager))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));

        if (await _roleManager.RoleExistsAsync(UserRoles.Manager))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Manager);
        }
        if (await _roleManager.RoleExistsAsync(UserRoles.Manager))
        {
            await _userManager.AddToRoleAsync(user, UserRoles.Manager);
        }

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        _emailService.SendEmail(user, confirmationToken);

        return new RegisterManagerResponse("New manager created succesfully.");
    }
}
