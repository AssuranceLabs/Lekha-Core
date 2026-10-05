using LekhaCore.Domain.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace LekhaCore.Application.DTOs.Users;

public sealed class CreateUserRequest
{
    [Required(ErrorMessage = ValidationMessages.FullNameRequired)]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.EmailRequired)]
    [EmailAddress(ErrorMessage = ValidationMessages.EmailInvalid)]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = ValidationMessages.PasswordRequired)]
    [MinLength(8, ErrorMessage = ValidationMessages.PasswordWeak)]
    [MaxLength(128)]
    public string Password { get; set; } = string.Empty;

    public List<string> RoleNames { get; set; } = [];
}

public sealed class UserDto
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public IReadOnlyList<string> Roles { get; set; } = [];
}
