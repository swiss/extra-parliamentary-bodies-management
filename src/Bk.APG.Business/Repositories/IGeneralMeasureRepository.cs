using Bk.APG.Business.Models;

namespace Bk.APG.Business.Repositories;

public interface IGeneralMeasureRepository
{
    Task<IEnumerable<GeneralGenderMeasure>> GetGeneralGenderMeasures();
    Task<IEnumerable<GeneralLanguageMeasure>> GetGeneralLanguageMeasures();
    Task<GeneralGenderMeasure?> GetGeneralGenderMeasure(Guid departmentId);
    Task<GeneralLanguageMeasure?> GetGeneralLanguageMeasure(Guid departmentId);
    Task<GeneralGenderMeasure?> GetGeneralGenderMeasureForUpdate(Guid departmentId);
    Task<GeneralLanguageMeasure?> GetGeneralLanguageMeasureForUpdate(Guid departmentId);
    Task AddGeneralGenderMeasure(GeneralGenderMeasure generalGenderMeasure);
    Task AddGeneralLanguageMeasure(GeneralLanguageMeasure generalLanguageMeasure);
    Task CommitChanges();
}
