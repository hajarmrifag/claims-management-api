namespace Claims.Application.Documents;

public record ClaimDocumentResponse(
    Guid Id,
    Guid ClaimId,
    string FileName,
    string BlobUrl,
    DateTime UploadedAt
);
