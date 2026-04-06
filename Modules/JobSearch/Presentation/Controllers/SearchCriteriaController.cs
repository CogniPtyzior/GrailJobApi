using GrailJobApi.Modules.JobSearch.Application;
using GrailJobApi.Modules.JobSearch.Presentation.Requests;
using GrailJobApi.Modules.JobSearch.Presentation.Responses;
using GrailJobApi.Shared.Configuration;
using GrailJobApi.Shared.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Filters;

namespace GrailJobApi.Modules.JobSearch.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/search-criteria")]
[Produces("application/json", "application/problem+json")]
public sealed class SearchCriteriaController(SearchService searchService, IOptions<SearchOptions> options) : ControllerBase
{
    private readonly SearchOptions _options = options.Value;

    /// <summary>Returns the saved search criteria for the authenticated user.</summary>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchCriteriaResponseExample))]
    [ProducesResponseType<SearchCriteriaResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchCriteriaResponse>> Get(CancellationToken cancellationToken)
        => Ok(await searchService.GetCriteriaAsync(GetUserId(), cancellationToken));

    /// <summary>Replaces the current list of search criteria.</summary>
    [HttpPut]
    [SwaggerRequestExample(typeof(SearchCriteriaRequest), typeof(SearchCriteriaRequestExample))]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchCriteriaResponseExample))]
    [ProducesResponseType<SearchCriteriaResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SearchCriteriaResponse>> Put([FromBody] SearchCriteriaRequest request, CancellationToken cancellationToken)
    {
        var criteria = request.Criteria ?? [];
        ValidateCriteria(criteria);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            return Ok(await searchService.SaveCriteriaAsync(GetUserId(), criteria, cancellationToken));
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(SearchCriteriaRequest.Criteria), exception.Message);
            return ValidationProblem(ModelState);
        }
    }

    private void ValidateCriteria(IReadOnlyList<string>? criteria)
    {
        if (criteria is null)
        {
            ModelState.AddModelError(nameof(SearchCriteriaRequest.Criteria), "The criteria collection is required.");
            return;
        }

        var normalized = criteria
            .Select(x => x?.Trim() ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => string.Join(' ', x.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
            .ToArray();

        foreach (var criterion in normalized)
        {
            if (criterion.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > _options.MaxCriteriaWordCount)
            {
                ModelState.AddModelError(nameof(SearchCriteriaRequest.Criteria), $"Each criterion must contain at most {_options.MaxCriteriaWordCount} words.");
            }
        }

        if (normalized.Length != normalized.Distinct(StringComparer.OrdinalIgnoreCase).Count())
        {
            ModelState.AddModelError(nameof(SearchCriteriaRequest.Criteria), "Duplicate criteria are not allowed.");
        }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
