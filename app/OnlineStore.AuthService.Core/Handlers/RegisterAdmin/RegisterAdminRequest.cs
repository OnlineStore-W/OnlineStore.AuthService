using MediatR;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.AuthService.Core.Handlers.RegisterAdmin;

public class RegisterAdminRequest : IRequest<RegisterAdminResponse>
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string ConfirmedPassword { get; set; }
}
