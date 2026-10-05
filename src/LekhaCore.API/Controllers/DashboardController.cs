using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LekhaCore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/dashboard")]
public class DashboardController(IMenuService menuService) : ApiControllerBase
{
    [Authorize(Policy = Permissions.DashboardView)]
    [HttpGet("menus")]
    public async Task<IActionResult> GetMenus(CancellationToken cancellationToken)
    {
        var result = await menuService.GetForCurrentUserAsync(cancellationToken);
        return ProcessResult(result);
    }
}
