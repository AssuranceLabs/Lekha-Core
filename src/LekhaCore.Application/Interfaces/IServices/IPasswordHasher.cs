namespace LekhaCore.Application.Interfaces.IService;

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string password, string passwordHash);

    void VerifyUnknownUser(string password);
}
