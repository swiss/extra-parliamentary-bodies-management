using Bk.APG.Business.Dtos;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Mapper;

public static class PagingMapper
{
    public static PagingParameters ToPagingParameters(PagingParametersDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(dto.PageSize);
        ArgumentNullException.ThrowIfNull(dto.PageIndex);

        return new PagingParameters
        {
            PageIndex = dto.PageIndex.Value,
            PageSize = dto.PageSize.Value
        };
    }
}
