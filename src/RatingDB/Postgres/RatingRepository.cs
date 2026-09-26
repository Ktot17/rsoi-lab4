using Microsoft.EntityFrameworkCore;
using RatingBL.Exceptions;
using RatingBL.InputPorts;
using RatingDB.Models;

namespace RatingDB.Postgres;

public class RatingRepository(PostgresDbContext context) : IRatingRepository
{
    public async Task<int> GetRating(string username) =>
        (await context.Ratings.FirstOrDefaultAsync(r => r.Username == username) ?? 
         throw new EntityNotFoundException(typeof(RatingDb))).Stars;

    public async Task UpdateRating(string username, int stars)
    {
        var rating = await context.Ratings.FirstOrDefaultAsync(r => r.Username == username) ?? 
                     throw new EntityNotFoundException(typeof(RatingDb));
        rating.Stars += stars;
        
        context.Ratings.Update(rating);
        await context.SaveChangesAsync();
    }
}