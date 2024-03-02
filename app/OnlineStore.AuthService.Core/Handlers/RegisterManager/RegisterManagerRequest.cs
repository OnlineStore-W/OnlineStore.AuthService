using MediatR;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.AuthService.Core.Handlers.RegisterManager;

public class RegisterManagerRequest : IRequest<RegisterManagerResponse>
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
