using System.ComponentModel.DataAnnotations;

namespace Bk.APG.Business.Dtos;

public class PagingParametersDto
{
    [Required, Range(0, int.MaxValue)]
    public int? PageIndex { get; init; }

    [Required]
    public int? PageSize { get; init; }
}
