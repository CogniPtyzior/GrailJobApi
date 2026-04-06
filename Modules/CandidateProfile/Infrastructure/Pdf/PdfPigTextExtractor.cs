using GrailJobApi.Modules.CandidateProfile.Application;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace GrailJobApi.Modules.CandidateProfile.Infrastructure.Pdf;

public sealed class PdfPigTextExtractor : IPdfTextExtractor
{
    public Task<string> ExtractTextAsync(Stream pdfStream, CancellationToken cancellationToken = default)
    {
        pdfStream.Position = 0;
        using var document = PdfDocument.Open(pdfStream);
        var builder = new StringBuilder();

        foreach (var page in document.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var pageText = ContentOrderTextExtractor.GetText(page, true);
            if (string.IsNullOrWhiteSpace(pageText))
            {
                continue;
            }

            pageText = pageText
                .ReplaceLineEndings(" ")
                .Replace('\t', ' ');

            pageText = string.Join(" ", pageText.Split(' ', StringSplitOptions.RemoveEmptyEntries)).Trim();

            if (pageText.Length == 0)
            {
                continue;
            }

            if (builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(pageText);
        }

        return Task.FromResult(builder.ToString().Trim());
    }
}
