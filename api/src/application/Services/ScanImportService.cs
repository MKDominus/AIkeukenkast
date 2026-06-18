using api.Application.Interfaces;
using api.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace api.Application.Services;

public class ScanImportService : IScanImportService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IProductService _productService;
    private readonly IScanService _scanService;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IConfiguration _configuration;

    public ScanImportService(
        IHttpClientFactory httpClientFactory,
        IProductService productService,
        IScanService scanService,
        IBlobStorageService blobStorageService,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _productService = productService;
        _scanService = scanService;
        _blobStorageService = blobStorageService;
        _configuration = configuration;
    }

    public async Task<IReadOnlyList<Scan>> CreateFromImagesAsync(
        IReadOnlyCollection<IFormFile> images,
        string? postalCode)
    {
        var validImages = images.Where(image => image.Length > 0).ToList();
        if (validImages.Count == 0)
        {
            throw new ArgumentException("At least one image file is required.", nameof(images));
        }

        var normalizedPostalCode = string.IsNullOrWhiteSpace(postalCode) ? null : postalCode.Trim();

        var aiServiceBaseUrl = _configuration["AI_SERVICE_BASE_URL"] ?? "http://127.0.0.1:8000";

        if (string.IsNullOrWhiteSpace(aiServiceBaseUrl))
        {
            throw new InvalidOperationException("AI_SERVICE_BASE_URL is not configured.");
        }

        var predictionEndpoint = $"{aiServiceBaseUrl.TrimEnd('/')}/predict";

        //expensive operation -> will change to get only product names and ids instead of all products
        var knownProducts = (await _productService.GetAllAsync())
            .ToDictionary(product => product.ProductName, StringComparer.OrdinalIgnoreCase);

        using var httpClient = _httpClientFactory.CreateClient();
        using var form = new MultipartFormDataContent();
        var streams = new List<Stream>();
        var fileContents = new List<StreamContent>();

        try
        {
            foreach (var image in validImages)
            {
                var stream = image.OpenReadStream();
                streams.Add(stream);

                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                    string.IsNullOrWhiteSpace(image.ContentType)
                        ? "application/octet-stream"
                        : image.ContentType);

                fileContents.Add(fileContent);
                form.Add(fileContent, "images", image.FileName);
            }

            using var response = await httpClient.PostAsync(predictionEndpoint, form);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Prediction service returned {(int)response.StatusCode} ({response.ReasonPhrase}).");
            }

            var payload = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(payload);
            var results = document.RootElement.GetProperty("results").EnumerateArray().ToList();

            if (results.Count == 0)
            {
                throw new InvalidOperationException("Prediction service returned no results.");
            }

            var createdScans = new List<Scan>();

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                var image = validImages[i];

                var imageUrl = await _blobStorageService.UploadImageAsync(image);

                var detectedProducts = result
                    .GetProperty("predictions")
                    .EnumerateArray()
                    .Select(prediction => new
                    {
                        ProductName = prediction.GetProperty("product name").GetString(),
                        Probability = prediction.GetProperty("probability").GetDouble()
                    })
                    .Where(prediction =>
                        prediction.ProductName != null &&
                        knownProducts.ContainsKey(prediction.ProductName))
                    .Select(prediction => new DetectedProduct
                    {
                        ProductId = knownProducts[prediction.ProductName!].ProductId,
                        Confidence = prediction.Probability,
                        Count = 1
                    })
                    .ToList();

                int randomIdNumber = new Random().Next(1, 6);

                var entity = new Scan
                {
                    ScanDate = DateTime.UtcNow,
                    ImageUrl = imageUrl,
                    PostalCode = normalizedPostalCode,
                    MunicipalityId = randomIdNumber,
                    DetectedProducts = detectedProducts
                };

                await _scanService.AddAsync(entity);
                createdScans.Add(entity);
            }

            return createdScans;
        }
        finally
        {
            foreach (var content in fileContents)
            {
                content.Dispose();
            }

            foreach (var stream in streams)
            {
                stream.Dispose();
            }
        }
    }
}