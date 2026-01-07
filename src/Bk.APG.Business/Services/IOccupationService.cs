using Bk.APG.Business.Dtos;

namespace Bk.APG.Business.Services;

public interface IOccupationService
{
    Task<IEnumerable<OccupationDto>> GetBySearchString(string searchString);
}
