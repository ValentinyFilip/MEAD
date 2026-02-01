using BlazorFrontend.Dtos;
using BlazorFrontend.Dtos.Requests;

namespace BlazorFrontend.Services;

public class WarehouseService(IHttpClientFactory httpClientFactory)
{
    public async Task<List<StockDto>> GetWarehouseStocksAsync()
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.GetAsync("/warehouse/medications");

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<StockDto>>();
        return result ?? [];
    }

    public async Task<StockDto?> AddStockAsync(CreateStockRequest request)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.PostAsJsonAsync("/warehouse/medications", request);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockDto>();
    }

    public async Task DeleteStockAsync(Guid stockId)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.DeleteAsync($"/warehouse/medications/{stockId}");

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }

        response.EnsureSuccessStatusCode();
    }
}