namespace ApiService.Configuration;

public class JwtConfiguration
{
    public string SigningKey { get; set; }
    public int AccessExpirationInMinutes { get; set; }
    public int RefreshExpirationInDays { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}