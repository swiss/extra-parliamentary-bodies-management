using System.Globalization;
using System.IO.Compression;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class FormLetterService : IFormLetterService
{
    private const string GeneralElectionTextGerman = "Gesamterneuerungswahl";
    private const string GeneralElectionTextFrench = "Renouvellement intégral";
    private const string GeneralElectionTextItalian = "Rinnovo integrale";
    private const string GeneralElectionTextRomansh = "Renovaziun totala";

    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly IDocumentService _documentServiceInternal;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IFormLetterSenderRepository _formLetterSenderRepository;
    private readonly ILogger<FormLetterService> _logger;

    public FormLetterService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        ITermOfOfficeDateService termOfOfficeDateService,
        IDocumentService documentServiceInternal,
        ICommitteeRepository committeeRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IMasterDataRepository masterDataRepository,
        IFormLetterSenderRepository formLetterSenderRepository,
        ILogger<FormLetterService> logger)
    {
        _documentService = documentService;
        _termOfOfficeDateService = termOfOfficeDateService;
        _documentServiceInternal = documentServiceInternal;
        _committeeRepository = committeeRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _masterDataRepository = masterDataRepository;
        _formLetterSenderRepository = formLetterSenderRepository;
        _logger = logger;
    }

    public async Task<(string fileName, Stream content)> CreateFormLetterAsZipFile(FormLetterFilterParameters filterDto)
    {
        ArgumentNullException.ThrowIfNull(filterDto);

        _logger.LogInformation("Generate form letter report");

        var reportDtos = await FillFormLetterDto(filterDto);

        var zipStream = new MemoryStream();
        const int maxFileNameLength = 150;

        // Type "single" means, that every committee will be exported in 1 to 4 single files
        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            if (filterDto.ExportType == "single")
            {
                foreach (var reportDto in reportDtos)
                {
                    if (reportDto.Memberships == null)
                    {
                        continue;
                    }

                    var grouped = reportDto.Memberships.GroupBy(m => m.CommitteeId).ToList();

                    foreach (var group in grouped)
                    {
                        var currentCommitteeId = group.Key;

                        var first = group.First();

                        var reducedMembersDto = group
                            .OrderBy(m => m.Surname)
                            .ThenBy(m => m.GivenName)
                            .ToList();

                        var reducedDataDto = new FormLetterReportDto
                        {
                            FormLetterLanguage = reportDto.FormLetterLanguage,
                            SenderOffice = reportDto.SenderOffice,
                            SenderName = reportDto.SenderName,
                            SenderStreet = reportDto.SenderStreet,
                            SenderZip = reportDto.SenderZip,
                            SenderCity = reportDto.SenderCity,
                            SenderPhone = reportDto.SenderPhone,
                            SenderEmail = reportDto.SenderEmail,
                            SenderWebsite = reportDto.SenderWebsite,
                            SenderSignature = reportDto.SenderSignature,
                            HasSignature = reportDto.HasSignature,
                            NextTermOfOfficeBeginDate = reportDto.NextTermOfOfficeBeginDate,
                            NextTermOfOfficeEndDate = reportDto.NextTermOfOfficeEndDate,
                            TermOfOfficeEndDate = reportDto.TermOfOfficeEndDate,
                            Memberships = reducedMembersDto,
                            TemplateName = reportDto.TemplateName,
                        };

                        var fileName = first.FileName.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);

                        if (fileName.Length > maxFileNameLength)
                        {
                            fileName = fileName[..maxFileNameLength];
                        }

                        fileName = $"{fileName}_{GetFileNameLanguageExtension(reducedDataDto.FormLetterLanguage)}";

                        await AddDocumentToZip(fileName, filterDto.ExportFileType!, reducedDataDto, zip);
                    }
                }
            }
            else
            {
                // here, all the committees are in one document per language
                foreach (var reportDto in reportDtos)
                {
                    var fileName = BusinessTexts.FormLetterCompleteExport_Filename;

                    if (fileName.Length > maxFileNameLength)
                    {
                        fileName = fileName[..maxFileNameLength];
                    }

                    fileName = $"{fileName}_{GetFileNameLanguageExtension(reportDto.FormLetterLanguage)}";

                    await AddDocumentToZip(fileName, filterDto.ExportFileType!, reportDto, zip);
                }
            }
        }

        zipStream.Position = 0;
        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd_HHmmss}_{BusinessTexts.FormLetterCompleteExport_Filename}.zip", zipStream);
    }

    private async Task AddDocumentToZip(string fileName, string exportFileType, FormLetterReportDto reportDto, ZipArchive zip)
    {
        if (exportFileType == "word")
        {
            var documentFile = zip.CreateEntry($"{fileName}.docx", CompressionLevel.Fastest);
            await using var doucmentFileStream = await documentFile.OpenAsync();

            await using var documentStream = (MemoryStream)await _documentService.CreateWordFromTemplate($"Templates/{reportDto.TemplateName}.docx", reportDto, "formLetter");
            await documentStream.CopyToAsync(doucmentFileStream);
        }
        else
        {
            var documentFile = zip.CreateEntry($"{fileName}.pdf", CompressionLevel.Fastest);
            await using var doucmentFileStream = await documentFile.OpenAsync();

            await using var documentStream = (MemoryStream)await _documentService.CreatePdfFromTemplate($"Templates/{reportDto.TemplateName}.docx", reportDto, "formLetter");
            await documentStream.CopyToAsync(doucmentFileStream);
        }
    }

    private async Task<IEnumerable<FormLetterReportDto>> FillFormLetterDto(FormLetterFilterParameters filterDto)
    {
        const string template = "FormLetterGeneralElection";

        var allElectionTypes = await _masterDataRepository.GetElectionTypes();
        var electionTypeList = allElectionTypes.Select(e => e.Id).ToList();
        electionTypeList.Remove(ElectionType.MembershipEndedBecauseOfDeathGuid);
        electionTypeList.Remove(ElectionType.PermanentGuid);

        if (filterDto.ElectionTypeIds != null && filterDto.ElectionTypeIds.Any())
        {
            electionTypeList = electionTypeList
                .Where(id => filterDto.ElectionTypeIds.Contains(id))
                .ToList();
        }

        var electionTypeListPresent = electionTypeList.ToList();
        electionTypeListPresent.Remove(ElectionType.NewElectionGuid);
        electionTypeListPresent.Remove(ElectionType.ReElectionGuid);

        var electionTypeListFuture = electionTypeList.ToList();
        electionTypeListFuture.Remove(ElectionType.MaximumMembershipDurationGuid);
        electionTypeListFuture.Remove(ElectionType.OtherRetirementReasonGuid);
        electionTypeListFuture.Remove(ElectionType.RetirementGuid);

        var sender = await _formLetterSenderRepository.GetByIdForUpdate(filterDto.FormLetterSenderId);

        var nextTermOfOfficeDate = await _termOfOfficeDateService.GetNextTermOfOfficeDate();
        var currentTermOfOfficeDate = await _termOfOfficeDateService.GetCurrentTermOfOfficeDate();

        filterDto.EndDateCurrentTermOfOfficeDate = currentTermOfOfficeDate.EndDate;

        var newAndReElections = await GetNewAndReelectionMemberships(filterDto, electionTypeListFuture, sender);

        var endedMemberships = await GetEndedMemberships(filterDto, electionTypeListPresent, sender);

        var allRecipients = newAndReElections.Concat(endedMemberships).ToList().OrderBy(m => m.Surname).ThenBy(m => m.GivenName);

        var formLetterReportList = new List<FormLetterReportDto>();

        var signaturePictureExists = false;
        var picBase64 = string.Empty;

        if (sender.SignatureFileReference != null)
        {
            using var signatureStream = await _documentServiceInternal.GetDocument(sender.SignatureFileReference.DocumentStorageId);

            if (signatureStream is { CanSeek: true })
            {
                picBase64 = signatureStream.TryGetBuffer(out var buffer)
                    ? Convert.ToBase64String(buffer.Array!, buffer.Offset, buffer.Count)
                    : Convert.ToBase64String(signatureStream.ToArray());

                signaturePictureExists = true;
            }
        }

        foreach (var language in Enum.GetValues<FormLetterLanguage>())
        {
            var currentRecipients = allRecipients.Where(r => r.FormLetterLanguage == language).ToList();

            if (currentRecipients.Count > 0)
            {
                var formLetterReportDto = new FormLetterReportDto
                {
                    FormLetterLanguage = language,
                    NextTermOfOfficeBeginDate = nextTermOfOfficeDate.BeginDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    NextTermOfOfficeEndDate = nextTermOfOfficeDate.EndDate?.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) ?? "",
                    TermOfOfficeEndDate = currentTermOfOfficeDate.EndDate?.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) ?? "",
                    HasSignature = signaturePictureExists,
                    SenderSignature = picBase64,
                    SenderOffice = GetSenderLanguageText(language, sender.Office?.DescriptionDe, sender.Office?.DescriptionFr, sender.Office?.DescriptionIt, sender.Office?.DescriptionRm),
                    SenderName = sender.GivenName + " " + sender.Surname,
                    SenderStreet = GetSenderLanguageText(language, sender.StreetGerman, sender.StreetFrench, sender.StreetItalian, sender.StreetRomansh),
                    SenderZip = sender.Zip,
                    SenderCity = GetSenderLanguageText(language, sender.CityGerman, sender.CityFrench, sender.CityItalian, sender.CityRomansh),
                    SenderPhone = sender.Phone,
                    SenderEmail = sender.Email,
                    SenderWebsite = sender.Website,
                    TemplateName = $"{template}_{language}",
                    Memberships = currentRecipients,
                };

                formLetterReportList.Add(formLetterReportDto);
            }
        }

        return formLetterReportList;
    }

    private async Task<List<FormLetterMembershipReportDto>> GetNewAndReelectionMemberships(FormLetterFilterParameters filterDto, List<Guid> electionTypeListFuture, FormLetterSender sender)
    {
        var formLetterDate = filterDto.FormLetterDate != null ? (DateOnly)filterDto.FormLetterDate! : DateOnly.FromDateTime(DateTime.Today);
        var dateLetter = FormatMonthAndYear(formLetterDate);

        var allValidMemberships = await _generalElectionCommitteeRepository.GetAllForFormLetter(filterDto, electionTypeListFuture);

        var newAndReElections = allValidMemberships
            .SelectMany(c => c.MembershipCandidates
                .Where(m => m.PersonId != null && m.Person!.CorrespondenceAddressId != null && m.IsSelected)
                .Select(m => MapToFormLetterMembershipDto(
                    formLetterType: m.ElectionTypeId == ElectionType.NewElectionGuid ? FormLetterType.NewElection : FormLetterType.ReElection,
                    committeeId: c.CommitteeId,
                    committeeNames: (c.DescriptionGerman, c.DescriptionFrench, c.DescriptionItalian, c.DescriptionRomansh),
                    selfOrganized: c.SelfOrganized != null && (bool)c.SelfOrganized,
                    dateLetter: dateLetter,
                    sender: sender,
                    person: m.Person!,
                    function: m.Function!)))
            .ToList();

        return newAndReElections;
    }

    private async Task<List<FormLetterMembershipReportDto>> GetEndedMemberships(FormLetterFilterParameters filterDto, List<Guid> electionTypeListPresent, FormLetterSender sender)
    {
        var dateLetter = FormatMonthAndYear(DateOnly.FromDateTime(DateTime.Now));

        var allEndedMemberships = await _committeeRepository.GetAllForFormLetter(filterDto, electionTypeListPresent);

        var endedMemberships = allEndedMemberships
            .SelectMany(c => c.Memberships
                .Select(m => MapToFormLetterMembershipDto(
                    formLetterType: m.ElectionTypeId == ElectionType.CommitteeDissolutionWithdrawalGuid ? FormLetterType.CommitteeDissolutionWithdrawal :
                        m.ElectionTypeId == ElectionType.RetirementGuid ? FormLetterType.Retire :
                        m.ElectionTypeId == ElectionType.MaximumMembershipDurationGuid ? FormLetterType.MaximumMembershipDuration : FormLetterType.OtherRetirement,
                    committeeId: c.Id,
                    committeeNames: (c.DescriptionGerman, c.DescriptionFrench, c.DescriptionItalian, c.DescriptionRomansh),
                    selfOrganized: c.SelfOrganized != null && (bool)c.SelfOrganized,
                    dateLetter: dateLetter,
                    sender: sender,
                    person: m.Person!,
                    function: m.Function!,
                    endDate: m.EndDate)))
            .ToList();

        return endedMemberships;
    }

    private static (string De, string Fr, string It, string Rm) FormatMonthAndYear(DateOnly date)
    {
        return (
            date.ToString("MMMM yyyy", new CultureInfo("de-CH")),
            date.ToString("MMMM yyyy", new CultureInfo("fr-CH")),
            date.ToString("MMMM yyyy", new CultureInfo("it-CH")),
            date.ToString("MMMM yyyy", new CultureInfo("rm-CH"))
        );
    }

    private static string GetSenderLanguageText(FormLetterLanguage language, string? germanText, string? frenchText, string? italianText, string? romanshText)
    {
        if (language == FormLetterLanguage.German)
        {
            return germanText ?? string.Empty;
        }

        if (language == FormLetterLanguage.French)
        {
            return frenchText ?? string.Empty;
        }

        if (language == FormLetterLanguage.Italian)
        {
            return italianText ?? string.Empty;
        }

        if (language == FormLetterLanguage.Romansh && !string.IsNullOrWhiteSpace(romanshText))
        {
            return romanshText;
        }

        return germanText ?? string.Empty;
    }

    private static string GetFileNameLanguageExtension(FormLetterLanguage language)
    {
        if (language == FormLetterLanguage.German)
        {
            return "DE";
        }

        if (language == FormLetterLanguage.French)
        {
            return "FR";
        }

        if (language == FormLetterLanguage.Italian)
        {
            return "IT";
        }

        if (language == FormLetterLanguage.Romansh)
        {
            return "RM";
        }

        return "DE";
    }

    private static FormLetterMembershipReportDto MapToFormLetterMembershipDto(
        FormLetterType formLetterType,
        Guid committeeId,
        (string De, string? Fr, string? It, string? Rm) committeeNames,
        bool selfOrganized,
        (string De, string Fr, string It, string Rm) dateLetter,
        FormLetterSender sender,
        Person person,
        Function function,
        DateOnly? endDate = null)
    {
        return new FormLetterMembershipReportDto
        {
            // as the zip file is always named in german, we also name all the committee files with the german name!
            FileName = committeeNames.De,
            FormLetterType = formLetterType,
            FormLetterLanguage = GetFormLetterLanguage(),
            SenderDepartment = GetText(sender.Department!.DescriptionDe, sender.Department!.DescriptionFr, sender.Department!.DescriptionIt, sender.Department!.DescriptionRm),
            SenderOffice = GetText(sender.Office?.DescriptionDe, sender.Office?.DescriptionFr, sender.Office?.DescriptionIt, sender.Office?.DescriptionRm),
            SenderOfficeShort = GetText(sender.Office?.TextDe, sender.Office?.TextFr, sender.Office?.TextIt, sender.Office?.TextRm),
            SenderName = sender.GivenName + " " + sender.Surname,
            SenderFunction = GetText(sender.SenderFunction!.DescriptionDe, sender.SenderFunction!.DescriptionFr, sender.SenderFunction!.DescriptionIt, sender.SenderFunction!.DescriptionRm),
            SenderStreet = GetText(sender.StreetGerman, sender.StreetFrench, sender.StreetItalian, sender.StreetRomansh),
            SenderZip = sender.Zip,
            SenderCity = GetText(sender.CityGerman, sender.CityFrench, sender.CityItalian, sender.CityRomansh),
            Subject = GetText(GeneralElectionTextGerman, GeneralElectionTextFrench, GeneralElectionTextItalian, GeneralElectionTextRomansh),
            DateLetter = GetText(dateLetter.De, dateLetter.Fr, dateLetter.It, dateLetter.Rm),
            CommitteeId = committeeId,
            CommitteeName = GetText(committeeNames.De, committeeNames.Fr, committeeNames.It, committeeNames.Rm),
            SelfOrganized = selfOrganized,
            CorrespondenceLanguageId = person.CorrespondenceLanguageId,
            Function = person.GenderId == Gender.MaleGuid
                ? GetText(function.TextDe, function.TextFr, function.TextIt, function.TextRm)
                : GetText(function.TextFemaleDe, function.TextFemaleFr, function.TextFemaleIt, function.TextFemaleRm),
            Salutation = GetText(person.Salutation?.TextDe, person.Salutation?.TextFr, person.Salutation?.TextIt, person.Salutation?.TextRm),
            SalutationText = person.SalutationText ?? string.Empty,
            GivenName = person.GivenName,
            Surname = person.Surname,
            CompanyName = person.CorrespondenceAddress!.CompanyName ?? string.Empty,
            Street = person.CorrespondenceAddress!.Street ?? string.Empty,
            PoBox = person.CorrespondenceAddress!.PoBox ?? string.Empty,
            Zip = person.CorrespondenceAddress!.Zip ?? string.Empty,
            City = person.CorrespondenceAddress!.City ?? string.Empty,
            Country = person.CorrespondenceAddress!.Country == null
                ? string.Empty
                : person.CorrespondenceAddress.Country.TextDe == "CH"
                    ? string.Empty
                    : GetText(person.CorrespondenceAddress.Country.DescriptionDe, person.CorrespondenceAddress.Country.DescriptionFr, person.CorrespondenceAddress.Country.DescriptionIt, person.CorrespondenceAddress.Country.DescriptionRm),
            EndDate = endDate,
        };

        FormLetterLanguage GetFormLetterLanguage()
        {
            if (person.CorrespondenceLanguageId == Language.GermanGuid)
            {
                return FormLetterLanguage.German;
            }

            if (person.CorrespondenceLanguageId == Language.FrenchGuid)
            {
                return FormLetterLanguage.French;
            }

            if (person.CorrespondenceLanguageId == Language.ItalianGuid)
            {
                return FormLetterLanguage.Italian;
            }

            if (person.CorrespondenceLanguageId == Language.RomanshGuid)
            {
                return FormLetterLanguage.Romansh;
            }

            return FormLetterLanguage.German;
        }

        string GetText(string? germanText, string? frenchText, string? italianText, string? romanshText)
        {
            if (person.CorrespondenceLanguageId == Language.GermanGuid)
            {
                return germanText ?? string.Empty;
            }

            if (person.CorrespondenceLanguageId == Language.FrenchGuid)
            {
                return frenchText ?? string.Empty;
            }

            if (person.CorrespondenceLanguageId == Language.ItalianGuid)
            {
                return italianText ?? string.Empty;
            }

            if (person.CorrespondenceLanguageId == Language.RomanshGuid && !string.IsNullOrWhiteSpace(romanshText))
            {
                return romanshText;
            }

            return germanText ?? string.Empty;
        }
    }
}
