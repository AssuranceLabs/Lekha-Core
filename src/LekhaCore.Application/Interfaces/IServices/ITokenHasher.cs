namespace LekhaCore.Application.Interfaces.IService;

public interface ITokenHasher
{
    string Hash(string value);
}
