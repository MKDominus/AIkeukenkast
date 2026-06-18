using api.Application.DTOs;
using api.Domain;
using Microsoft.AspNetCore.Http;

namespace api.Application.Interfaces;

public interface IBaseService<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public interface IProductService : IBaseService<Product>
{
    Task<IEnumerable<Ingredient>> GetIngredientsByProductIdAsync(int productId);
    Task<List<Ingredient>> GetIngredientsByIdsAsync(IEnumerable<int> ingredientIds);
    Task<IEnumerable<ProductCategoryCountDto>> GetCategoryCountsAsync();

    Task<IEnumerable<Product>> GetAllAlternativesForProductAsync(int id);
}

public interface IScanService : IBaseService<Scan>
{
    Task<ScanStatsDto> GetStatsAsync();
    Task<IEnumerable<ProductCategoryCountDto>> GetCategoryCountsAsync();
}

public interface IScanImportService
{
    Task<IReadOnlyList<Scan>> CreateFromImagesAsync(
        IReadOnlyCollection<IFormFile> images,
        string? postalCode,
        bool postalCodePermission);
}

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<(Stream Content, string ContentType)?> DownloadImageAsync(string imageUrl);
}

public interface IIngredientService : IBaseService<Ingredient>
{
}

public interface IMunicipalityService : IBaseService<Municipality>
{
    Task<IEnumerable<MunicipalityScanCountDto>> GetScanCountsAsync();
}

public interface IDetectedProductService : IBaseService<DetectedProduct>
{
    Task<IEnumerable<DetectedProduct>> GetByScanIdAsync(int scanId);
}
