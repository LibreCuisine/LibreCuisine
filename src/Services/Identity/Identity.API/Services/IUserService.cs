using Identity.API.Models;
using Identity.Common.Dtos;
using Identity.Common.Enums;

namespace Identity.API.Services;

public interface IUserService
{
    List<AuthScopes> GetScopesOfUser(Guid id);
    User? ValidateUser(UserDto userDto);
    Guid RegisterUser(UserDto userDto);
}