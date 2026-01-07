using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.Infrastructure.DataSource.Migration.Mapping;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class AppointmentDecisionService : IAppointmentDecisionService
{
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly IDocumentService _documentService;
    private readonly IAppointmentDecisionRepository _appointmentDecisionRepository;
    private readonly IDocumentStorageRepository _documentStorageRepository;
    private readonly ILogger<AppointmentDecisionService> _logger;

    public AppointmentDecisionService(IDocumentService documentService, IAppointmentDecisionRepository appointmentDecisionRepository, IDocumentStorageRepository documentStorageRepository, ILogger<AppointmentDecisionService> logger)
    {
        _documentService = documentService;
        _appointmentDecisionRepository = appointmentDecisionRepository;
        _documentStorageRepository = documentStorageRepository;
        _logger = logger;
    }

    public async Task MigrateAppointmentDecisions(SqlConnection connection, IConfiguration s3Configuration)
    {
        _logger.LogInformation("Start migrating AppointmentDecisions including documents");

        await _documentService.SetupStorage();

        const string commandText = "SELECT j.*," +
                                   "(SELECT Cast(GUID as nvarchar(36)) FROM Gremium WHERE Id = j.GremId ) as GremiumGuid, " +
                                   "(SELECT Cast(GUID as nvarchar(36)) FROM JournalCode WHERE Id = j.CodeId ) as JournalCodeGuid, " +
                                   "(SELECT Cast(GUID as nvarchar(36)) FROM LinkTyp WHERE Id = j.LinkTypId ) as LinkTypGuid " +
                                   "FROM Journal j";

        await using var command = new SqlCommand(commandText, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var journal = new Journal();

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Id")).ToString(), out var validNumber))
            {
                journal.Id = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("LinkTypId")).ToString(), out validNumber))
            {
                journal.LinkTypId = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("CodeId")).ToString(), out validNumber))
            {
                journal.CodeId = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("GremId")).ToString(), out validNumber))
            {
                journal.GremId = validNumber;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Datum")).ToString(), out var validDate))
            {
                journal.Datum = validDate;
            }
            else
            {
                journal.Datum = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("InsertDate")).ToString(), out validDate))
            {
                journal.InsertDate = validDate.ToUniversalTime();
            }
            else
            {
                journal.InsertDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("UpdateDate")).ToString(), out validDate))
            {
                journal.UpdateDate = validDate.ToUniversalTime();
            }
            else
            {
                journal.UpdateDate = _now;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("Guid")).ToString(), out var validGuid))
            {
                journal.Guid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GremiumGuid")).ToString(), out validGuid))
            {
                journal.GremiumGuid = validGuid;
            }
            else
            {
                journal.GremiumGuid = Guid.Empty;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("JournalCodeGuid")).ToString(), out validGuid))
            {
                // 6 old types of master data should be mapped to new one (PBI 130326)
                if (validGuid == Guid.Parse("50E24DB2-40D3-4E7A-819F-F89E9973CC38") ||
                    validGuid == Guid.Parse("DFEA0850-EA85-4ECB-9EEE-0CE1A7ED5000") ||
                    validGuid == Guid.Parse("B2BEA37A-95E9-4B89-8C58-212D001A7517") ||
                    validGuid == Guid.Parse("BB2D1F71-4451-4F68-B35F-AE2DB6074A82") ||
                    validGuid == Guid.Parse("A5DF773C-7D44-4CA8-9DB7-7A8C2351A617") ||
                    validGuid == Guid.Parse("000A286F-1C36-4C18-ABE8-DCB679ABD10D"))
                {
                    validGuid = Guid.Parse("03043662-CAA9-40EC-AB77-D8F2825EB775");
                }

                journal.JournalCodeGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("LinkTypGuid")).ToString(), out validGuid))
            {
                // Hard replace, as defined in one of the PBIs
                if (validGuid == new Guid("E985CC03-51CD-4F8E-9189-50CFF3F2C06E"))
                {
                    validGuid = new Guid("3E0016AE-13DB-4A1F-9E26-ADA79D93834E");
                }

                journal.LinkTypGuid = validGuid;
            }

            journal.Kategorie = reader.GetValue(reader.GetOrdinal("Kategorie")).ToString() ?? throw new BusinessValidationException("Empty text for Kategorie");
            journal.Text = reader.GetValue(reader.GetOrdinal("Text")).ToString();
            journal.Link = reader.GetValue(reader.GetOrdinal("Link")).ToString();
            journal.Status = reader.GetValue(reader.GetOrdinal("Status")).ToString() ?? throw new BusinessValidationException("Empty text for Status");
            journal.FileContent = reader["FileContent"] as byte[];
            journal.FileName = reader.GetValue(reader.GetOrdinal("FileName")).ToString();
            // For data migration, we have to guess a language and go for german.
            journal.OriginalLanguageId = Guid.Parse(Language.GermanId);

            var mappedAppointmentType = MigrationMapping.ToAppointmentDecision(journal);

            if (journal.FileContent is not null && journal.FileContent.Length > 0)
            {
                var documentKey = await _documentService.UploadDocument(journal.FileContent);

                var documentStorage = new DocumentStorage
                {
                    // Important, to have matching files and database records, for RHOS, the files have to be uploaded to the right S3 storage. Check the settings, before running the migration!
                    Id = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    CreatedBy = "Migration",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = "Migration",
                    DocumentName = $"{journal.FileName}.pdf",
                    DocumentStorageId = documentKey
                };
                _documentStorageRepository.CreateForMigration(documentStorage);

                mappedAppointmentType.OriginalDocumentId = documentStorage.Id;
                mappedAppointmentType.FileReferenceGermanId = documentStorage.Id;
            }

            _appointmentDecisionRepository.CreateForMigration(mappedAppointmentType);
        }

        _logger.LogInformation("AppointmentDecisions migrated completely.");
    }
}
