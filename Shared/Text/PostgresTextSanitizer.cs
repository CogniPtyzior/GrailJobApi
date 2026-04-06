namespace GrailJobApi.Shared.Text;

public static class PostgresTextSanitizer
{
    public static string Sanitize(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var normalized = input.Normalize(NormalizationForm.FormKC);
        var builder = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            // PostgreSQL text cannot store NUL
            if (ch == '\0')
            {
                continue;
            }

            // Keep common whitespace, remove other control chars
            if (char.IsControl(ch) && ch is not '\r' and not '\n' and not '\t')
            {
                continue;
            }

            builder.Append(ch);
        }

        return builder.ToString().Trim();
    }
}