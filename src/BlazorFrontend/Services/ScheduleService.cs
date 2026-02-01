using BlazorFrontend.Dtos;
using BlazorFrontend.Dtos.Requests;

namespace BlazorFrontend.Services;

public class ScheduleService(IHttpClientFactory httpClientFactory)
{
    public async Task<List<ScheduleDto>> GetSchedulesAsync(Guid? medicationId = null)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        const string url = "/api/schedules";
        var response = await client.GetFromJsonAsync<SchedulesResponse>(url);
        return response?.Items ?? new List<ScheduleDto>();
    }

    public async Task<ScheduleDto?> AddScheduleAsync(CreateScheduleRequest request)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.PostAsJsonAsync("/schedules", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ScheduleDto>();
    }

    public async Task DeleteScheduleAsync(Guid scheduleId)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        await client.DeleteAsync($"/schedules/{scheduleId}");
    }

    private record SchedulesResponse(List<ScheduleDto> Items);
}