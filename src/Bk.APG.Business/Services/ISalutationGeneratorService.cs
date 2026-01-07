namespace Bk.APG.Business.Services;

public interface ISalutationGeneratorService
{
    Task<string> CreateSalutationTextForPerson(Guid genderId, Guid correspondenceLanguageId, string surname, string? title);
}
