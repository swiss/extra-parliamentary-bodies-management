using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;

namespace Bk.APG.Business.Services;

public class TermOfOfficeDateService : ITermOfOfficeDateService
{
    private readonly ITermOfOfficeDateRepository _termOfOfficeDateRepository;
    private readonly DateOnly _today = DateOnly.FromDateTime(DateTime.Today);

    public TermOfOfficeDateService(ITermOfOfficeDateRepository termOfOfficeDateRepository)
    {
        _termOfOfficeDateRepository = termOfOfficeDateRepository;
    }

    public async Task<bool> CheckForRunningGeneralElection()
    {
        var termOfOfficeDates = await _termOfOfficeDateRepository.GetAll();

        return termOfOfficeDates.Any(t => t.IsGeneralElection == true);
    }

    public async Task<TermOfOfficeDate> GetNextTermOfOfficeDate()
    {
        var termOfOfficeDates = await _termOfOfficeDateRepository.GetAll();

        var nextTermOfOfficeDate = termOfOfficeDates.FirstOrDefault(t => t.BeginDate > _today);

        return nextTermOfOfficeDate ?? throw new EntityNotFoundException("No future TermOfOfficeDate entry found, General Election cannot be started!");
    }

    public async Task<TermOfOfficeDate> GetCurrentTermOfOfficeDate()
    {
        var termOfOfficeDates = await _termOfOfficeDateRepository.GetAll();

        var nextTermOfOfficeDate = termOfOfficeDates.FirstOrDefault(t => t.BeginDate <= _today && t.EndDate >= _today);

        return nextTermOfOfficeDate ?? throw new EntityNotFoundException("No future TermOfOfficeDate entry found, General Election cannot be started!");
    }

    public async Task<TermOfOfficeDate> GetGeneralElectionTermOfOfficeDate()
    {
        var termOfOfficeDates = await _termOfOfficeDateRepository.GetAll();

        var geTermOfOfficeDate = termOfOfficeDates.FirstOrDefault(t => t.IsGeneralElection == true);

        return geTermOfOfficeDate ?? throw new EntityNotFoundException("No TermOfOfficeDate with active GeneralElection found");
    }

    public async Task<TermOfOfficeDate> Update(TermOfOfficeDate termOfOfficeDate)
    {
        ArgumentNullException.ThrowIfNull(termOfOfficeDate);

        var termOfOfficeDateFromDb = await _termOfOfficeDateRepository.GetById(termOfOfficeDate.Id);

        var updatedTermOfOfficeDate = await _termOfOfficeDateRepository.Update(termOfOfficeDateFromDb, termOfOfficeDate);

        return updatedTermOfOfficeDate;
    }
}
