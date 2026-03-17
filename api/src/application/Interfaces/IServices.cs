using api.Domain;

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
}

public interface IScanService : IBaseService<Scan>
{
    Task<IEnumerable<Scan>> GetScansByUserIdAsync(int userId);
}

public interface IIngredientService : IBaseService<Ingredient>
{
}

public interface IMunicipalityService : IBaseService<Municipality>
{
}

public interface IUserService : IBaseService<User>
{
}

public interface IDetectedProductService : IBaseService<DetectedProduct>
{
    Task<IEnumerable<DetectedProduct>> GetByScanIdAsync(int scanId);
}
