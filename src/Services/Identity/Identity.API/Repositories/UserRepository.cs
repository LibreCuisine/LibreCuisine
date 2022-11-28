using Identity.API.Data;
using Identity.API.Models;

namespace Identity.API.Repositories;

public class UserRepository: IUserRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(IdentityDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
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
        try
        {
            return _context.SaveChanges() > 0;
        }
        catch (Exception e)
        {
            _logger.LogError("Something went wrong while saving changes: {E}", e);
            return false;
        }
    }
}