namespace GrailJobApi.Modules.UserAccess.Presentation.Responses;

public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}

public sealed record AdminSiteAccessRequestResponse(
    string Id,
    string FirstName,
    string LastName,
    string CompanyName,
    string ContactEmail,
    string JobOffer,
    DateTime CreatedAtUtc,
    string NotificationStatus)
{
    public static AdminSiteAccessRequestResponse From(SiteAccessRequest request)
        => new(
            request.Id.ToString(),
            request.FirstName,
            request.LastName,
            request.CompanyName,
            request.ContactEmail,
            request.JobOffer,
            request.CreatedAtUtc,
            request.NotificationStatus.ToString());
}
