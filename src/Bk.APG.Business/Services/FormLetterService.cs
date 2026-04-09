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
    private readonly Swiss.FCh.DocumentService.Client.IDocumentService _documentService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly IDocumentService _documentServiceInternal;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IFormLetterSenderRepository _formLetterSenderRepository;
    private readonly ILogger<ReportService> _logger;

    public FormLetterService(
        Swiss.FCh.DocumentService.Client.IDocumentService documentService,
        ICultureService cultureService,
        ITermOfOfficeDateService termOfOfficeDateService,
        IDocumentService documentServiceInternal,
        ICommitteeRepository committeeRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        IMasterDataRepository masterDataRepository,
        IGeneralMeasureRepository generalMeasureRepository,
        IFormLetterSenderRepository formLetterSenderRepository,
        ILogger<ReportService> logger)
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
        var maxFileNameLength = 150;

        if (reportDto.Memberships != null && filterDto.ExportType == "single")
        {
            var template = "FormLetterGeneralElection";

            var allMemberships = reportDto.Memberships.ToList();

            var grouped = reportDto.Memberships.GroupBy(m => m.CommitteeId).ToList();

            using var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true);
            foreach (var group in grouped)
            {
                var currentCommitteeId = group.Key;

                var first = group.First();

                var reducedDataDto = reportDto;
                reducedDataDto.Memberships = allMemberships;

                var reducedMembersDto = reducedDataDto.Memberships.Where(m => m.CommitteeId == currentCommitteeId).OrderBy(m => m.Surname).ThenBy(m => m.GivenName);
                reducedDataDto.Memberships = reducedMembersDto;

                var fileName = first.FileName.Replace(" ", "_", StringComparison.InvariantCultureIgnoreCase);

                if (fileName.Length > maxFileNameLength)
                {
                    fileName = fileName.Substring(0, maxFileNameLength);
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

        var template = "FormLetterGeneralElection";

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

        if (sender != null && nextTermOfOfficeDate != null && currentTermOfOfficeDate != null)
        {
            var signaturePictureExists = false;
            var picBase64 = string.Empty;

            if (sender.SignatureFileReference != null)
            {
                using var signatureStream = await _documentServiceInternal.GetDocument(sender.SignatureFileReference.DocumentStorageId ?? string.Empty);

                if (signatureStream != null && signatureStream.CanSeek)
                {
                    signatureStream.Position = 0;
                    picBase64 = Convert.ToBase64String(signatureStream.ToArray());
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
                SenderOffice = sender.Office?.DescriptionDe,
                SenderOfficeShort = sender.Office?.TextDe,
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
        else
        {
            return new FormLetterReportDto();
        }
    }

    private async Task<List<FormLetterMembershipReportDto>> GetNewAndReelectionMemberships(FormLetterFilterParameters filterDto, List<Guid> electionTypeListFuture, FormLetterSender sender)
    {
        var generalElectionTextGerman = "Gesamterneuerungswahl";
        var generalElectionTextFrench = "Renouvellement intégral";
        var generalElectionTextItalian = "Rinnovo integrale";
        var generalElectionTextRomansh = "Renovaziun totala";

        var formLetterDate = filterDto.FormLetterDate != null ? (DateOnly)filterDto.FormLetterDate! : DateOnly.FromDateTime(DateTime.Today);

        var currentMonthAndYearGerman = formLetterDate.ToString("MMMM yyyy", new CultureInfo("de-CH")) ?? "";
        var currentMonthAndYearFrench = formLetterDate.ToString("MMMM yyyy", new CultureInfo("fr-CH")) ?? "";
        var currentMonthAndYearItalian = formLetterDate.ToString("MMMM yyyy", new CultureInfo("it-CH")) ?? "";
        var currentMonthAndYearRomansh = formLetterDate.ToString("MMMM yyyy", new CultureInfo("rm-CH")) ?? "";

        var allValidMemberships = await _generalElectionCommitteeRepository.GetAllForFormLetter(filterDto, electionTypeListFuture);

        var newAndReElections = allValidMemberships
            .SelectMany(c => c.MembershipCandidates.Where(m => m.PersonId != null && m.Person!.CorrespondenceAddressId != null && m.IsSelected).Select(m => new FormLetterMembershipReportDto
            {
                // as the zip file is always named in german, we also name all the committee files with the german name!
                FileName = c.DescriptionGerman,
                FormLetterType = m.ElectionTypeId == ElectionType.NewElectionGuid ? FormLetterType.NewElection : FormLetterType.ReElection,
                FormLetterLanguage = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? FormLetterLanguage.German :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? FormLetterLanguage.French :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? FormLetterLanguage.Italian : FormLetterLanguage.Romansh,
                SenderDepartment = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Department!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Department!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Department!.DescriptionIt : sender.Department!.DescriptionRm,
                SenderOffice = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.DescriptionIt : sender.Office?.DescriptionRm,
                SenderOfficeShort = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.TextIt : sender.Office?.TextRm,
                SenderName = sender.GivenName + " " + sender.Surname,
                SenderFunction = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.SenderFunction!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.SenderFunction!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.SenderFunction!.DescriptionIt : sender.SenderFunction!.DescriptionRm,
                SenderStreet = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.StreetGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.StreetFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.StreetItalian : sender.StreetRomansh,
                SenderZip = sender.Zip,
                SenderCity = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.CityGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.CityFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.CityItalian : sender.CityRomansh,
                Subject = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? generalElectionTextGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? generalElectionTextFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? generalElectionTextItalian : generalElectionTextRomansh,
                DateLetter = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? currentMonthAndYearGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? currentMonthAndYearFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? currentMonthAndYearItalian : currentMonthAndYearRomansh,
                CommitteeId = c.CommitteeId,
                CommitteeName = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? c.DescriptionGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? c.DescriptionFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? c.DescriptionItalian : c.DescriptionRomansh,
                CorrespondenceLanguageId = m.Person != null ? m.Person!.CorrespondenceLanguageId : Guid.Empty,
                Function = m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleFr :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextIt :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleIt :
                m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextRm : string.Empty,
                Salutation = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? m.Person!.Salutation!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? m.Person!.Salutation!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? m.Person!.Salutation!.TextIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid ? m.Person!.Salutation!.TextRm : string.Empty,
                SalutationText = m.Person!.SalutationText ?? string.Empty,
                GivenName = m.Person!.GivenName ?? string.Empty,
                Surname = m.Person!.Surname ?? string.Empty,
                CompanyName = m.Person!.CorrespondenceAddress!.CompanyName ?? string.Empty,
                Street = m.Person!.CorrespondenceAddress.Street ?? string.Empty,
                PoBox = m.Person!.CorrespondenceAddress.PoBox ?? string.Empty,
                Zip = m.Person!.CorrespondenceAddress!.Zip ?? string.Empty,
                City = m.Person!.CorrespondenceAddress!.City ?? string.Empty,
                Country = m.Person!.CorrespondenceAddress.Country == null ? string.Empty : m.Person!.CorrespondenceAddress.Country!.TextDe == "CH" ? string.Empty :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionRm : string.Empty,
            }))
            .ToList();

        return newAndReElections;
    }

    private async Task<List<FormLetterMembershipReportDto>> GetEndedMemberships(FormLetterFilterParameters filterDto, List<Guid> electionTypeListPresent, FormLetterSender sender)
    {
        var generalElectionTextGerman = "Gesamterneuerungswahl";
        var generalElectionTextFrench = "Renouvellement intégral";
        var generalElectionTextItalian = "Rinnovo integrale";
        var generalElectionTextRomansh = "Renovaziun totala";

        var currentMonthAndYearGerman = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("de-CH")) ?? "";
        var currentMonthAndYearFrench = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("fr-CH")) ?? "";
        var currentMonthAndYearItalian = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("it-CH")) ?? "";
        var currentMonthAndYearRomansh = DateTime.Now.ToString("MMMM yyyy", new CultureInfo("rm-CH")) ?? "";

        var allEndedMemberships = await _committeeRepository.GetAllForFormLetter(filterDto, electionTypeListPresent);

        var endedMemberships = allEndedMemberships
            .SelectMany(c => c.Memberships.Select(m => new FormLetterMembershipReportDto
            {
                // as the zip file is always named in german, we also name all the committee files with the german name!
                FileName = c.DescriptionGerman,
                FormLetterType = m.ElectionTypeId == ElectionType.RetirementGuid ? FormLetterType.Retire : m.ElectionTypeId == ElectionType.MaximumMembershipDurationGuid ? FormLetterType.MaximumMembershipDuration : FormLetterType.OtherRetirement,
                FormLetterLanguage = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? FormLetterLanguage.German :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? FormLetterLanguage.French :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? FormLetterLanguage.Italian : FormLetterLanguage.Romansh,
                SenderDepartment = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Department!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Department!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Department!.DescriptionIt : sender.Department!.DescriptionRm,
                SenderOffice = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.DescriptionIt : sender.Office?.DescriptionRm,
                SenderOfficeShort = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.Office?.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.Office?.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.Office?.TextIt : sender.Office?.TextRm,
                SenderName = sender.GivenName + " " + sender.Surname,
                SenderFunction = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.SenderFunction!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.SenderFunction!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.SenderFunction!.DescriptionIt : sender.SenderFunction!.DescriptionRm,
                SenderStreet = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.StreetGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.StreetFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.StreetItalian : sender.StreetRomansh,
                SenderZip = sender.Zip,
                SenderCity = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? sender.CityGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? sender.CityFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? sender.CityItalian : sender.CityRomansh,
                Subject = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? generalElectionTextGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? generalElectionTextFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? generalElectionTextItalian : generalElectionTextRomansh,
                DateLetter = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? currentMonthAndYearGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? currentMonthAndYearFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? currentMonthAndYearItalian : currentMonthAndYearRomansh,
                CommitteeId = c.Id,
                CommitteeName = m.Person!.CorrespondenceLanguageId == Language.GermanGuid ? c.DescriptionGerman :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid ? c.DescriptionFrench :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid ? c.DescriptionItalian : c.DescriptionRomansh,
                CorrespondenceLanguageId = m.Person != null ? m.Person!.CorrespondenceLanguageId : Guid.Empty,
                Function = m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleFr :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextIt :
                m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.GenderId == Gender.FemaleGuid ? m.Function!.TextFemaleIt :
                m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.GenderId == Gender.MaleGuid ? m.Function!.TextRm : string.Empty,
                Salutation = m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.Salutation != null ? m.Person!.Salutation!.TextRm : string.Empty,
                SalutationText = m.Person!.SalutationText ?? string.Empty,
                GivenName = m.Person!.GivenName ?? string.Empty,
                Surname = m.Person!.Surname ?? string.Empty,
                CompanyName = m.Person!.CorrespondenceAddress!.CompanyName ?? string.Empty,
                Street = m.Person!.CorrespondenceAddress.Street ?? string.Empty,
                PoBox = m.Person!.CorrespondenceAddress.PoBox ?? string.Empty,
                Zip = m.Person!.CorrespondenceAddress!.Zip ?? string.Empty,
                City = m.Person!.CorrespondenceAddress!.City ?? string.Empty,
                Country = m.Person!.CorrespondenceAddress.Country == null ? string.Empty : m.Person!.CorrespondenceAddress.Country!.TextDe == "CH" ? string.Empty :
                    m.Person!.CorrespondenceLanguageId == Language.GermanGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionDe :
                    m.Person!.CorrespondenceLanguageId == Language.FrenchGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionFr :
                    m.Person!.CorrespondenceLanguageId == Language.ItalianGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionIt :
                    m.Person!.CorrespondenceLanguageId == Language.RomanshGuid && m.Person!.CorrespondenceAddress!.Country != null ? m.Person!.CorrespondenceAddress!.Country!.DescriptionRm : string.Empty,
            }))
            .ToList();

        return endedMemberships;
    }
}
