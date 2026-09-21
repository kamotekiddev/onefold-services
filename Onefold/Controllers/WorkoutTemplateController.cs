using Application.Features.Workout.WorkoutTemplateModule.CreateWorkoutTemplate;
using Application.Features.Workout.WorkoutTemplateModule.UpdateWorkoutTemplate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkoutTemplateController(
        CreateWorkoutTemplateHandler createWorkoutTemplateHandler,
        UpdateWorkoutTemplateHandler updateWorkoutTemplateHandler) : ControllerBase
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
        public async Task<IActionResult> Update([FromRoute] Guid templateId,
            [FromBody] UpdateWorkoutTemplateRequest request)
        {
            var result = await updateWorkoutTemplateHandler.ExecuteAsync(templateId, request);
            return Ok(result);
        }
    }
}