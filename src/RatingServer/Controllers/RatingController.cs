using Microsoft.AspNetCore.Mvc;
using RatingBL.OutputPorts;
using RatingServer.Models;

namespace RatingServer.Controllers;

[ApiController]
[Route("api/v1/rating/")]
public class RatingController(IRatingManager ratingManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserRatingResponse>> GetUserRating(
        [FromHeader(Name = "X-User-Name")] string username) => 
        Ok(new UserRatingResponse(await ratingManager.GetRating(username)));

    [HttpPatch]
    public async Task<ActionResult> UpdateUserRating(
        [FromHeader(Name = "X-User-Name")] string username, int stars)
    {
        await ratingManager.UpdateRating(username, stars);
        return Ok();
    }
}