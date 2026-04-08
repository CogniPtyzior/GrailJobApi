namespace GrailJobApi.Modules.UserAccess.Application;

public interface IPasswordSetupEmailSender
{
    string PasswordSetupUrlBase { get; }

    Task SendAsync(User user, string passwordSetupLink, CancellationToken cancellationToken = default);
}
