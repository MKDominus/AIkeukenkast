using api.Domain;
using api.Application.Interfaces;
using api.Application.DTOs;
using api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Application.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Ingredients) 
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Ingredients)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<Product> AddAsync(Product entity)
    {
        _context.Products.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Product entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Products.FindAsync(id);
        if (entity != null)
        {
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Ingredient>> GetIngredientsByProductIdAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Ingredients)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        return product?.Ingredients ?? Enumerable.Empty<Ingredient>();
    }

    public async Task<List<Ingredient>> GetIngredientsByIdsAsync(IEnumerable<int> ingredientIds)
    {
        var ids = ingredientIds.Distinct().ToList();

        if (ids.Count == 0)
        {
            return new List<Ingredient>();
        }

        return await _context.Ingredients
            .Where(i => ids.Contains(i.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductCategoryCountDto>> GetCategoryCountsAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .GroupBy(p => p.ProductType)
            .Select(group => new ProductCategoryCountDto
            {
                ProductType = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(item => item.Count)
            .ThenBy(item => item.ProductType)
            .ToListAsync();
    }
}
