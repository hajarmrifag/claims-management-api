using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Claims.Application.Documents;

namespace Claims.Infrastructure.Documents;

public class AzureBlobFileStorage : IFileStorage
{
    private readonly BlobContainerClient _containerClient;

    public AzureBlobFileStorage(
        BlobContainerClient containerClient)
    {
        _containerClient = containerClient;
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await _containerClient.CreateIfNotExistsAsync(
            cancellationToken: cancellationToken);

        var safeFileName =
            $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";

        var blobClient =
            _containerClient.GetBlobClient(safeFileName);

        await blobClient.UploadAsync(
            stream,
            new BlobHttpHeaders
            {
                ContentType = contentType
            },
            cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> OpenReadAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobName(fileUrl);

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        var response = await blobClient.DownloadStreamingAsync(
            cancellationToken: cancellationToken);

        return response.Value.Content;
    }

    public async Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        var blobName = GetBlobName(fileUrl);

        var blobClient =
            _containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(
            cancellationToken: cancellationToken);
    }

    private static string GetBlobName(string fileUrl)
    {
        var uri = new Uri(fileUrl);

        return Uri.UnescapeDataString(
            Path.GetFileName(uri.AbsolutePath));
    }
}
