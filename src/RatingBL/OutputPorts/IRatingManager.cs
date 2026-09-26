namespace RatingBL.OutputPorts;

public interface IRatingManager
{
    public Task<int> GetRating(string username);
    public Task UpdateRating(string username, int stars);
}