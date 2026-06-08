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

        var reportDto = await FillFormLetterDto(filterDto);

        var zipStream = new MemoryStream();
        const int maxFileNameLength = 150;

        if (reportDto.Memberships != null && filterDto.ExportType == "single")
        {
            const string template = "FormLetterGeneralElection";

            var allMemberships = reportDto.Memberships.ToList();

            var grouped = reportDto.Memberships.GroupBy(m => m.CommitteeId).ToList();

            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true);
            foreach (var group in grouped)
            {
                var currentCommitteeId = group.Key;

                var first = group.First();

                var reducedMembersDto = allMemberships
                    .Where(m => m.CommitteeId == currentCommitteeId)
                    .OrderBy(m => m.Surname)
                    .ThenBy(m => m.GivenName)
                    .ToList();

                var reducedDataDto = new FormLetterReportDto
                {
                    SenderOfficeGerman = reportDto.SenderOfficeGerman,
                    SenderOfficeFrench = reportDto.SenderOfficeFrench,
                    SenderOfficeItalian = reportDto.SenderOfficeItalian,
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
                };

                var fileName = first.FileName.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);

                if (fileName.Length > maxFileNameLength)
                {
                    fileName = fileName[..maxFileNameLength];
                }

                if (filterDto.ExportFileType == "word")
                {
                    var zipFile = zip.CreateEntry($"{fileName}.docx", CompressionLevel.Fastest);
                    await using var zipFileStream = zipFile.Open();

                    await using var documentStream = (MemoryStream)await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", reducedDataDto, "formLetter");
                    await documentStream.CopyToAsync(zipFileStream);
                }
                else
                {
                    var zipFile = zip.CreateEntry($"{fileName}.pdf", CompressionLevel.Fastest);
                    await using var zipFileStream = zipFile.Open();

                    await using var documentStream = (MemoryStream)await _documentService.CreatePdfFromTemplate($"Templates/{template}.docx", reducedDataDto, "formLetter");
                    await documentStream.CopyToAsync(zipFileStream);
                }
            }
        }

        zipStream.Position = 0;
        return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.FormLetterCompleteExport_Filename}.zip", zipStream);
    }

    public async Task<(string fileName, Stream content)> CreateFormLetterSingleDocument(FormLetterFilterParameters filterDto)
    {
        ArgumentNullException.ThrowIfNull(filterDto);

        const string template = "FormLetterGeneralElection";

        var reportDto = await FillFormLetterDto(filterDto);

        if (filterDto.ExportFileType == "word")
        {
            await using var documentStream = (MemoryStream)await _documentService.CreateWordFromTemplate($"Templates/{template}.docx", reportDto, "formLetter");

            var stream = new MemoryStream();
            await documentStream.CopyToAsync(stream);
            stream.Position = 0;

            return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.FormLetterCompleteExport_Filename}.docx", stream);
        }
        else
        {
            await using var documentStream = (MemoryStream)await _documentService.CreatePdfFromTemplate($"Templates/{template}.docx", reportDto, "formLetter");

            var stream = new MemoryStream();
            await documentStream.CopyToAsync(stream);
            stream.Position = 0;

            return ($"{DateTime.UtcNow.ToLocalTime():yyyyMMdd}_{BusinessTexts.FormLetterCompleteExport_Filename}.pdf", stream);
        }
    }

    private async Task<FormLetterReportDto> FillFormLetterDto(FormLetterFilterParameters filterDto)
    {
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

        var formLetterReportDto = new FormLetterReportDto
        {
            NextTermOfOfficeBeginDate = nextTermOfOfficeDate.BeginDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
            NextTermOfOfficeEndDate = nextTermOfOfficeDate.EndDate?.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) ?? "",
            TermOfOfficeEndDate = currentTermOfOfficeDate.EndDate?.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) ?? "",
            Memberships = allRecipients,
            HasSignature = signaturePictureExists,
            SenderSignature = picBase64,
            SenderOfficeGerman = sender.Office?.DescriptionDe,
            SenderOfficeFrench = sender.Office?.DescriptionFr,
            SenderOfficeItalian = sender.Office?.DescriptionIt,
            SenderName = sender.GivenName + " " + sender.Surname,
            SenderStreet = sender.StreetGerman,
            SenderZip = sender.Zip,
            SenderCity = sender.CityGerman,
            SenderPhone = sender.Phone,
            SenderEmail = sender.Email,
            SenderWebsite = sender.Website,
        };

        return formLetterReportDto;
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
                    formLetterType: m.ElectionTypeId == ElectionType.RetirementGuid ? FormLetterType.Retire :
                        m.ElectionTypeId == ElectionType.MaximumMembershipDurationGuid ? FormLetterType.MaximumMembershipDuration : FormLetterType.OtherRetirement,
                    committeeId: c.Id,
                    committeeNames: (c.DescriptionGerman, c.DescriptionFrench, c.DescriptionItalian, c.DescriptionRomansh),
                    dateLetter: dateLetter,
                    sender: sender,
                    person: m.Person!,
                    function: m.Function!)))
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

    private static FormLetterMembershipReportDto MapToFormLetterMembershipDto(
        FormLetterType formLetterType,
        Guid committeeId,
        (string De, string? Fr, string? It, string? Rm) committeeNames,
        (string De, string Fr, string It, string Rm) dateLetter,
        FormLetterSender sender,
        Person person,
        Function function)
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
            CorrespondenceLanguageId = person.CorrespondenceLanguageId,
            Function = person.GenderId == Gender.MaleGuid
                ? GetText(function.TextDe, function.TextFr, function.TextIt, function.TextRm)
                : GetText(function.TextFemaleDe, function.TextFemaleFr, function.TextFemaleIt, function.TextFemaleRm),
            Salutation = GetText(person.Salutation?.TextDe, person.Salutation?.TextFr, person.Salutation?.TextIt, person.Salutation?.TextRm),
            SalutationText = person.SalutationText ?? string.Empty,
            GivenName = person.GivenName,
            Surname = person.Surname,
            CompanyName = person.CorrespondenceAddress!.CompanyName ?? string.Empty,
            Street = person.CorrespondenceAddress.Street ?? string.Empty,
            PoBox = person.CorrespondenceAddress.PoBox ?? string.Empty,
            Zip = person.CorrespondenceAddress!.Zip ?? string.Empty,
            City = person.CorrespondenceAddress!.City ?? string.Empty,
            Country = person.CorrespondenceAddress.Country == null
                ? string.Empty
                : person.CorrespondenceAddress.Country!.TextDe == "CH"
                    ? string.Empty
                    : GetText(person.CorrespondenceAddress?.Country?.DescriptionDe, person.CorrespondenceAddress?.Country?.DescriptionFr, person.CorrespondenceAddress?.Country?.DescriptionIt, person.CorrespondenceAddress?.Country?.DescriptionRm),
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

            if (person.CorrespondenceLanguageId == Language.RomanshGuid)
            {
                return romanshText ?? string.Empty;
            }

            return germanText ?? string.Empty;
        }
    }
}
