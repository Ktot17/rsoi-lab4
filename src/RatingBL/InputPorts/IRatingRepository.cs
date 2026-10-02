namespace RatingBL.InputPorts;

public interface IRatingRepository
{
    public Task<int> GetRating(string username);
    public Task UpdateRating(string username, int stars);
}