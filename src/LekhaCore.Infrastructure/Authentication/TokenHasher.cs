using System.Security.Cryptography;
using System.Text;
using LekhaCore.Application.Interfaces.IService;

namespace LekhaCore.Infrastructure.Authentication;

public sealed class TokenHasher : ITokenHasher
{
    public string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
