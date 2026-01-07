using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Bk.APG.Infrastructure.DataSource.Migration.Mapping;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Bk.APG.Infrastructure.Service.UID.Service;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class InterestService : IInterestService
{
    private readonly IUidService _uidService;
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly IInterestRepository _interestRepository;
    private readonly ILogger<InterestService> _logger;

    public InterestService(IUidService uidService, IInterestRepository interestRepository, ILogger<InterestService> logger)
    {
        _uidService = uidService;
        _interestRepository = interestRepository;
        _logger = logger;
    }

    public void MigrateInterests(SqlConnection connection)
    {
        _logger.LogInformation("Start migrating interests.");

        var commandText = "SELECT i.*, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Person WHERE Id = i.PersId ) as PersonGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM InteressenbindungRechtsform WHERE Id = i.RechtsformId ) as RechtsformGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM InteressenbindungGremium WHERE Id = i.GremiumId ) as GremiumGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM InteressenbindungFunktion WHERE Id = i.FunktionId ) as FunktionGuid " +
                          "FROM Interessenbindung i " +
                          // do not select records from incomplete persons. As soon as these are fixed in the data, the selection will be valid!
                          "WHERE PersId in (SELECT Id FROM Person WHERE Vorname is not null AND Name is not null) " +
                          // and ignore the ones with no interest markings, in all possible forms of writing...
                          "AND Text != 'Aucun lien d’intérêts' AND Text != 'Keine Interssenbindungen' AND Text != 'Keine Interessenbindungen' AND Text != 'Keine Interessensbindungen' ";

        using var command = new SqlCommand(commandText, connection);
        command.CommandTimeout = 60;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var interessenbindung = new Interessenbindung { Text = string.Empty };

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Id")).ToString(), out var validNumber))
            {
                interessenbindung.Id = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("PersId")).ToString(), out validNumber))
            {
                interessenbindung.PersId = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("RechtsformId")).ToString(), out validNumber))
            {
                interessenbindung.RechtsformId = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("GremiumId")).ToString(), out validNumber))
            {
                interessenbindung.GremiumId = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("FunktionId")).ToString(), out validNumber))
            {
                interessenbindung.FunktionId = validNumber;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("InsertDate")).ToString(), out var validDate))
            {
                interessenbindung.InsertDate = validDate.ToUniversalTime();
            }
            else
            {
                interessenbindung.InsertDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("UpdateDate")).ToString(), out validDate))
            {
                interessenbindung.UpdateDate = validDate.ToUniversalTime();
            }
            else
            {
                interessenbindung.UpdateDate = _now;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("PersonGuid")).ToString(), out var validGuid))
            {
                interessenbindung.PersonGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GremiumGuid")).ToString(), out validGuid))
            {
                interessenbindung.GremiumGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("RechtsformGuid")).ToString(), out validGuid))
            {
                interessenbindung.RechtsformGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("FunktionGuid")).ToString(), out validGuid))
            {
                interessenbindung.FunktionGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("Guid")).ToString(), out validGuid))
            {
                interessenbindung.Guid = validGuid;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("GEW")).ToString(), out var validBoolean))
            {
                interessenbindung.GEW = validBoolean;
            }

            interessenbindung.Text = reader.GetValue(reader.GetOrdinal("Text")).ToString() ?? throw new BusinessValidationException("Empty text for Interessenbindung");
            interessenbindung.Bemerkungen = reader.GetValue(reader.GetOrdinal("Bemerkungen")).ToString();
            interessenbindung.LastupdateUser = reader.GetValue(reader.GetOrdinal("LastupdateUser")).ToString();

            var mappedInterest = MigrationMapping.ToInterest(interessenbindung);

            _interestRepository.CreateForMigration(mappedInterest);
        }

        _logger.LogInformation("Interests migrated completely.");
    }

    public void VerifyInterestsWithUid()
    {
        foreach (var interest in _interestRepository.GetAllUnverifiedInterests())
        {
            SearchInterestInUid(interest);
        }
    }

    public void VerifyInterestsForActivePersons(List<Guid> activePersons)
    {
        foreach (var personId in activePersons)
        {
            var interests = _interestRepository.GetAllByPersonIdForUpdate(personId).Result;

            foreach (var interest in interests)
            {
                SearchInterestInUid(interest);
            }
        }
    }

    private void SearchInterestInUid(Interest interest)
    {
        _logger.LogInformation("Searching in uid for {Interest}", interest.Text);
        var result = _uidService.Search(interest.Text!).Result;

        var uidDtos = result.ToList();
        if (uidDtos.Count != 0)
        {
            var bestResult = uidDtos.First();
            _logger.LogInformation("Current legal form: {LegalFormId}", bestResult.LegalFormId);
            interest.UidOrganisationId = bestResult.UidOrganisationId;
            interest.VerifiedSuccessfully = true;
            interest.LegalFormId = bestResult.LegalFormId == Guid.Empty ? null : bestResult.LegalFormId;
            interest.UidOrganisationNameClosestMatch = bestResult.OrganizationName;
            interest.UidMatchQuality = bestResult.MatchQuality;

            _interestRepository.CommitChanges();
        }
        else
        {
            _logger.LogInformation("No hit for {Interest}", interest.Text);
            interest.VerifiedSuccessfully = false;
            _interestRepository.CommitChanges();
        }
    }
}
