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
}