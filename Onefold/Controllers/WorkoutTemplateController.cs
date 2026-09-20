using Application.Features.Workout.AddWorkoutExerciseToTemplate;
using Application.Features.Workout.CreateWorkoutTemplate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutTemplateController(
        CreateWorkoutTemplateHandler createWorkoutTemplateHandler,
        AddWorkoutExerciseToTemplateHandler addWorkoutExerciseToTemplateHandler) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateWorkoutTemplateRequest request)
        {
            var result = await createWorkoutTemplateHandler.ExecuteAsync(request);
            return Ok(result);
        }

        [HttpPut("{id:guid}/exercises")]
        [Authorize]
        public async Task<IActionResult> AddExerciseToTemplate([FromRoute] Guid templateId,
            [FromBody] AddWorkoutExerciseToTemplateRequest request)
        {
            var result = await addWorkoutExerciseToTemplateHandler.ExecuteAsync(templateId, request);
            return Ok(result);
        }
    }
}