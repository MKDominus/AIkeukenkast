using api.Domain;
using api.Application.Interfaces;
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
            .FirstOrDefaultAsync(p => p.Id == id);
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
            .FirstOrDefaultAsync(p => p.Id == productId);

        return product?.Ingredients ?? Enumerable.Empty<Ingredient>();
    }
}
