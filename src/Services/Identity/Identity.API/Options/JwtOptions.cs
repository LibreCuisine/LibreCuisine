namespace Identity.API.Options;

public class JwtOptions
{
    public const string Jwt = "Jwt";
    
    public string PrivateKeyPath { get; set; } = string.Empty;
    public string PublicKeyPath { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}