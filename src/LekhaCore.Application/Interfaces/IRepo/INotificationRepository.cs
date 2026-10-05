using LekhaCore.Domain.Entities;

namespace LekhaCore.Application.Interfaces.IRepo;

public interface INotificationRepository : IRepository<Notification>
{
    Task<Notification?> GetByIdAsync(Guid id);

    Task<List<Notification>> GetByUserIdAsync(Guid userId);
}
