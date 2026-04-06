using GrailJobApi.Modules.CompanyWorkspace.Application;
using GrailJobApi.Modules.CompanyWorkspace.Domain;
using GrailJobApi.Modules.CompanyWorkspace.Presentation.Requests;
using GrailJobApi.Modules.CompanyWorkspace.Presentation.Responses;
using GrailJobApi.Shared.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GrailJobApi.Modules.CompanyWorkspace.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/companies")]
[Produces("application/json", "application/problem+json")]
public sealed class CompaniesController(CompanyWorkspaceService service) : ControllerBase
{
    /// <summary>Returns the list of saved companies.</summary>
    [HttpGet("saved")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CompanyListResponseExample))]
    [ProducesResponseType<CompanyListResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyListResponse>> GetSaved(CancellationToken cancellationToken)
        => Ok(await service.GetSavedAsync(GetUserId(), cancellationToken));

    /// <summary>Returns the list of excluded companies.</summary>
    [HttpGet("excluded")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CompanyListResponseExample))]
    [ProducesResponseType<CompanyListResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyListResponse>> GetExcluded(CancellationToken cancellationToken)
        => Ok(await service.GetExcludedAsync(GetUserId(), cancellationToken));

    /// <summary>Returns a single company detail. This endpoint supports both current search results and tracked companies.</summary>
    [HttpGet("{id:guid}")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CompanyResponseExample))]
    [ProducesResponseType<CompanyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var company = await service.GetByIdAsync(GetUserId(), id, cancellationToken);
        return company is null
            ? NotFound(new ProblemDetails { Title = "Company not found.", Status = StatusCodes.Status404NotFound })
            : Ok(company);
    }

    /// <summary>Transfers all current search results into the saved list and clears the current results.</summary>
    [HttpPost("save-current-results")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CompanyListResponseExample))]
    [ProducesResponseType<CompanyListResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyListResponse>> SaveCurrentResults(CancellationToken cancellationToken)
        => Ok(await service.SaveCurrentResultsAsync(GetUserId(), cancellationToken));

    /// <summary>Updates a company status. The endpoint accepts both current results and tracked companies.</summary>
    [HttpPatch("{id:guid}/status")]
    [SwaggerRequestExample(typeof(UpdateCompanyStatusRequest), typeof(UpdateCompanyStatusRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CompanyResponseExample))]
    [ProducesResponseType<CompanyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyResponse>> UpdateStatus(Guid id, [FromBody] UpdateCompanyStatusRequest request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<JobOpportunityStatus>(request.Status, true, out var status))
        {
            ModelState.AddModelError(nameof(request.Status), "Allowed values are Saved or Excluded.");
            return ValidationProblem(ModelState);
        }

        try
        {
            return Ok(await service.UpdateStatusAsync(GetUserId(), id, status, cancellationToken));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ProblemDetails { Title = "Company not found.", Status = StatusCodes.Status404NotFound });
        }
    }

    /// <summary>Updates a company comment. The endpoint accepts both current results and tracked companies.</summary>
    [HttpPatch("{id:guid}/comment")]
    [SwaggerRequestExample(typeof(UpdateCompanyCommentRequest), typeof(UpdateCompanyCommentRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CompanyResponseExample))]
    [ProducesResponseType<CompanyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyResponse>> UpdateComment(Guid id, [FromBody] UpdateCompanyCommentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await service.UpdateCommentAsync(GetUserId(), id, request.Comment, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(request.Comment), exception.Message);
            return ValidationProblem(ModelState);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ProblemDetails { Title = "Company not found.", Status = StatusCodes.Status404NotFound });
        }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
