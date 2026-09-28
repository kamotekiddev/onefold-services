using Application.Features.Workout.Session.SaveSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers.Workout
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController(SaveSessionHandler saveSessionHandler) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveSessionAsync(SaveSessionRequest request)
        {
            var result = await saveSessionHandler.ExecuteAsync(request);
            return Ok(result);
        }
    }
}