namespace BlazorFrontend.Services;

public sealed class BrowserAuthState(LocalStorageService storage) : IAuthState
{
    public Task<string?> GetUserIdAsync() =>
        storage.GetItemAsync("auth_userId");

    public Task<string?> GetAccessTokenAsync() =>
        storage.GetItemAsync("auth_token");

    public async Task SetAuthAsync(string userId, string accessToken)
    {
        await storage.SetItemAsync("auth_userId", userId);
        await storage.SetItemAsync("auth_token", accessToken);
    }

    public async Task ClearAsync()
    {
        await storage.RemoveItemAsync("auth_userId");
        await storage.RemoveItemAsync("auth_token");
    }
}