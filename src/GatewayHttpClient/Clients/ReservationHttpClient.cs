using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GatewayBL.Exceptions;
using GatewayBL.InputPorts;
using GatewayBL.Models;
using GatewayHttp.Models;

namespace GatewayHttp.Clients;

public class ReservationHttpClient(HttpClient client) : IReservationHttpClient
{
    public async Task<IEnumerable<Reservation>> GetReservationsAsync(string username)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/reservations");
        request.Headers.Add("X-User-Name", username);

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var reservations = 
            await response.Content.ReadFromJsonAsync<IEnumerable<ReservationResponse>>() ?? throw new JsonException();
        return reservations.Select(r => r.ToBlModel());
    }

    public async Task<Reservation> TakeBookAsync(string username, Guid libraryUid, Guid bookUid, DateTime tillDate)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/reservations");
        request.Headers.Add("X-User-Name", username);
        request.Content = JsonContent.Create(new TakeBookRequest(libraryUid, bookUid, tillDate));

        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var reservation = await response.Content.ReadFromJsonAsync<ReservationResponse>() ?? throw new JsonException();
        return reservation.ToBlModel();
    }

    public async Task<Reservation> ReturnBookAsync(Guid reservationUid, DateTime returnDate)
    {
        using var response = await client.PostAsJsonAsync($"api/v1/reservations/{reservationUid}/return", new ReturnBookRequest(returnDate));

        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new EntityNotFoundException();
        
        response.EnsureSuccessStatusCode();
        
        var reservation = await response.Content.ReadFromJsonAsync<ReservationResponse>() ?? throw new JsonException();
        return reservation.ToBlModel();
    }

    public async Task<int> GetReservationCountAsync(string username)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/v1/reservations/count");
        request.Headers.Add("X-User-Name", username);
        
        using var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<int>();
    }
}