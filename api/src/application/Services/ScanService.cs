using api.Domain;
using api.Application.Interfaces;
using api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Application.Services;

public class ScanService : IScanService
{
    private readonly AppDbContext _context;

    public ScanService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Scan>> GetAllAsync()
    {
        return await _context.Scans
            .Include(s => s.DetectedProducts)
            .ThenInclude(dp => dp.Product)
            .Include(s => s.User)
            .Include(s => s.Municipality)
            .ToListAsync();
    }

    public async Task<Scan?> GetByIdAsync(int id)
    {
        return await _context.Scans
            .Include(s => s.DetectedProducts)
            .ThenInclude(dp => dp.Product)
            .Include(s => s.User)
            .Include(s => s.Municipality)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Scan> AddAsync(Scan entity)
    {
        _context.Scans.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Scan entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Scans.FindAsync(id);
        if (entity != null)
        {
            _context.Scans.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Scan>> GetScansByUserIdAsync(int userId)
    {
        return await _context.Scans
            .Include(s => s.DetectedProducts)
            .ThenInclude(dp => dp.Product)
            .Where(s => s.UserId == userId)
            .ToListAsync();
    }
}
