using LekhaCore.Domain.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace LekhaCore.Application.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = ValidationMessages.RefreshTokenRequired)]
    public string RefreshToken { get; set; } = string.Empty;
}
