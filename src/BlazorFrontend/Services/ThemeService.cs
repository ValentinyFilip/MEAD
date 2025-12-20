using Microsoft.JSInterop;

namespace BlazorFrontend.Services;

public class ThemeService(IJSRuntime js)
{
    public ValueTask SetDarkModeAsync(bool isDark) =>
        js.InvokeVoidAsync("appTheme.setDarkMode", isDark);
}