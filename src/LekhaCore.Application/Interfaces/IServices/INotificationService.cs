using LekhaCore.Application.DTOs.Notifications;
using LekhaCore.Domain.Common;

namespace LekhaCore.Application.Interfaces.IService;

public interface INotificationService
{
    Task<Result<NotificationDto>> CreateAsync(CreateNotificationRequest request);
    Task<Result<NotificationDto>> GetByIdAsync(Guid id);
    Task<Result<List<NotificationDto>>> GetMyNotificationsAsync();
    Task<Result> MarkAsReadAsync(Guid id);
    Task<Result> MarkAllAsReadAsync();
    Task NotifyUsersByRoleAsync(string role, string title, string message);
}
