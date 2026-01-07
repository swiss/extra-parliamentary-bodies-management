using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class OccupationService : IOccupationService
{
    private readonly IOccupationRepository _occupationRepository;
    private readonly ICultureService _cultureService;

    public OccupationService(
        IOccupationRepository occupationRepository,
        ICultureService cultureService)
    {
        _occupationRepository = occupationRepository;
        _cultureService = cultureService;
    }

    public async Task<IEnumerable<OccupationDto>> GetBySearchString(string searchString)
    {
        return (await _occupationRepository.GetBySearchString(searchString)).Select(y => MasterDataMapper.MapToOccupationDto(y, _cultureService.GetCurrentUiCulture()));
    }
}
