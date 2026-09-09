namespace Claims.Domain.Entities;

public class ClaimDocument
{
    public Guid Id { get; private set; }

    public Guid ClaimId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string BlobUrl { get; private set; } = string.Empty;

    public DateTime UploadedAt { get; private set; }

    private ClaimDocument()
    {
    }

    public ClaimDocument(
        Guid claimId,
        string fileName,
        string blobUrl)
    {
        Id = Guid.NewGuid();
        ClaimId = claimId;
        FileName = fileName;
        BlobUrl = blobUrl;
        UploadedAt = DateTime.UtcNow;
    }
}
