using GatewayBL.Enums;
using GatewayBL.Models;

namespace GatewayBL.OutputPorts;

public interface IGatewayManager
{
    public Task<(int, IEnumerable<Library>)> GetLibrariesAsync(string city, int page, int size);
    public Task<(int, IEnumerable<Book>)> GetBooksAsync(Guid libraryUid, int page, int size, bool showAll);
    public Task<IEnumerable<FullInfo>> GetReservationsAsync(string username);
    public Task<FullInfo> TakeBookAsync(string username, Guid libraryUid, Guid bookUid, DateTime tillDate);
    public Task ReturnBookAsync(string username, Guid reservationUid, Condition condition, DateTime returnDate);
    public Task<int> GetUserRatingAsync(string username);
}