namespace GatewayServer.Models;

public record LibraryPaginationResponse(int Page, int PageSize, int TotalElements, IEnumerable<LibraryResponse> Items);