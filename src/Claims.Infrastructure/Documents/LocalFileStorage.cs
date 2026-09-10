using Claims.Application.Documents;

namespace Claims.Infrastructure.Documents;

public class LocalFileStorage : IFileStorage
{
    private readonly string _storagePath;

    public LocalFileStorage()
    {
        _storagePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "uploads");

        Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var safeFileName =
            $"{Guid.NewGuid()}-{Path.GetFileName(fileName)}";

        var filePath = Path.Combine(
            _storagePath,
            safeFileName);

        await using var fileStream = File.Create(filePath);

        await stream.CopyToAsync(
            fileStream,
            cancellationToken);

        return $"/uploads/{safeFileName}";
    }

    public Task<Stream> OpenReadAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(fileUrl);

        var filePath = Path.Combine(
            _storagePath,
            fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Stored file was not found.",
                fileName);
        }

        Stream stream = File.OpenRead(filePath);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(
        string fileUrl,
        CancellationToken cancellationToken = default)
    {
        var fileName = Path.GetFileName(fileUrl);

        var filePath = Path.Combine(
            _storagePath,
            fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
