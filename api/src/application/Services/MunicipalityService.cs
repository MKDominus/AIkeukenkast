using api.Domain;
using api.Application.DTOs;
using api.Application.Interfaces;
using api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Application.Services;

public class MunicipalityService : IMunicipalityService
{
    private readonly AppDbContext _context;

    public MunicipalityService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Municipality>> GetAllAsync()
    {
        return await _context.Municipalities.ToListAsync();
    }

    public async Task<Municipality?> GetByIdAsync(int id)
    {
        return await _context.Municipalities.FindAsync(id);
    }

    public async Task<Municipality> AddAsync(Municipality entity)
    {
        _context.Municipalities.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Municipality entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Municipalities.FindAsync(id);
        if (entity != null)
        {
            _context.Municipalities.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<MunicipalityScanCountDto>> GetScanCountsAsync()
    {
        return await (
            from scan in _context.Scans
            where scan.MunicipalityId != null
            join municipality in _context.Municipalities on scan.MunicipalityId equals municipality.Id
            group scan by new { municipality.Id, municipality.Name } into grouped
            orderby grouped.Count() descending
            select new MunicipalityScanCountDto
            {
                MunicipalityId = grouped.Key.Id,
                MunicipalityName = grouped.Key.Name,
                ScanCount = grouped.Count()
            }
        ).ToListAsync();
    }
}
