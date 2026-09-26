using System.Net.Http.Json;
using System.Text.Json;
using GatewayBL.InputPorts;
using GatewayBL.Models;
using GatewayHttp.Models;

namespace GatewayHttp.Clients;

public class LibraryHttpClient(HttpClient client) : ILibraryHttpClient
{
    public async Task<(int, IEnumerable<Library>)> GetLibraries(string city, int page, int size)
    {
        using var response = await client.GetAsync($"api/v1/libraries?city={city}&page={page}&size={size}");
        response.EnsureSuccessStatusCode();

        var pagination = await response.Content.ReadFromJsonAsync<LibraryPaginationResponse>() ?? throw new JsonException();
        return (pagination.TotalElements, pagination.Items.Select(l => l.ToBlModel()));
    }

    public async Task<(int, IEnumerable<Book>)> GetBooks(Guid libraryUid, int page, int size, bool showAll)
    {
        using var response = await client.GetAsync($"api/v1/libraries/{libraryUid}/books?page={page}&size={size}&showAll={showAll}");
        response.EnsureSuccessStatusCode();

        var pagination = await response.Content.ReadFromJsonAsync<LibraryBookPaginationResponse>() ?? throw new JsonException();
        return (pagination.TotalElements, pagination.Items.Select(l => l.ToBlModel()));
    }

    public async Task<(IEnumerable<Library>, IEnumerable<Book>)> GetLibrariesAndBooksAsync(
        IEnumerable<Guid> libraryUids, IEnumerable<Guid> bookUids)
    {
        using var response = await client.PostAsJsonAsync("api/v1/libraries/books", new LibrariesAndBooksRequest(libraryUids, bookUids));
        response.EnsureSuccessStatusCode();

        var librariesAndBooks = await response.Content.ReadFromJsonAsync<LibrariesAndBooksResponse>() ?? throw new JsonException();
        return (librariesAndBooks.Libraries, librariesAndBooks.Books);
    }

    public async Task ChangeAvailableCountAsync(Guid libraryUid, Guid bookUid, int count)
    {
        using var response = await client.PatchAsync($"api/v1/libraries/{libraryUid}/books/{bookUid}?count={count}", null);
        response.EnsureSuccessStatusCode();
    }
}