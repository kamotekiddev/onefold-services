using Application.Features.Authentication.EmailSignUp;
using Application.Features.Authentication.SignInWithEmail;
using Microsoft.AspNetCore.Mvc;

namespace Onefold.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(EmailSignUp emailSignUp, SignInWithEmailHandler signInWithEmailHandler) : ControllerBase
    {
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpWithEmail([FromBody] EmailSignUpRequest request)
        {
            var result = await emailSignUp.ExecuteAsync(request);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignInWithEmail([FromBody] SignInWithEmailRequest request)
        {
            var result = await signInWithEmailHandler.ExecuteAsync(request);
            return Ok(result);
        }
    }
}