using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Interfaces.IService;

public interface IJwtTokenGenerator
{
    (string AccessToken, DateTime ExpiresAt) GenerateToken(User user);
}
