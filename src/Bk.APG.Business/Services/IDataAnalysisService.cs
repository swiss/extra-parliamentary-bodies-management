namespace Bk.APG.Business.Services;

public interface IDataAnalysisService
{
    Task<(string fileName, Stream content)> GenerateCommitteeTypeExport(DateOnly dataAnalysisDate);
    Task<(string fileName, Stream content)> GenerateCommitteeExport(DateOnly dataAnalysisDate);
    Task<(string fileName, Stream content)> GenerateMembershipExport(DateOnly dataAnalysisDate);
    Task<(string fileName, Stream content)> GenerateMembershipInterestExport(DateOnly dataAnalysisDate);
    Task<(string fileName, Stream content)> GeneratePersonExport(DateOnly dataAnalysisDate);
    Task<(string fileName, Stream content)> GenerateContactPointExport(DateOnly dataAnalysisDate, Guid contactPointTypeId);
    Task<(string fileName, Stream content)> GenerateRegionExport(DateOnly dataAnalysisDate);
    Task<(string fileName, Stream content)> GenerateAgeExport(DateOnly dataAnalysisDate);
}
