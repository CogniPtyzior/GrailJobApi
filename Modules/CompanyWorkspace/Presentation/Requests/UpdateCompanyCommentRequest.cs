namespace GrailJobApi.Modules.CompanyWorkspace.Presentation.Requests;

/// <summary>Company comment update payload.</summary>
public sealed class UpdateCompanyCommentRequest
{
    /// <summary>User comment. Empty text clears the comment.</summary>
    public string Comment { get; init; } = string.Empty;
}
