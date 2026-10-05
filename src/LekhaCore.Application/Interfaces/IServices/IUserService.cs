using LekhaCore.Application.DTOs.Users;
using LekhaCore.Domain.Common;

namespace LekhaCore.Application.Interfaces.IService;

public interface IUserService
{
    Task<Result<UserDto>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}
