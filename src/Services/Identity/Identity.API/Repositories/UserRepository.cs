using Identity.API.Data;
using Identity.API.Models;

namespace Identity.API.Repositories;

public class UserRepository: IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }
    
    public void CreateUser(User user)
    {
        _context.Users.Add(user);
    }
    public User? GetUserByUsername(string username)
    {
        return _context.Users.FirstOrDefault(x => x.Username == username);
    }
    public User? GetUserById(Guid id)
    {
        return _context.Users.FirstOrDefault(x => x.Id == id);
    }
    public bool SaveChanges()
    {
        return _context.SaveChanges() > 0;
    }
}