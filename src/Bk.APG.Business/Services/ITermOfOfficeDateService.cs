using Bk.APG.Business.Models;

namespace Bk.APG.Business.Services;

public interface ITermOfOfficeDateService
{
    Task<bool> CheckForRunningGeneralElection();
    Task<TermOfOfficeDate> GetNextTermOfOfficeDate();
    Task<TermOfOfficeDate> GetGeneralElectionTermOfOfficeDate();
    Task<TermOfOfficeDate> GetCurrentTermOfOfficeDate();
    Task<TermOfOfficeDate> Update(TermOfOfficeDate termOfOfficeDate);
}
