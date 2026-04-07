using GrailJobApi.Modules.UserAccess.Presentation.Requests;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrailJobApi.Modules.UserAccess.Presentation.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/site-access-requests")]
[Produces("application/json", "application/problem+json")]
public sealed class SiteAccessRequestsController(ILogger<SiteAccessRequestsController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CreateSiteAccessRequestResponse>(StatusCodes.Status202Accepted)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<CreateSiteAccessRequestResponse> Create([FromBody] CreateSiteAccessRequest request)
    {
        Validate(request);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        logger.LogInformation(
            "Site access request received for {CompanyName} from {FirstName} {LastName} <{ContactEmail}>.",
            request.CompanyName.Trim(),
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.ContactEmail.Trim());

        return Accepted(new CreateSiteAccessRequestResponse(
            true,
            "Votre demande d'accès a bien été transmise. Nous reviendrons vers vous après vérification."));
    }

    private void Validate(CreateSiteAccessRequest request)
    {
        AddIfBlank(nameof(request.FirstName), request.FirstName, "Le prénom est obligatoire.");
        AddIfBlank(nameof(request.LastName), request.LastName, "Le nom est obligatoire.");
        AddIfBlank(nameof(request.CompanyName), request.CompanyName, "L'entreprise concernée est obligatoire.");
        AddIfBlank(nameof(request.ContactEmail), request.ContactEmail, "L'email de contact est obligatoire.");
        AddIfBlank(nameof(request.JobOffer), request.JobOffer, "L'offre d'emploi est obligatoire.");
    }

    private void AddIfBlank(string key, string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ModelState.AddModelError(key, message);
        }
    }
}