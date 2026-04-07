namespace GrailJobApi.Modules.UserAccess.Domain;

public sealed class SiteAccessRequest
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string ContactEmail { get; private set; } = string.Empty;
    public string JobOffer { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public SiteAccessRequestNotificationStatus NotificationStatus { get; private set; }
    public DateTime? NotificationSentAtUtc { get; private set; }
    public string? NotificationLastError { get; private set; }
    public int NotificationAttemptCount { get; private set; }

    private SiteAccessRequest()
    {
    }

    public static SiteAccessRequest Create(
        string firstName,
        string lastName,
        string companyName,
        string contactEmail,
        string jobOffer,
        DateTime nowUtc)
    {
        return new SiteAccessRequest
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            CompanyName = companyName,
            ContactEmail = contactEmail,
            JobOffer = jobOffer,
            CreatedAtUtc = nowUtc,
            NotificationStatus = SiteAccessRequestNotificationStatus.Pending,
            NotificationAttemptCount = 0
        };
    }

    public void MarkNotificationAttempt()
    {
        NotificationAttemptCount++;
    }

    public void MarkNotificationSent(DateTime sentAtUtc)
    {
        NotificationStatus = SiteAccessRequestNotificationStatus.Sent;
        NotificationSentAtUtc = sentAtUtc;
        NotificationLastError = null;
    }

    public void MarkNotificationFailed(string error)
    {
        NotificationStatus = SiteAccessRequestNotificationStatus.Failed;
        NotificationLastError = error;
    }
}

public enum SiteAccessRequestNotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}