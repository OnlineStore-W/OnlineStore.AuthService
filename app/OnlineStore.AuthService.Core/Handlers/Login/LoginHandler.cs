using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OnlineStore.AuthService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineStore.AuthService.Core.Handlers.Login;

public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly UserManager<AuthUser> _userManager;

    private readonly IConfiguration _configuration;
    private readonly IServiceProvider serviceProvider;

    public LoginHandler(
        IConfiguration configuration,
        IServiceProvider serviceProvider
        )
    {
        _configuration = configuration;
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        var serviceScope = this.serviceProvider.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope();
        _userManager = serviceScope.ServiceProvider.GetService<UserManager<AuthUser>>();
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            throw new Exception($"Username or password is not correct.");
        }
        if (!user.EmailConfirmed)
        {
            throw new Exception("Email is not confirmed.");
        }
        if (user.AccessFailedCount >= 5)
        {
            throw new Exception("Account is blocked");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (today >= DateOnly.FromDateTime(user.PasswordExpiration))
        {
            throw new Exception("Password expired.");
        }

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.ResetAccessFailedCountAsync(user);

            return await GetAuthToken(user);
        }
        await _userManager.AccessFailedAsync(user);
        throw new Exception("Unauthorized");
    }

    private async Task<LoginResponse> GetAuthToken(AuthUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("userId", user.Id.ToString())
            };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var tokenId = GetTokenId(authClaims);

        return new LoginResponse(new JwtSecurityTokenHandler().WriteToken(tokenId), tokenId.ValidTo);
    }

    private JwtSecurityToken GetTokenId(List<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var tokenId = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

        return tokenId;
    }
}

