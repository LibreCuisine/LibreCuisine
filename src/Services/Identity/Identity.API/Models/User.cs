using System.ComponentModel.DataAnnotations;
using Identity.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    public User(string username, byte[] passwordHash, byte[] passwordSalt, List<AuthScopes> scopes)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Scopes = scopes;
    }

    public Guid Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public List<AuthScopes> Scopes { get; set; }
}