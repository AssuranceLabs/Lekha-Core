namespace LekhaCore.Application.Interfaces;

public interface IWorkContext
{
    string CurrentUserId { get; }

    string? IpAddress { get; }

    string? CorrelationId { get; }

    void SetCurrentUser(Guid userId);
}
