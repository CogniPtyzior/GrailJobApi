using GrailJobApi.Modules.JobSearch.Application;
using GrailJobApi.Modules.JobSearch.Presentation.Requests;
using GrailJobApi.Modules.JobSearch.Presentation.Responses;
using GrailJobApi.Shared.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GrailJobApi.Modules.JobSearch.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/searches")]
[Produces("application/json", "application/problem+json")]
public sealed class SearchesController(SearchService searchService) : ControllerBase
{
    /// <summary>Executes a new search using the current candidate profile and supplied criteria.</summary>
    [HttpPost]
    [SwaggerRequestExample(typeof(SearchCriteriaRequest), typeof(SearchCriteriaRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchResponseExample))]
    [ProducesResponseType<SearchResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchResponse>> Execute([FromBody] SearchCriteriaRequest request, CancellationToken cancellationToken)
    {
        if (request.Criteria is null)
        {
            ModelState.AddModelError(nameof(SearchCriteriaRequest.Criteria), "The criteria collection is required.");
            return ValidationProblem(ModelState);
        }

        try
        {
            return Ok(await searchService.ExecuteSearchAsync(GetUserId(), request.Criteria, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(SearchCriteriaRequest.Criteria), exception.Message);
            return ValidationProblem(ModelState);
        }
    }

    /// <summary>Returns the current persisted search results.</summary>
    [HttpGet("current")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchResponseExample))]
    [ProducesResponseType<SearchResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchResponse>> GetCurrent(CancellationToken cancellationToken)
        => Ok(await searchService.GetCurrentSearchAsync(GetUserId(), cancellationToken));

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
