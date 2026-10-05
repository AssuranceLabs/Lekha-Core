using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task RevokeAllForUserAsync(Guid userId, DateTime revokedAtUtc, CancellationToken cancellationToken = default);
}
