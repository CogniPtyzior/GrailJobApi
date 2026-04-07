namespace GrailJobApi.Modules.UserAccess.Application;

public interface ISiteAccessRequestEmailSender
{
    Task SendAsync(SiteAccessRequest request, CancellationToken cancellationToken = default);
}