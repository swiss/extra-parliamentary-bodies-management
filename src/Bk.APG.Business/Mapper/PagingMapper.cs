using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Mapper;

public static class PagingMapper
{
    public static PagingParameters ToPagingParameters(PagingParametersDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.PageSize is null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (dto.PageIndex is null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        return new PagingParameters
        {
            PageIndex = dto.PageIndex.GetValueOrDefault(),
            PageSize = dto.PageSize.GetValueOrDefault()
        };
    }
}
