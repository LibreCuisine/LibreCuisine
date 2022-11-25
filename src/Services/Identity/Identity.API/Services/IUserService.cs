using Identity.Common.Dtos;
using Identity.Common.Enums;

namespace Identity.API.Services;

public interface IUserService
{
    List<AuthScopes> GetScopesOfUser(Guid userId);
    bool ValidateUser(UserDto userDto);
    Guid RegisterUser(UserDto userDto);
}