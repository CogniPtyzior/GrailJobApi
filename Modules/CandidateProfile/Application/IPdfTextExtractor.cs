namespace GrailJobApi.Modules.CandidateProfile.Application;

public interface IPdfTextExtractor
{
    Task<string> ExtractTextAsync(Stream pdfStream, CancellationToken cancellationToken = default);
}
