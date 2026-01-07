using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;

namespace Bk.APG.Business.Services;

public class SalutationGeneratorService : ISalutationGeneratorService
{
    private readonly IMasterDataRepository _masterDataRepository;

    public SalutationGeneratorService(
        IMasterDataRepository masterDataRepository)
    {
        _masterDataRepository = masterDataRepository;
    }

    public async Task<string> CreateSalutationTextForPerson(Guid genderId, Guid correspondenceLanguageId, string surname, string? title)
    {
        var salutationText = string.Empty;

        var salutations = await _masterDataRepository.GetSalutations();

        var salutation = salutations.FirstOrDefault(s => s.GenderId == genderId);

        if (salutation != null)
        {
            if (correspondenceLanguageId == Guid.Parse(Language.GermanId))
            {
                salutationText = string.IsNullOrWhiteSpace(title) ? salutation.DescriptionDe + " " + surname : salutation.DescriptionDe + " " + title + " " + surname;
            }
            else if (correspondenceLanguageId == Guid.Parse(Language.FrenchId))
            {
                salutationText = salutation.DescriptionFr;
            }
            else if (correspondenceLanguageId == Guid.Parse(Language.ItalianId))
            {
                salutationText = salutation.DescriptionIt;
            }
        }

        return salutationText;
    }
}
