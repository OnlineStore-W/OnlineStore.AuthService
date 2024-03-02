using Microsoft.AspNetCore.Identity;

namespace OnlineStore.AuthService.Models;

public class AuthUser : IdentityUser
{
    public DateTime PasswordExpiration { get; set; }
}