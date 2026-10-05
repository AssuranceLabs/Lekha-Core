using LekhaCore.Application.Interfaces.IRepo;
using LekhaCore.Domain.Entities;
using LekhaCore.Infrastructure.EntityFramework;

namespace LekhaCore.Infrastructure.Persistence.Repositories;

public class NotificationRepository(AppDbContext context) : Repository<Notification>(context), INotificationRepository
{
    public Task<Notification?> GetByIdAsync(Guid id)
    {
        return GetAsync(notification => notification.Id == id);
    }

    public Task<List<Notification>> GetByUserIdAsync(Guid userId)
    {
        return GetManyAsync(notification => notification.UserId == userId);
    }
}
