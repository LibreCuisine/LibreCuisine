using System.Security.Cryptography;

namespace Identity.API.Services;

public class HashService: IHashService
{
    public (byte[] PasswordHash, byte[] PasswordSalt) CreatePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var passwordSalt = hmac.Key;
        var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return (passwordHash, passwordSalt);
    }

    public bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        return passwordHash == hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    }
}