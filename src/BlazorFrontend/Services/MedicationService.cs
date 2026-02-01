using BlazorFrontend.Dtos;
using BlazorFrontend.Dtos.Requests;

namespace BlazorFrontend.Services;

public class MedicationService(IHttpClientFactory httpClientFactory)
{
    public async Task<List<MedicationDto>> GetMedicationsAsync()
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.GetFromJsonAsync<List<MedicationDto>>("/medications");
        return response ?? new List<MedicationDto>();
    }

    public async Task<MedicationDto?> CreateMedicationAsync(CreateMedicationRequest request)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.PostAsJsonAsync("/medications", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MedicationDto>();
    }
}