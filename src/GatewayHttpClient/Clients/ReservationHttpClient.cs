using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GatewayBL.Enums;
using GatewayBL.Exceptions;
using GatewayBL.InputPorts;
using GatewayBL.Models;
using GatewayHttp.Models;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace GatewayHttp.Clients;

public class ReservationHttpClient(HttpClient client) : IReservationHttpClient
{
    private const ServiceName ThisServiceName = ServiceName.Reservation;
    
    public async Task<IEnumerable<Reservation>> GetReservationsAsync(string username)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("api/v1/reservations", UriKind.Relative));
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

            var reservations =
                await response.Content.ReadFromJsonAsync<IEnumerable<ReservationResponse>>() ??
                throw new JsonException();
            return reservations.Select(r => r.ToBlModel());
        }
    }

    public async Task<Reservation> TakeBookAsync(string username, Guid libraryUid, Guid bookUid, DateTime tillDate)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("api/v1/reservations", UriKind.Relative));
        request.Headers.Add("X-User-Name", username);
        request.Content = JsonContent.Create(new TakeBookRequest(libraryUid, bookUid, tillDate));

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

            var reservation = await response.Content.ReadFromJsonAsync<ReservationResponse>() ??
                              throw new JsonException();
            return reservation.ToBlModel();
        }
    }

    public async Task<Reservation> ReturnBookAsync(Guid reservationUid, DateTime returnDate)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync($"api/v1/reservations/{reservationUid}/return",
                new ReturnBookRequest(returnDate));
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
            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new EntityNotFoundException();

            response.EnsureSuccessStatusCode();

            var reservation = await response.Content.ReadFromJsonAsync<ReservationResponse>() ??
                              throw new JsonException();
            return reservation.ToBlModel();
        }
    }

    public async Task<int> GetReservationCountAsync(string username)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("api/v1/reservations/count", UriKind.Relative));
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

            return await response.Content.ReadFromJsonAsync<int>();
        }
    }

    public async Task RevertTakeBookAsync(Guid reservationUid)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.DeleteAsync(new Uri($"api/v1/reservations/{reservationUid}", UriKind.Relative));
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
