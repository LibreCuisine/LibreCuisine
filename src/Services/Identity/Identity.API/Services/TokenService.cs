using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Identity.API.Options;
using Identity.Common.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Identity.API.Services;

public class TokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly string _privateKey;
    private readonly string _publicKey;

    public TokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _privateKey = File.ReadAllText(_options.PrivateKeyPath);
        _publicKey = File.ReadAllText(_options.PublicKeyPath);
    }
    public string GenerateToken(string username, List<AuthScopes> scopes)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(_privateKey);
        var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
        };
        claims.AddRange(scopes.Select(scope => new Claim(CustomJwtNames.Scope, scope.ToString())));

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Issuer,
            claims, expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool VerityToken(string token)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(_publicKey);
        var validationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new RsaSecurityKey(rsa),
            ValidateAudience = false,
            ValidateIssuer = false,
        };
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}