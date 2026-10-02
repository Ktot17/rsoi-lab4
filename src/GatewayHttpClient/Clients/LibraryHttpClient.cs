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

public class LibraryHttpClient(HttpClient client) : ILibraryHttpClient
{
    private const ServiceName ThisServiceName = ServiceName.Library;
    
    public async Task<(int, IEnumerable<Library>)> GetLibraries(string city, int page, int size)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(new Uri(
                $"api/v1/libraries?city={Uri.EscapeDataString(city)}&page={page}&size={size}",
                UriKind.Relative));
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

            var pagination = await response.Content.ReadFromJsonAsync<LibraryPaginationResponse>() ??
                             throw new JsonException();
            return (pagination.TotalElements, pagination.Items.Select(l => l.ToBlModel()));
        }
    }

    public async Task<(int, IEnumerable<Book>)> GetBooks(Guid libraryUid, int page, int size, bool showAll)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(new Uri(
                $"api/v1/libraries/{libraryUid}/books?page={page}&size={size}&showAll={showAll}",
                UriKind.Relative));
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

            var pagination = await response.Content.ReadFromJsonAsync<LibraryBookPaginationResponse>() ??
                             throw new JsonException();
            return (pagination.TotalElements, pagination.Items.Select(l => l.ToBlModel()));
        }
    }

    public async Task<(IEnumerable<Library>, IEnumerable<Book>)> GetLibrariesAndBooksAsync(
        IEnumerable<Guid> libraryUids, IEnumerable<Guid> bookUids)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync("api/v1/libraries/books",
                new LibrariesAndBooksRequest(libraryUids, bookUids));
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

            var librariesAndBooks = await response.Content.ReadFromJsonAsync<LibrariesAndBooksResponse>() ??
                                    throw new JsonException();
            return (librariesAndBooks.Libraries, librariesAndBooks.Books);
        }
    }

    public async Task ChangeAvailableCountAsync(Guid libraryUid, Guid bookUid, int count)
    {
        HttpResponseMessage response;
        try
        {
            response = await client.PatchAsync(new Uri($"api/v1/libraries/{libraryUid}/books/{bookUid}?count={count}",
                UriKind.Relative), null);
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
