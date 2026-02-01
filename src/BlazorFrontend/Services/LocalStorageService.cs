using Microsoft.JSInterop;

namespace BlazorFrontend.Services;

public class LocalStorageService(IJSRuntime jsRuntime)
{
    public async Task<string?> GetItemAsync(string key)
    {
        try
        {
            return await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop not available during prerendering
            return null;
        }
    }

    public async Task SetItemAsync(string key, string value)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop not available during prerendering
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop not available during prerendering
        }
    }
}