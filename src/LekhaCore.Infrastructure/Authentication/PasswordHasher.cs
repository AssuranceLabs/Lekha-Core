using LekhaCore.Application.Interfaces.IService;

namespace LekhaCore.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private static readonly string UnknownUserHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N"));

    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash) => BCrypt.Net.BCrypt.Verify(password, passwordHash);

    public void VerifyUnknownUser(string password) => BCrypt.Net.BCrypt.Verify(password, UnknownUserHash);
}
