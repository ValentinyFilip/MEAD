namespace BlazorFrontend.Services;

public sealed class BrowserAuthState(LocalStorageService storage) : IAuthState
{
    public async Task<string?> GetUserIdAsync()
    {
        return await storage.GetItemAsync("auth_userId");
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var token = await storage.GetItemAsync("auth_token");
        // Debug: Check if token exists
        Console.WriteLine($"GetAccessTokenAsync: Token exists = {!string.IsNullOrEmpty(token)}");
        return token;
    }

    public async Task SetAuthAsync(string userId, string accessToken)
    {
        Console.WriteLine($"SetAuthAsync: UserId = {userId}, Token length = {accessToken?.Length ?? 0}");
        await storage.SetItemAsync("auth_userId", userId);
        await storage.SetItemAsync("auth_token", accessToken);
    }

    public async Task ClearAsync()
    {
        Console.WriteLine("ClearAsync: Removing auth data");
        await storage.RemoveItemAsync("auth_userId");
        await storage.RemoveItemAsync("auth_token");
    }
}