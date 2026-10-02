namespace GatewayServer.Models;

public record LibraryBookPaginationResponse(int Page, int PageSize, int TotalElements, IEnumerable<LibraryBookResponse> Items);