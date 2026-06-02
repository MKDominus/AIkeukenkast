using api.Domain;
using api.Application.Interfaces;
using api.Application.DTOs;
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
            .ThenInclude(p => p!.Ingredients)
            .Include(s => s.Municipality)
            .ToListAsync();
    }

    public async Task<Scan?> GetByIdAsync(int id)
    {
        return await _context.Scans
            .Include(s => s.DetectedProducts)
            .ThenInclude(dp => dp.Product)
            .ThenInclude(p => p!.Ingredients)
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

    public async Task<ScanStatsDto> GetStatsAsync()
    {
        var aggregates = await _context.DetectedProducts
            .Include(dp => dp.Product)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ProductsScanned = g.Sum(dp => dp.Count),
                SafetyProducts = g.Sum(dp => dp.Product == null || dp.Product.RiskLevel == ProductRiskLevel.Veilig ? dp.Count : 0),
                RiskProducts = g.Sum(dp => dp.Product != null && dp.Product.RiskLevel != ProductRiskLevel.Veilig ? dp.Count : 0)
            })
            .FirstOrDefaultAsync();

        var totalScans = await _context.Scans.CountAsync();

        if (aggregates == null || aggregates.ProductsScanned == 0)
        {
            return new ScanStatsDto
            {
                TotalScans = totalScans,
                ProductsScanned = 0,
                AverageSafety = 0,
                AverageRisk = 0
            };
        }

        return new ScanStatsDto
        {
            TotalScans = totalScans,
            ProductsScanned = aggregates.ProductsScanned,
            AverageSafety = (double)aggregates.SafetyProducts / aggregates.ProductsScanned * 100,
            AverageRisk = (double)aggregates.RiskProducts / aggregates.ProductsScanned * 100
        };
    }
}
