using Application.Features.Workout.Session.GetSessionById;
using Application.Features.Workout.Session.GetSessions;
using Application.Features.Workout.Session.SaveSession;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers.Workout
{
    [Route("api/workout/[controller]")]
    [ApiController]
    public class SessionController(
        SaveSessionHandler saveSessionHandler,
        GetSessionByIdHandler getSessionByIdHandler,
        GetSessionsHandler getSessionsHandler)
        : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await getSessionsHandler.ExecuteAsync();
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
        {
            var result = await getSessionByIdHandler.ExecuteAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveSessionAsync(SaveSessionRequest request)
        {
            var result = await saveSessionHandler.ExecuteAsync(request);
            return Ok(result);
        }
    }
}