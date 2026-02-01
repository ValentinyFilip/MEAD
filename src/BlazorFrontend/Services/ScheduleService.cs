using BlazorFrontend.Dtos;
using BlazorFrontend.Dtos.Requests;

namespace BlazorFrontend.Services;

public class ScheduleService(IHttpClientFactory httpClientFactory)
{
    public async Task<List<ScheduleDto>> GetSchedulesAsync(Guid? medicationId = null)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var url = "/schedules";
        var response = await client.GetAsync(url);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SchedulesResponse>();
        return result?.Items ?? new List<ScheduleDto>();
    }

    public async Task<ScheduleDto?> AddScheduleAsync(CreateScheduleRequest request)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.PostAsJsonAsync("/schedules", request);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ScheduleDto>();
    }

    public async Task DeleteScheduleAsync(Guid scheduleId)
    {
        var client = httpClientFactory.CreateClient("BackendApi");
        var response = await client.DeleteAsync($"/schedules/{scheduleId}");

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException("Session expired");
        }

        response.EnsureSuccessStatusCode();
    }

    private record SchedulesResponse(List<ScheduleDto> Items);
}