using Application.Features.Workout.CreateWorkoutTemplate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutTemplateController(CreateWorkoutTemplateHandler createWorkoutTemplateHandler) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateWorkoutTemplateRequest request)
        {
            var result = await createWorkoutTemplateHandler.ExecuteAsync(request);
            return Ok(result);
        }
    }
}