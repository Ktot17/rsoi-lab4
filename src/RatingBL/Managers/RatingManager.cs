using RatingBL.InputPorts;
using RatingBL.OutputPorts;

namespace RatingBL.Managers;

public class RatingManager(IRatingRepository ratingRepository) : IRatingManager
{
    public async Task<int> GetRating(string username) => await ratingRepository.GetRating(username);
    
    public async Task UpdateRating(string username, int stars) => await ratingRepository.UpdateRating(username, stars);
}