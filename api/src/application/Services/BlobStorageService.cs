using Azure.Storage.Blobs;
using api.Application.Interfaces;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _container;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("AzureBlobStorage");

        _container = new BlobContainerClient(
            connectionString,
            "tzorgopslagfoto");
    }

    public async Task<string> UploadImageAsync(IFormFile image)
    {
        var extension = Path.GetExtension(image.FileName);

        var blobName =
            $"{Guid.NewGuid()}{extension}";

        var blobClient =
            _container.GetBlobClient(blobName);

        await using var stream =
            image.OpenReadStream();

        await blobClient.UploadAsync(stream);

        return blobClient.Uri.ToString();
    }

    public async Task<(Stream Content, string ContentType)?> DownloadImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return null;
        }

        var uri = new Uri(imageUrl);
        var relativePath = Uri.UnescapeDataString(uri.AbsolutePath).Trim('/');
        var containerPrefix = "tzorgopslagfoto/";

        var blobName = relativePath.StartsWith(containerPrefix, StringComparison.OrdinalIgnoreCase)
            ? relativePath[containerPrefix.Length..]
            : relativePath;

        if (string.IsNullOrWhiteSpace(blobName))
        {
            return null;
        }

        var blobClient = _container.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var download = await blobClient.DownloadStreamingAsync();
        var contentType = download.Value.Details.ContentType ?? "application/octet-stream";

        return (download.Value.Content, contentType);
    }
}