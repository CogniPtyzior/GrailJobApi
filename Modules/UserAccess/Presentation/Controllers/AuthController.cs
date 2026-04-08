using GrailJobApi.Modules.UserAccess.Application;
using GrailJobApi.Modules.UserAccess.Presentation.Requests;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;
using GrailJobApi.Shared.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GrailJobApi.Modules.UserAccess.Presentation.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json", "application/problem+json")]
public sealed class AuthController(AuthService authService, PasswordSetupService passwordSetupService) : ControllerBase
{
    /// <summary>Authenticates a user and creates the authentication cookie.</summary>
    [HttpPost("login")]
    [SwaggerRequestExample(typeof(LoginRequest), typeof(LoginRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LoginResponseExample))]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        return response is null
            ? Unauthorized(new ProblemDetails { Title = "Invalid credentials.", Status = StatusCodes.Status401Unauthorized })
            : Ok(response);
    }

    /// <summary>Logs out the current user.</summary>
    [AllowAnonymous]
    [HttpPost("logout")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LogoutResponseExample))]
    [ProducesResponseType<LogoutResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<LogoutResponse>> Logout()
    {
        await authService.LogoutAsync(HttpContext);
        return Ok(new LogoutResponse(true));
    }

    /// <summary>Returns the currently authenticated user.</summary>
    [Authorize]
    [HttpGet("me")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LoginResponseExample))]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Me()
    {
        var response = await authService.GetCurrentAsync(User);
        return response is null
            ? Unauthorized(new ProblemDetails { Title = "The current user is not authenticated.", Status = StatusCodes.Status401Unauthorized })
            : Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("password-setup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> CompletePasswordSetup(
        [FromBody] CompletePasswordSetupRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await passwordSetupService.CompleteAsync(
                request.UserId,
                request.Token,
                request.Password,
                request.ConfirmPassword,
                cancellationToken);

            return Ok(new { success = true });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message, Status = StatusCodes.Status404NotFound });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new ProblemDetails { Title = exception.Message, Status = StatusCodes.Status400BadRequest });
        }
    }
}
