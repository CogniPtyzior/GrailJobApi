using GrailJobApi.Shared.Configuration;

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Email;

internal static class GrailJobEmailLayoutBuilder
{
    public static string BuildHtmlDocument(
        SiteAccessEmailOptions options,
        string title,
        string contentHtml)
    {
        ValidateBrandingOptions(options);

        return $"""
<html>
  <body style="margin:0;padding:24px;background-color:#f7f1ec;font-family:Segoe UI,Arial,sans-serif;color:#2f2928;">
    <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%" style="max-width:720px;margin:0 auto;background:#ffffff;border:1px solid #eaded6;border-radius:18px;overflow:hidden;">
      <tr>
        <td style="padding:20px 28px;border-bottom:1px solid #eaded6;">
          <table role="presentation" cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
              <td align="left" valign="middle">
                <a href="{HtmlEncode(options.WebsiteUrl)}" target="_blank" rel="noopener noreferrer">
                  <img src="{HtmlEncode(options.BrandLogoImageUrl)}" alt="GrailJob" style="display:block;max-width:180px;height:40px;border:0;" />
                </a>
              </td>
              <td align="right" valign="middle">
                <a href="{HtmlEncode(options.LinkedInUrl)}" target="_blank" rel="noopener noreferrer">
                  <img src="{HtmlEncode(options.ProfileImageUrl)}" alt="Profil Damien Farina" style="display:block;width:56px;height:56px;border-radius:28px;object-fit:cover;border:0;" />
                </a>
              </td>
            </tr>
          </table>
        </td>
      </tr>
      <tr>
        <td style="padding:28px;">
          <h2 style="margin:0 0 20px 0;font-size:24px;line-height:1.3;color:#2f2928;">{HtmlEncode(title)}</h2>
          {contentHtml}
        </td>
      </tr>
    </table>
  </body>
</html>
""";
    }

    public static void ValidateBrandingOptions(SiteAccessEmailOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.WebsiteUrl))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:WebsiteUrl est obligatoire.");
        }

        if (string.IsNullOrWhiteSpace(options.LinkedInUrl))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:LinkedInUrl est obligatoire.");
        }

        if (string.IsNullOrWhiteSpace(options.BrandLogoImageUrl))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:BrandLogoImageUrl est obligatoire.");
        }

        if (string.IsNullOrWhiteSpace(options.ProfileImageUrl))
        {
            throw new InvalidOperationException("La configuration SiteAccessEmail:ProfileImageUrl est obligatoire.");
        }
    }

    public static string HtmlEncode(string value)
        => System.Net.WebUtility.HtmlEncode(value);
}
