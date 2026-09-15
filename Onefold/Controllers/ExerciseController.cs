using Application.Features.Workout.CreateExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExerciseController(CreateExerciseHandler createExerciseHandler) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseRequest request)
        {
            var exercise = await createExerciseHandler.ExecuteAsync(request);
            return Ok(exercise);
        }
    }
}