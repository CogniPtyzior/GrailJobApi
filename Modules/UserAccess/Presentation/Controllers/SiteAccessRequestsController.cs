using GrailJobApi.Modules.UserAccess.Application;
using GrailJobApi.Modules.UserAccess.Presentation.Requests;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrailJobApi.Modules.UserAccess.Presentation.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/site-access-requests")]
[Produces("application/json", "application/problem+json")]
public sealed class SiteAccessRequestsController(SiteAccessRequestService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateSiteAccessRequestResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSiteAccessRequestResponse>> Create(
        [FromBody] CreateSiteAccessRequest request,
        CancellationToken cancellationToken)
    {
        var response = await service.CreateAsync(request, cancellationToken);
        return Accepted(response);
    }
}