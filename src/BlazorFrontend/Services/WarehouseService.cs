using BlazorFrontend.Dtos;
using BlazorFrontend.Dtos.Requests;

namespace BlazorFrontend.Services;

public class WarehouseService(IHttpClientFactory httpClientFactory)
{
    public async Task<List<StockDto>> GetWarehouseStocksAsync()
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.GetFromJsonAsync<WarehouseMedicationsResponse>("/warehouse/medications");
        return response?.Items ?? new List<StockDto>();
    }

    public async Task<StockDto?> AddStockAsync(CreateStockRequest request)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.PostAsJsonAsync("/warehouse/medications", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockDto>();
    }

    public async Task DeleteStockAsync(Guid stockId)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        await client.DeleteAsync($"/warehouse/medications/{stockId}");
    }

    private record WarehouseMedicationsResponse(List<StockDto> Items);
}