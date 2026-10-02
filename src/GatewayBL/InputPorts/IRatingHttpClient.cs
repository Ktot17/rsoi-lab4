namespace GatewayBL.InputPorts;

public interface IRatingHttpClient
{
    public Task<int> GetUserRatingAsync(string username);
    public Task UpdateUserRatingAsync(string username, int stars);
}