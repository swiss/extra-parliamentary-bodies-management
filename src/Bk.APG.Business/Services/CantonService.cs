using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class CantonService : ICantonService
{
    private readonly ICantonRepository _cantonRepository;

    public CantonService(ICantonRepository cantonRepository)
    {
        _cantonRepository = cantonRepository;
    }

    public async Task<IEnumerable<CantonDto>> GetAll()
    {
        var cantons = await _cantonRepository.GetAll();

        return cantons.Select(c => CantonMapper.ToCantonDto(c)).OrderBy(c => c.Sort).ThenBy(c => c.Text);
    }

    public async Task<Canton> CreateOrUpdate(Canton canton)
    {
        var now = DateTime.UtcNow;

        var cantonFromDb = await _cantonRepository.GetByUri(canton.Uri);
        if (cantonFromDb is null)
        {
            var cantonToCreate = new Canton
            {
                Created = now,
                CreatedBy = canton.ModifiedBy,
                Modified = now,
                ModifiedBy = canton.ModifiedBy,
                IsDeleted = false,
                TextDe = canton.TextDe,
                TextFr = canton.TextFr,
                TextIt = canton.TextIt,
                TextRm = canton.TextRm,
                DescriptionDe = canton.DescriptionDe,
                DescriptionFr = canton.DescriptionFr,
                DescriptionIt = canton.DescriptionIt,
                DescriptionRm = canton.DescriptionRm,
                Uri = canton.Uri,
                Sort = canton.Sort,
            };

            var newCanton = await _cantonRepository.Create(cantonToCreate);
            return newCanton;
        }

        var updatedCanton = await _cantonRepository.Update(cantonFromDb, canton);

        return updatedCanton;
    }
}
