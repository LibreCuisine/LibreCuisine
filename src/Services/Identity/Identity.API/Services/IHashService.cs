namespace Identity.API.Services;

public interface IHashService
{
    (byte[] PasswordHash, byte[] PasswordSalt) CreatePasswordHash(string password);
    bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt);
}