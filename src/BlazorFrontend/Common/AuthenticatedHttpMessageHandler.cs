using System.Net;
using System.Net.Http.Headers;
using BlazorFrontend.Dtos;
using BlazorFrontend.Services;

namespace BlazorFrontend.Common;

public sealed class AuthenticatedHttpMessageHandler(
    IAuthState authState,
    IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? token = null;

        try
        {
            Console.WriteLine($"[AuthHandler] Request to: {request.RequestUri}");

            // 1. Attach current access token
            token = await authState.GetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"[AuthHandler] Token attached to request (length: {token.Length})");
            }
            else
            {
                Console.WriteLine($"[AuthHandler] WARNING: No token available!");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"[AuthHandler] JSInterop not available during prerendering: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthHandler] Error getting token: {ex.Message}");
        }

        var response = await base.SendAsync(request, cancellationToken);
        Console.WriteLine($"[AuthHandler] Response status: {response.StatusCode}");

        // Only try refresh on interactive render (not during prerendering)
        if (response.StatusCode == HttpStatusCode.Unauthorized && token != null)
        {
            Console.WriteLine("[AuthHandler] Got 401, attempting token refresh...");
            response.Dispose();

            // 2. Try refresh
            var refreshed = await TryRefreshAsync(cancellationToken);
            if (!refreshed)
            {
                Console.WriteLine("[AuthHandler] Refresh failed, clearing auth state");
                try
                {
                    await authState.ClearAsync();
                }
                catch (InvalidOperationException)
                {
                    // JavaScript interop not available during prerendering
                }

                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            Console.WriteLine("[AuthHandler] Refresh succeeded, retrying request");

            // 3. Retry request once with new access token
            try
            {
                token = await authState.GetAccessTokenAsync();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (InvalidOperationException)
            {
                // JavaScript interop not available during prerendering
            }

            var cloned = await CloneRequestAsync(request, cancellationToken);
            return await base.SendAsync(cloned, cancellationToken);
        }

        return response;
    }

    private async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
    {
        try
        {
            var userId = await authState.GetUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.WriteLine("[AuthHandler] Refresh failed: No userId");
                return false;
            }

            var client = httpClientFactory.CreateClient("AuthApi");

            var tokenRequest = new TokenRequest
            {
                UserId = userId
            };

            var resp = await client.PostAsJsonAsync("auth/refresh", tokenRequest, cancellationToken);
            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($"[AuthHandler] Refresh failed: {resp.StatusCode}");
                return false;
            }

            var payload = await resp.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
            if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
            {
                Console.WriteLine("[AuthHandler] Refresh failed: Invalid response");
                return false;
            }

            await authState.SetAuthAsync(payload.UserId, payload.AccessToken);
            Console.WriteLine("[AuthHandler] Refresh successful");
            return true;
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("[AuthHandler] Refresh failed: JSInterop not available");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthHandler] Refresh failed: {ex.Message}");
            return false;
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var opt in request.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(opt.Key), opt.Value);
        }

        if (request.Content != null)
        {
            var ms = new MemoryStream();
            await request.Content.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            var contentClone = new StreamContent(ms);

            foreach (var header in request.Content.Headers)
            {
                contentClone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            clone.Content = contentClone;
        }

        return clone;
    }
}