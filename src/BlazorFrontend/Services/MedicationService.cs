using BlazorFrontend.Dtos;
using BlazorFrontend.Dtos.Requests;

namespace BlazorFrontend.Services;

public class MedicationService(IHttpClientFactory httpClientFactory)
{
    public async Task<List<MedicationDto>> GetMedicationsAsync()
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.GetAsync("/medications");
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }
        
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<MedicationDto>>();
        return result ?? new List<MedicationDto>();
    }

    public async Task<MedicationDto?> CreateMedicationAsync(CreateMedicationRequest request)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.PostAsJsonAsync("/medications", request);
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MedicationDto>();
    }
}