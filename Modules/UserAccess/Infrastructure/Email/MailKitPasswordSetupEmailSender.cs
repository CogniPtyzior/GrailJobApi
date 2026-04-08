using GrailJobApi.Modules.UserAccess.Application;
using GrailJobApi.Shared.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Email;

public sealed class MailKitPasswordSetupEmailSender(
    IOptions<SiteAccessEmailOptions> options) : IPasswordSetupEmailSender
{
    private readonly SiteAccessEmailOptions _options = options.Value;

    public string PasswordSetupUrlBase => _options.PasswordSetupUrlBase;

    public async Task SendAsync(User user, string passwordSetupLink, CancellationToken cancellationToken = default)
    {
        ValidateOptions();

        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new InvalidOperationException("Impossible d'envoyer le mail de définition du mot de passe sans adresse email utilisateur.");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        message.To.Add(MailboxAddress.Parse(user.Email));
        message.Subject = "Accès GrailJob validé - définissez votre mot de passe";

        var displayName = user.GetDisplayName();

        var body = new BodyBuilder
        {
            TextBody =
$"""
Bonjour {displayName},

Votre accès GrailJob est maintenant actif.

Pour définir ou mettre à jour votre mot de passe, utilisez ce lien :
{passwordSetupLink}

Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer ce message.
""",
            HtmlBody = GrailJobEmailLayoutBuilder.BuildHtmlDocument(
                _options,
                "Définissez votre mot de passe",
                $"""
<p style="margin:0 0 16px 0;">Bonjour {GrailJobEmailLayoutBuilder.HtmlEncode(displayName)},</p>
<p style="margin:0 0 16px 0;">Votre accès GrailJob est maintenant actif.</p>
<p style="margin:0 0 20px 0;">Pour définir ou mettre à jour votre mot de passe en toute sécurité, utilisez le lien ci-dessous :</p>
<p style="margin:0 0 20px 0;">
  <a href="{GrailJobEmailLayoutBuilder.HtmlEncode(passwordSetupLink)}" target="_blank" rel="noopener noreferrer" style="display:inline-block;padding:12px 18px;background:#b7815f;color:#ffffff;text-decoration:none;border-radius:999px;font-weight:600;">
    Définir mon mot de passe
  </a>
</p>
<p style="margin:0;color:#6f625c;">Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer ce message.</p>
""")
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

        if (string.IsNullOrWhiteSpace(_options.PasswordSetupUrlBase))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:PasswordSetupUrlBase est obligatoire.");
        }

        GrailJobEmailLayoutBuilder.ValidateBrandingOptions(_options);
    }
}
