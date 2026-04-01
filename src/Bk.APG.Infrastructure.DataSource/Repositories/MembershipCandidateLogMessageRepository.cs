using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Repositories;

public class MembershipCandidateLogMessageRepository : IMembershipCandidateLogMessageRepository
{
    private readonly DataContext _dataContext;

    public MembershipCandidateLogMessageRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<MembershipCandidateLogMessage> Create(MembershipCandidateLogMessage logMessage)
    {
        var entry = await _dataContext.MembershipCandidateLogMessages.AddAsync(logMessage);
        await _dataContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<IEnumerable<MembershipCandidateLogMessage>> GetByPersonId(Guid personId)
    {
        return await _dataContext.MembershipCandidateLogMessages
            .Where(log => log.PersonId == personId)
            .ToListAsync();
    }

    public void DeleteRange(IEnumerable<MembershipCandidateLogMessage> logMessages)
    {
        _dataContext.MembershipCandidateLogMessages.RemoveRange(logMessages);
    }
}
