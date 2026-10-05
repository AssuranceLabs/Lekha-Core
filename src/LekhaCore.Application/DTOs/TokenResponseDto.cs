namespace LekhaCore.Application.DTOs;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime AccessTokenExpiresAtUtc { get; set; }

    public IReadOnlyList<string> Roles { get; set; } = [];

    public string FullName { get; set; } = string.Empty;
}
