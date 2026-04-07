using GrailJobApi.Modules.UserAccess.Presentation.Requests;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;

namespace GrailJobApi.Modules.UserAccess.Application;

public sealed class SiteAccessRequestService(
    ISiteAccessRequestRepository repository,
    ISiteAccessRequestEmailSender emailSender,
    ILogger<SiteAccessRequestService> logger)
{
    public async Task<CreateSiteAccessRequestResponse> CreateAsync(
        CreateSiteAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = SiteAccessRequest.Create(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.CompanyName.Trim(),
            request.ContactEmail.Trim(),
            request.JobOffer.Trim(),
            DateTime.UtcNow);

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        try
        {
            entity.MarkNotificationAttempt();
            await emailSender.SendAsync(entity, cancellationToken);
            entity.MarkNotificationSent(DateTime.UtcNow);
        }
        catch (Exception exception)
        {
            entity.MarkNotificationFailed(exception.Message);
            logger.LogError(
                exception,
                "Échec de l'envoi du mail pour la demande d'accès {SiteAccessRequestId}.",
                entity.Id);
        }

        await repository.SaveChangesAsync(cancellationToken);

        return new CreateSiteAccessRequestResponse(
            true,
            "Votre demande a bien été enregistrée. Nous reviendrons vers vous rapidement.");
    }
}