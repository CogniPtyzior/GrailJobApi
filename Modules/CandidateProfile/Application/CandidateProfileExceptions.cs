namespace GrailJobApi.Modules.CandidateProfile.Application;

public sealed class InvalidCandidateProfileDocumentException(string message) : Exception(message);

public sealed class CandidateProfileEnrichmentUnavailableException(string message, Exception? innerException = null)
    : Exception(message, innerException);
