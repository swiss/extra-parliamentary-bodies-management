using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IGeneralMeasureService
{
    Task<IEnumerable<GeneralMeasureDto>> GetGeneralMeasures();
    Task AddOrUpdateGeneralMeasure(GeneralMeasureUpdateDto generalMeasureUpdate);
}
