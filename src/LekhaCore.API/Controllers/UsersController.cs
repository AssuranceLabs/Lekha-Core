using LekhaCore.Api.Contracts;
using LekhaCore.Application.DTOs.Users;
using LekhaCore.Application.Interfaces.IService;
using LekhaCore.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LekhaCore.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/users")]
public class UsersController(IUserService users) : ApiControllerBase
{
    //[Authorize(Policy = Permissions.UsersCreate)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await users.CreateAsync(request, cancellationToken);
        if (result.IsFailure)
            return ProcessResult(result);

        return StatusCode(StatusCodes.Status201Created, ApiResponse<UserDto>.Success(result.Data));
    }
}
