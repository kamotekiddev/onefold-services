using Application.Features.Authentication.EmailSignUp;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(EmailSignUp emailSignUp) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpWithEmail([FromBody] EmailSignUpRequest request)
        {
            var result = await emailSignUp.ExecuteAsync(request);
            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}