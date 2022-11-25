using Identity.API.Models;
using Identity.API.Repositories;
using Identity.Common.Dtos;
using Identity.Common.Enums;

namespace Identity.API.Services;

public class UserService: IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;

    public UserService(IUserRepository userRepository, IHashService hashService)
    {
        _userRepository = userRepository;
        _hashService = hashService;
    }
    public List<AuthScopes> GetScopesOfUser(Guid id)
    {
        var user = _userRepository.GetUserById(id);
        return user is null ? new List<AuthScopes>() : user.Scopes;
    }

    public bool ValidateUser(UserDto userDto)
    {
        var user = _userRepository.GetUserByUsername(userDto.Username);
        return user is not null && _hashService.VerifyPassword(userDto.Password, user.PasswordHash, user.PasswordSalt);
    }

    public Guid RegisterUser(UserDto userDto)
    {
        var hash = _hashService.CreatePasswordHash(userDto.Password);
        var user = new User(userDto.Username, hash.PasswordHash, hash.PasswordSalt, GetDefaultAuthScopes());
        _userRepository.CreateUser(user);
        return _userRepository.SaveChanges() ? user.Id : Guid.Empty;
    }

    private static List<AuthScopes> GetDefaultAuthScopes() => new() {AuthScopes.RecipeCreate};
}