using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class EiamAssignmentRepository : IEiamAssignmentRepository
{
    private readonly DataContext _dataContext;

    public EiamAssignmentRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<EiamAssignment> GetByExternalId(string externalId)
    {
        var result = await _dataContext.EiamAssignments
            .Include(ea => ea.Department!.Committees)
            .Include(ea => ea.Office!.Committees)
            .Include(ea => ea.Committee!.Department)
            .Include(ea => ea.Parent!.Parent)
            .Include(ea => ea.Parent).ThenInclude(p => p!.Department).ThenInclude(d => d!.Offices)
            .Include(ea => ea.Parent).ThenInclude(p => p!.Office)
            .Include(ea => ea.Parent).ThenInclude(p => p!.Committee)
            .Include(ea => ea.Children).ThenInclude(c => c.Department).ThenInclude(d => d!.Offices)
            .Include(ea => ea.Children).ThenInclude(c => c.Office)
            .Include(ea => ea.Children).ThenInclude(c => c.Committee)
            .Include(ea => ea.Children).ThenInclude(c => c.Children).ThenInclude(cc => cc.Department)
            .Include(ea => ea.Children).ThenInclude(c => c.Children).ThenInclude(cc => cc.Office)
            .Include(ea => ea.Children).ThenInclude(c => c.Children).ThenInclude(cc => cc.Committee)
            .Include(ea => ea.Children).ThenInclude(c => c.Children).ThenInclude(c => c.Children).ThenInclude(cc => cc.Committee)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(ea => ea.ExternalId == externalId);

        return result ?? throw new EntityNotFoundException($"EiamAssignment with externalId {externalId} not found");
    }

    public async Task<EiamAssignment> GetById(Guid id)
    {
        var result = await _dataContext.EiamAssignments
            .FirstOrDefaultAsync(ea => ea.Id == id);

        return result ?? throw new EntityNotFoundException($"EiamAssignment with id {id} not found");
    }

    public async Task<EiamAssignment> Create(EiamAssignment eiamAssignment)
    {
        var entry = await _dataContext.EiamAssignments.AddAsync(eiamAssignment);
        await _dataContext.SaveChangesAsync();

        return entry.Entity;
    }
}
