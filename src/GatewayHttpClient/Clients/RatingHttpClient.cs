using System.Net.Http.Json;
using System.Text.Json;
using GatewayBL.InputPorts;
using GatewayHttp.Models;

namespace GatewayHttp.Clients;

public class RatingHttpClient(HttpClient client) : IRatingHttpClient
{
    public async Task<int> GetUserRatingAsync(string username)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/rating");
        request.Headers.Add("X-User-Name", username);

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<UserRatingResponse>() ?? throw new JsonException()).Stars;
    }

    public async Task UpdateUserRatingAsync(string username, int stars)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, "api/v1/rating?stars=" + stars);
        request.Headers.Add("X-User-Name", username);
        
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}