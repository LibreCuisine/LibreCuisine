using Identity.Common.Enums;

namespace Identity.API.Services;

public interface ITokenService
{
    string GenerateToken(string username, List<AuthScopes> scopes);
    bool VerityToken(string token);
}