using Application.Features.Workout.Exercise.CreateExercise;
using Application.Features.Workout.ExerciseModule.GetExercise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers.Workout
{
    [Route("api/workout/[controller]")]
    [ApiController]
    public class ExerciseController(CreateExerciseHandler createExerciseHandler, GetExerciseHandler getExerciseHandler)
        : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseRequest request)
        {
            var exercise = await createExerciseHandler.ExecuteAsync(request);
            return Ok(exercise);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetExerciseById([FromRoute] Guid id)
        {
            var exercise = await getExerciseHandler.ExecuteAsync(id);
            return Ok(exercise);
        }
    }
}