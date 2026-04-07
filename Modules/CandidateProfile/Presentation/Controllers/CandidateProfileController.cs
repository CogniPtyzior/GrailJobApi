using GrailJobApi.Modules.CandidateProfile.Application;
using GrailJobApi.Modules.CandidateProfile.Presentation.Responses;
using GrailJobApi.Shared.OpenApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GrailJobApi.Modules.CandidateProfile.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
[Produces("application/json", "application/problem+json")]
public sealed class CandidateProfileController(CandidateProfileService service) : ControllerBase
{
    [HttpGet("cv")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CvProfileResponseExample))]
    [ProducesResponseType<CvProfileResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CvProfileResponse>> GetCurrentCv(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        return Ok(await service.GetAsync(userId, cancellationToken));
    }

    [HttpPost("cv")]
    [Consumes("multipart/form-data")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CvProfileResponseExample))]
    [ProducesResponseType<CvProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<CvProfileResponse>> UploadCv([FromForm] UploadCandidateProfileRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null)
        {
            ModelState.AddModelError(nameof(request.File), "A PDF file is required.");
            return ValidationProblem(ModelState);
        }

        var extension = Path.GetExtension(request.File.FileName ?? string.Empty);
        if (!service.GetAllowedExtensions().Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(request.File), $"Only the following extensions are allowed: {string.Join(", ", service.GetAllowedExtensions())}.");
        }

        if (request.File.Length <= 0)
        {
            ModelState.AddModelError(nameof(request.File), "The uploaded file must not be empty.");
        }

        if (!string.IsNullOrWhiteSpace(request.File.ContentType) && !service.GetAllowedContentTypes().Contains(request.File.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(request.File), $"Only the following content types are allowed: {string.Join(", ", service.GetAllowedContentTypes())}.");
        }

        if (request.File.Length > service.GetMaxFileSizeBytes())
        {
            ModelState.AddModelError(nameof(request.File), $"The file size must not exceed {service.GetMaxFileSizeBytes()} bytes.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var response = await service.UploadAsync(GetUserId(), request.File, cancellationToken);
            return Ok(response);
        }
        catch (InvalidCandidateProfileDocumentException exception)
        {
            ModelState.AddModelError(nameof(request.File), exception.Message);
            return ValidationProblem(ModelState);
        }
        catch (CandidateProfileEnrichmentUnavailableException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Analyse du CV temporairement indisponible",
                detail: exception.Message);
        }
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public sealed class UploadCandidateProfileRequest
{
    public IFormFile? File { get; init; }
}