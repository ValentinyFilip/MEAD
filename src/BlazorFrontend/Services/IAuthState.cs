namespace BlazorFrontend.Services;

public interface IAuthState
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetUserIdAsync();
    Task SetAuthAsync(string userId, string accessToken);
    Task ClearAsync();
}