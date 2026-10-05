using LekhaCore.Application.DTOs.Dashboard;
using LekhaCore.Domain.Common;

namespace LekhaCore.Application.Interfaces.IService;

public interface IMenuService
{
    Task<Result<DashboardMenuResponse>> GetForCurrentUserAsync(CancellationToken cancellationToken = default);
}
