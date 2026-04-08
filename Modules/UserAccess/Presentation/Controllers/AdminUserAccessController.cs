using GrailJobApi.Modules.UserAccess.Application;
using GrailJobApi.Modules.UserAccess.Presentation.Requests;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GrailJobApi.Modules.UserAccess.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/admin")]
[Produces("application/json", "application/problem+json")]
public sealed class AdminUserAccessController(
    AdminUserAccessService service,
    UserManager<User> userManager) : ControllerBase
{
    private const string AdminEmail = "admin@grailjob.local";

    [HttpGet("users")]
    [ProducesResponseType<PagedResponse<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] string? status = null,
        [FromQuery] string? firstName = null,
        [FromQuery] string? lastName = null,
        [FromQuery] string? email = null,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        var response = await service.GetUsersAsync(page, pageSize, status, firstName, lastName, email, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("users/{id:guid}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> UpdateUser(
        Guid id,
        [FromBody] UpdateAdminUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        try
        {
            var response = await service.UpdateUserIdentityAsync(id, request.FirstName, request.LastName, cancellationToken);
            return Ok(response);
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

    [HttpPost("users/{id:guid}/password-reset-link")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> SendPasswordResetLink(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        try
        {
            var response = await service.SendPasswordResetLinkAsync(id, cancellationToken);
            return Ok(response);
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

    [HttpPatch("users/{id:guid}/status")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> UpdateUserStatus(
        Guid id,
        [FromBody] UpdateUserStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var currentUser = await userManager.GetUserAsync(User);
        var adminCheck = EnsureAdmin(currentUser);
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        try
        {
            var response = await service.SetUserStatusAsync(currentUser!.Id, id, request.IsActive, cancellationToken);
            return Ok(response);
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

    [HttpPut("users/{id:guid}/password")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> SetUserPassword(
        Guid id,
        [FromBody] SetUserPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        try
        {
            var response = await service.SetUserPasswordAsync(id, request.Password, cancellationToken);
            return Ok(response);
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

    [HttpGet("site-access-requests")]
    [ProducesResponseType<PagedResponse<AdminSiteAccessRequestResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PagedResponse<AdminSiteAccessRequestResponse>>> GetSiteAccessRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        var response = await service.GetSiteAccessRequestsAsync(page, pageSize, cancellationToken);
        return Ok(response);
    }

    [HttpPatch("site-access-requests/{id:guid}")]
    [ProducesResponseType<AdminSiteAccessRequestResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminSiteAccessRequestResponse>> UpdateSiteAccessRequest(
        Guid id,
        [FromBody] UpdateAdminSiteAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        try
        {
            var response = await service.UpdateSiteAccessRequestIdentityAsync(id, request.FirstName, request.LastName, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message, Status = StatusCodes.Status404NotFound });
        }
    }

    [HttpPost("site-access-requests/{id:guid}/accept")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> AcceptSiteAccessRequest(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        try
        {
            var response = await service.AcceptSiteAccessRequestAsync(id, cancellationToken);
            return Ok(response);
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

    [HttpDelete("site-access-requests/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> DeleteSiteAccessRequest(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var adminCheck = await EnsureAdminAsync();
        if (adminCheck is not null)
        {
            return adminCheck;
        }

        try
        {
            await service.DeleteSiteAccessRequestAsync(id, cancellationToken);
            return Ok(new { Success = true });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new ProblemDetails { Title = exception.Message, Status = StatusCodes.Status404NotFound });
        }
    }

    private async Task<ActionResult?> EnsureAdminAsync()
    {
        var currentUser = await userManager.GetUserAsync(User);
        return EnsureAdmin(currentUser);
    }

    private ActionResult? EnsureAdmin(User? currentUser)
    {
        if (currentUser is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Utilisateur non authentifié.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        if (!string.Equals(currentUser.Email, AdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Title = "Accès réservé à l'administration.",
                Status = StatusCodes.Status403Forbidden
            });
        }

        return null;
    }
}
