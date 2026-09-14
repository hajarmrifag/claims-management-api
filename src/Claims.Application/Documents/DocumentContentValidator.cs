namespace Claims.Application.Documents;

public static class DocumentContentValidator
{
    private static readonly IReadOnlyDictionary<string, byte[]> Signatures =
        new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["application/pdf"] = "%PDF-"u8.ToArray(),
            ["image/jpeg"] = [0xFF, 0xD8, 0xFF],
            ["image/png"] = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]
        };

    public static async Task<bool> MatchesDeclaredTypeAsync(
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        if (!Signatures.TryGetValue(contentType, out var expectedSignature))
        {
            return false;
        }

        var actualSignature = new byte[expectedSignature.Length];
        var bytesRead = 0;

        while (bytesRead < actualSignature.Length)
        {
            var read = await stream.ReadAsync(
                actualSignature.AsMemory(bytesRead),
                cancellationToken);

            if (read == 0)
            {
                return false;
            }

            bytesRead += read;
        }

        return actualSignature.AsSpan().SequenceEqual(expectedSignature);
    }
}
