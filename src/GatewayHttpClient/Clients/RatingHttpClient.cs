using System.Net.Http.Json;
using System.Text.Json;
using GatewayBL.Enums;
using GatewayBL.Exceptions;
using GatewayBL.InputPorts;
using GatewayHttp.Models;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace GatewayHttp.Clients;

public class RatingHttpClient(HttpClient client) : IRatingHttpClient
{
    private const ServiceName ThisServiceName = ServiceName.Rating;
    
    public async Task<int> GetUserRatingAsync(string username)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("api/v1/rating", UriKind.Relative));
        request.Headers.Add("X-User-Name", username);

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw new ServiceIsUnavailableException(ThisServiceName);
        }
        catch (BrokenCircuitException)
        {
            throw new ServiceIsUnavailableException(ThisServiceName);
        }
        catch (TimeoutRejectedException)
        {
            throw new ServiceIsUnavailableException(ThisServiceName);
        }
        
        using (response)
        {
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<UserRatingResponse>() ?? throw new JsonException()).Stars;
        }
    }

    public async Task UpdateUserRatingAsync(string username, int stars)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, new Uri("api/v1/rating?stars=" + stars, UriKind.Relative));
        request.Headers.Add("X-User-Name", username);

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw new ServiceIsUnavailableException(ThisServiceName);
        }
        catch (BrokenCircuitException)
        {
            throw new ServiceIsUnavailableException(ThisServiceName);
        }
        catch (TimeoutRejectedException)
        {
            throw new ServiceIsUnavailableException(ThisServiceName);
        }
        
        using (response)
            response.EnsureSuccessStatusCode();
    }
}
