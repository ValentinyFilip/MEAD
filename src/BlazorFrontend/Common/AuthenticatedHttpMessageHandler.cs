using System.Net;
using System.Net.Http.Headers;
using BlazorFrontend.Dtos;
using BlazorFrontend.Services;

namespace BlazorFrontend.Common;

public sealed class AuthenticatedHttpMessageHandler(
    IAuthState authState,
    IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    public static event Func<Task>? OnUnauthorized;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? token = null;

        try
        {
            // 1. Attach current access token
            token = await authState.GetAccessTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop not available during prerendering
            // We'll let the request continue and handle 401 in the response
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Only try refresh on interactive render (not during prerendering)
        if (response.StatusCode == HttpStatusCode.Unauthorized && token != null)
        {
            response.Dispose();

            // 2. Try refresh (cookie carries refresh token, body has UserId)
            var refreshed = await TryRefreshAsync(cancellationToken);
            if (!refreshed)
            {
                try
                {
                    await authState.ClearAsync();
                }
                catch (InvalidOperationException)
                {
                    // JavaScript interop not available during prerendering
                }

                // Trigger the unauthorized event for redirect
                if (OnUnauthorized != null)
                {
                    await OnUnauthorized.Invoke();
                }

                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            // 3. Retry request once with new access token
            try
            {
                token = await authState.GetAccessTokenAsync();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    request.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
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
                return false;
            }

            var payload = await resp.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
            if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
            {
                return false;
            }

            await authState.SetAuthAsync(payload.UserId, payload.AccessToken);
            return true;
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop not available during prerendering
            return false;
        }
        catch (Exception)
        {
            // Any other error during refresh
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