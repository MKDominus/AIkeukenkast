using api.Domain;
using api.Application.Interfaces;
using api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Application.Services;

public class DetectedProductService : IDetectedProductService
{
    private readonly AppDbContext _context;

    public DetectedProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DetectedProduct>> GetAllAsync()
    {
        return await _context.DetectedProducts
            .Include(dp => dp.Product)
            .Include(dp => dp.Scan)
            .ToListAsync();
    }

    public async Task<DetectedProduct?> GetByIdAsync(int id)
    {
        return await _context.DetectedProducts
            .Include(dp => dp.Product)
            .Include(dp => dp.Scan)
            .FirstOrDefaultAsync(dp => dp.Id == id);
    }

    public async Task<DetectedProduct> AddAsync(DetectedProduct entity)
    {
        _context.DetectedProducts.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(DetectedProduct entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.DetectedProducts.FindAsync(id);
        if (entity != null)
        {
            _context.DetectedProducts.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<DetectedProduct>> GetByScanIdAsync(int scanId)
    {
        return await _context.DetectedProducts
            .Include(dp => dp.Product)
            .Where(dp => dp.ScanId == scanId)
            .ToListAsync();
    }
}
