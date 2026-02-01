using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorFrontend.Services;

// BrowserAuthState.cs - Add static cache

public sealed class BrowserAuthState : IAuthState
{
    private readonly ProtectedSessionStorage _sessionStorage;

    // Static cache shared across all instances in the same process
    private static string? _staticCachedUserId;
    private static string? _staticCachedToken;
    private static bool _staticIsInitialized = false;
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    public BrowserAuthState(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    private async Task EnsureInitializedAsync()
    {
        if (_staticIsInitialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (_staticIsInitialized) return; // Double-check after acquiring lock

            try
            {
                var userIdResult = await _sessionStorage.GetAsync<string>("auth_userId");
                var tokenResult = await _sessionStorage.GetAsync<string>("auth_token");

                _staticCachedUserId = userIdResult.Success ? userIdResult.Value : null;
                _staticCachedToken = tokenResult.Success ? tokenResult.Value : null;
                _staticIsInitialized = true;

                Console.WriteLine(
                    $"[BrowserAuthState] Initialized from storage - UserId exists: {!string.IsNullOrEmpty(_staticCachedUserId)}, Token exists: {!string.IsNullOrEmpty(_staticCachedToken)}");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("[BrowserAuthState] JSInterop not available yet (prerendering)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BrowserAuthState] Initialization failed: {ex.Message}");
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<string?> GetUserIdAsync()
    {
        await EnsureInitializedAsync();
        Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] GetUserIdAsync: {_staticCachedUserId ?? "null"}");
        return _staticCachedUserId;
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        await EnsureInitializedAsync();
        var tokenPreview = !string.IsNullOrEmpty(_staticCachedToken)
            ? _staticCachedToken.Substring(0, Math.Min(20, _staticCachedToken.Length)) + "..."
            : "null";
        Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] GetAccessTokenAsync: {tokenPreview}");
        return _staticCachedToken;
    }

    public async Task SetAuthAsync(string userId, string accessToken)
    {
        Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] SetAuthAsync: UserId={userId}, Token length={accessToken?.Length ?? 0}");

        await _initLock.WaitAsync();
        try
        {
            // Update static cache immediately
            _staticCachedUserId = userId;
            _staticCachedToken = accessToken;
            _staticIsInitialized = true;

            // Persist to session storage
            try
            {
                await _sessionStorage.SetAsync("auth_userId", userId);
                await _sessionStorage.SetAsync("auth_token", accessToken);
                Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] SetAuthAsync: Successfully saved to session storage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] SetAuthAsync storage ERROR: {ex.Message}");
            }
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task ClearAsync()
    {
        Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] ClearAsync: Removing auth data");

        await _initLock.WaitAsync();
        try
        {
            // Clear static cache immediately
            _staticCachedUserId = null;
            _staticCachedToken = null;
            _staticIsInitialized = false;

            // Clear session storage
            try
            {
                await _sessionStorage.DeleteAsync("auth_userId");
                await _sessionStorage.DeleteAsync("auth_token");
                Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] ClearAsync: Successfully cleared session storage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BrowserAuthState:{GetHashCode()}] ClearAsync ERROR: {ex.Message}");
            }
        }
        finally
        {
            _initLock.Release();
        }
    }
}