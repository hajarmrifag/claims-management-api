using Claims.Application.Claims;
using Claims.Domain.Entities;

namespace Claims.Application.Documents;

public class ClaimDocumentService
{
    private readonly IClaimRepository _claimRepository;
    private readonly IFileStorage _fileStorage;

    public ClaimDocumentService(
        IClaimRepository claimRepository,
        IFileStorage fileStorage)
    {
        _claimRepository = claimRepository;
        _fileStorage = fileStorage;
    }

    public async Task<ClaimDocumentResponse> UploadAsync(
        Guid claimId,
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var claim = await _claimRepository.GetByIdAsync(
            claimId,
            cancellationToken);

        if (claim is null)
        {
            throw new ArgumentException("Claim not found.");
        }

        var fileUrl = await _fileStorage.UploadAsync(
            stream,
            fileName,
            contentType,
            cancellationToken);

        var document = new ClaimDocument(
            claimId,
            fileName,
            fileUrl);

        await _claimRepository.AddDocumentAsync(
            document,
            cancellationToken);

        await _claimRepository.SaveChangesAsync(
            cancellationToken);

        return new ClaimDocumentResponse(
            document.Id,
            document.ClaimId,
            document.FileName,
            document.BlobUrl,
            document.UploadedAt);
    }

    public async Task<(Stream Stream, string FileName)> DownloadAsync(
        Guid claimId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var document = await _claimRepository.GetDocumentByIdAsync(
            documentId,
            cancellationToken);

        if (document is null || document.ClaimId != claimId)
        {
            throw new ArgumentException("Claim document not found.");
        }

        var stream = await _fileStorage.OpenReadAsync(
            document.BlobUrl,
            cancellationToken);

        return (stream, document.FileName);
    }

}
