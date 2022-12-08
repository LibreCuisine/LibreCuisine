using Identity.API.Models;

namespace Identity.API.Repositories;

public interface IUserRepository
{
    void CreateUser(User user);
    User? GetUserByUsername(string username);
    User? GetUserById(Guid id);
    bool SaveChanges();
}