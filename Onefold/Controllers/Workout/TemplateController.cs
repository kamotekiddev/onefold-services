using Application.Features.Workout.Template.CreateWorkoutTemplate;
using Application.Features.Workout.Template.GetWorkoutTemplateById;
using Application.Features.Workout.Template.UpdateWorkoutTemplate;
using Application.Features.Workout.WorkoutTemplateModule.CreateWorkoutTemplate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers.Workout
{
    [Route("api/workout/[controller]")]
    [ApiController]
    public class TemplateController(
        CreateWorkoutTemplateHandler createWorkoutTemplateHandler,
        UpdateWorkoutTemplateHandler updateWorkoutTemplateHandler,
        GetWorkoutTemplateByIdHandler getWorkoutTemplateByIdHandler) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateWorkoutTemplateRequest request)
        {
            var result = await createWorkoutTemplateHandler.ExecuteAsync(request);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await getWorkoutTemplateByIdHandler.HandleAsync(id);
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