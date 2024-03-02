using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineStore.AuthService.Core.Constants;
using OnlineStore.AuthService.Models;
using System.Net.Mail;
using System.Net;

namespace OnlineStore.AuthService.Core.Handlers.RegisterAdmin;

public class RegisterAdminHandler : IRequestHandler<RegisterAdminRequest, RegisterAdminResponse>
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IConfiguration _configuration;
    private readonly IServiceProvider serviceProvider;

    public RegisterAdminHandler(
        IConfiguration configuration,
        IServiceProvider serviceProvider
        )
    {
        _configuration = configuration;
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

        AuthUser user = new()
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

        string to = _configuration["GmailClient:Host"];
        using (var client = new SmtpClient())
        {
            client.Host = _configuration["GmailClient:Host"];
            client.Port = int.Parse(_configuration["GmailClient:Port"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_configuration["GmailClient:Email"], _configuration["GmailClient:AppPassword"]);
            using (var message = new MailMessage(
                from: new MailAddress(_configuration["GmailClient:Email"], "OnlineStore"),
                to: new MailAddress(user.Email, user.UserName)
                ))
            {
                message.Subject = $"ConfirmationToken for {user.UserName}";
                message.Body = $"User this token to confirm your email - `{confirmationToken}`";

                client.Send(message);
            }
        }

        return new RegisterAdminResponse("New user created succesfully.");
    }
}
