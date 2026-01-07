using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class CommitteeTypeRepository : ICommitteeTypeRepository
{
    private readonly DataContext _dataContext;

    public CommitteeTypeRepository(DataContext context)
    {
        _dataContext = context;
    }

    public IEnumerable<CommitteeType> GetAll()
    {
        return _dataContext.CommitteeTypes.AsEnumerable();
    }

    public async Task<IEnumerable<CommitteeType>> GetList()
    {
        return await _dataContext.CommitteeTypes.Where(ct => !ct.IsDeleted).OrderBy(ct => ct.Sort).ToListAsync();
    }

    public async Task<CommitteeType> GetByIdForUpdate(Guid id, uint? updateDtoRowVersion = null)
    {
        var committeeType = await _dataContext.CommitteeTypes
            .FirstOrDefaultAsync(p => p.Id == id);

        if (committeeType is null)
        {
            throw new EntityNotFoundException($"CommitteeType Id={id} not found");
        }

        if (updateDtoRowVersion.HasValue && updateDtoRowVersion != committeeType.RowVersion)
        {
            _dataContext.Entry(committeeType).Property(x => x.RowVersion).OriginalValue = updateDtoRowVersion.Value;
        }

        return committeeType;
    }

    public async Task CommitChanges()
    {
        await _dataContext.SaveChangesAsync();
    }
}
