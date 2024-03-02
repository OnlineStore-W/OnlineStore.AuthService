using MediatR;
using System.ComponentModel.DataAnnotations;

namespace OnlineStore.AuthService.Core.Handlers.ChangePassword;

public class ChangePasswordRequest : IRequest<ChangePasswordResponse>
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string OldPassword { get; set; }

    [Required]
    public string NewPassword { get; set; }

    [Required]
    public string ConfirmedNewPassword { get; set; }
}
