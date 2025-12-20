namespace BlazorFrontend.Dtos;

public sealed class TokenResponse
{
    public string UserId { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
}