using GrailJobApi.Modules.UserAccess.Application;
using GrailJobApi.Shared.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Email;

public sealed class MailKitSiteAccessRequestEmailSender(
    IOptions<SiteAccessEmailOptions> options) : ISiteAccessRequestEmailSender
{
    private readonly SiteAccessEmailOptions _options = options.Value;

    public async Task SendAsync(SiteAccessRequest request, CancellationToken cancellationToken = default)
    {
        ValidateOptions();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        message.To.Add(MailboxAddress.Parse(_options.RecipientEmail));
        message.Subject = $"Nouvelle demande d'accès - {request.CompanyName} - {request.FirstName} {request.LastName}";

        var body = new BodyBuilder
        {
            TextBody =
$"""
Une nouvelle demande d'accès a été reçue.

Prénom : {request.FirstName}
Nom : {request.LastName}
Entreprise concernée : {request.CompanyName}
Email de contact : {request.ContactEmail}
Date UTC : {request.CreatedAtUtc:O}

Offre d'emploi :
{request.JobOffer}
""",
            HtmlBody =
$"""
<html>
  <body style="font-family:Segoe UI,Arial,sans-serif;color:#2f2928;">
    <h2>Nouvelle demande d'accès</h2>
    <p><strong>Prénom :</strong> {HtmlEncode(request.FirstName)}</p>
    <p><strong>Nom :</strong> {HtmlEncode(request.LastName)}</p>
    <p><strong>Entreprise concernée :</strong> {HtmlEncode(request.CompanyName)}</p>
    <p><strong>Email de contact :</strong> {HtmlEncode(request.ContactEmail)}</p>
    <p><strong>Date UTC :</strong> {request.CreatedAtUtc:O}</p>
    <p><strong>Offre d'emploi :</strong></p>
    <pre style="white-space:pre-wrap;font-family:inherit;">{HtmlEncode(request.JobOffer)}</pre>
  </body>
</html>
"""
        };

        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();

        var socketOptions = _options.UseStartTls
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.Auto;

        await client.ConnectAsync(_options.SmtpHost, _options.SmtpPort, socketOptions, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.SmtpUsername))
        {
            await client.AuthenticateAsync(_options.SmtpUsername, ResolvePassword(), cancellationToken);
        }

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    private string ResolvePassword()
    {
        if (!string.IsNullOrWhiteSpace(_options.SmtpPassword))
        {
            return _options.SmtpPassword;
        }

        if (!string.IsNullOrWhiteSpace(_options.SmtpPasswordFile))
        {
            var path = Path.IsPathRooted(_options.SmtpPasswordFile)
                ? _options.SmtpPasswordFile
                : Path.Combine(Directory.GetCurrentDirectory(), _options.SmtpPasswordFile);

            if (File.Exists(path))
            {
                return File.ReadAllText(path).Trim();
            }
        }

        return string.Empty;
    }

    private void ValidateOptions()
    {
        if (string.IsNullOrWhiteSpace(_options.RecipientEmail))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:RecipientEmail est obligatoire.");
        }

        if (string.IsNullOrWhiteSpace(_options.FromEmail))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:FromEmail est obligatoire.");
        }

        if (string.IsNullOrWhiteSpace(_options.SmtpHost))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:SmtpHost est obligatoire.");
        }

        if (_options.SmtpPort <= 0)
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:SmtpPort est invalide.");
        }
    }

    private static string HtmlEncode(string value)
        => System.Net.WebUtility.HtmlEncode(value);
}