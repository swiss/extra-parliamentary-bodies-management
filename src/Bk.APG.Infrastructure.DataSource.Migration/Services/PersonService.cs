using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.Infrastructure.DataSource.Migration.Mapping;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Person = Bk.APG.Infrastructure.DataSource.Migration.Models.Person;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class PersonService : IPersonService
{
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly ISalutationGeneratorService _salutationGeneratorService;
    private readonly IPersonRepository _personRepository;
    private readonly IOccupationRepository _occupationRepository;
    private readonly ILogger<PersonService> _logger;

    public PersonService(ISalutationGeneratorService salutationGeneratorService, IPersonRepository personRepository, IOccupationRepository occupationRepository, ILogger<PersonService> logger)
    {
        _salutationGeneratorService = salutationGeneratorService;
        _personRepository = personRepository;
        _occupationRepository = occupationRepository;
        _logger = logger;
    }

    public async Task MigratePersons(SqlConnection connection)
    {
        _logger.LogInformation("Start migrating persons.");

        var commandText = "SELECT p.*," +
                          "(SELECT GUID FROM [PersonAddressMapping] pam WHERE pam.PersonID = p.ID and pam.PrivateAddress = 1) as PrivatAdresse," +
                          "(SELECT GUID FROM [PersonAddressMapping] pam WHERE pam.PersonID = p.ID and pam.PrivateAddress = 0) as GeschäftsAdresse," +
                          "(SELECT GUID FROM [PersonAddressMapping] pam WHERE pam.PersonID = p.ID and pam.CommunicationAddress = 1) as AktiveAdresse, " +
                          "Case when GeschlechtId = 0 then '72DCBA6D-C45E-489E-AC42-BB61359668B3'" +
                          "when GeschlechtId = 1 then '83A925B0-DB23-42C8-98B7-DAAF5BE02A4D'" +
                          "else '1E7B2341-00CF-4959-BCEF-EAB8E2C9996C' end as AnredeGuid," +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Sprache WHERE Id = SpracheId ) as SpracheGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Sprache WHERE Id = KorrespSpracheId ) as KorrespondenzSpracheGuid, + " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Geschlecht WHERE Id = GeschlechtId ) as GeschlechtGuid, " +
                          "(select count(*) from Interessenbindung where PersId = p.Id) as AnzahlInteressen " +
                          "FROM Person p ";
        // There is one invalid record without name/vorname. Will be logged in the source and skipped.
        // where name is not null and vorname is not null";

        using var command = new SqlCommand(commandText, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var person = new Person { Name = string.Empty, Vorname = string.Empty };

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Id")).ToString(), out var validNumber))
            {
                person.Id = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Jahrgang")).ToString(), out validNumber))
            {
                person.Jahrgang = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("AnzahlInteressen")).ToString(), out validNumber))
            {
                person.AnzahlInteressen = validNumber;
            }

            person.Name = reader.GetValue(reader.GetOrdinal("Name")).ToString() ?? string.Empty;
            person.Vorname = reader.GetValue(reader.GetOrdinal("Vorname")).ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(person.Vorname) && string.IsNullOrWhiteSpace(person.Name))
            {
                _logger.LogInformation("Invalid person without surname/givenname, record skipped. Check record with id': {PersonId}", person.Id);
                continue;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("InsertDate")).ToString(), out var validDate))
            {
                person.InsertDate = validDate.ToUniversalTime();
            }
            else
            {
                person.InsertDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("UpdateDate")).ToString(), out validDate))
            {
                person.UpdateDate = validDate.ToUniversalTime();
            }
            else
            {
                person.UpdateDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Histo")).ToString(), out validDate))
            {
                person.Histo = validDate.ToUniversalTime();
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("Guid")).ToString(), out var validGuid))
            {
                person.Guid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("PrivatAdresse")).ToString(), out validGuid))
            {
                person.PrivatAdresseGuid = validGuid;
            }
            else
            {
                person.PrivatAdresseGuid = null;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GeschäftsAdresse")).ToString(), out validGuid))
            {
                person.GeschäftsAdresseGuid = validGuid;
            }
            else
            {
                person.GeschäftsAdresseGuid = null;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("AktiveAdresse")).ToString(), out validGuid))
            {
                person.AktiveAdresseGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GeschlechtGuid")).ToString(), out validGuid))
            {
                person.GeschlechtGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("AnredeGuid")).ToString(), out validGuid))
            {
                person.AnredeGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("SpracheGuid")).ToString(), out validGuid))
            {
                person.SpracheGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("KorrespondenzSpracheGuid")).ToString(), out validGuid))
            {
                person.KorrespondenzSpracheGuid = validGuid;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("MitgliedBV")).ToString(), out var validBoolean))
            {
                person.MitgliedBV = validBoolean;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("AngestelltBD")).ToString(), out validBoolean))
            {
                person.AngestelltBD = validBoolean;
            }

            person.Briefanrede = reader.GetValue(reader.GetOrdinal("Briefanrede")).ToString();
            person.TitelText = reader.GetValue(reader.GetOrdinal("TitelText")).ToString();
            person.Beruf = reader.GetValue(reader.GetOrdinal("Beruf")).ToString();
            person.Organisation = reader.GetValue(reader.GetOrdinal("Organisation")).ToString();
            person.Funktion = reader.GetValue(reader.GetOrdinal("Funktion")).ToString();
            person.AnredeTitel = reader.GetValue(reader.GetOrdinal("AnredeTitel")).ToString();
            person.BemerkungPersonendaten = reader.GetValue(reader.GetOrdinal("BemerkungPersonendaten")).ToString();
            person.BemerkungInteressenbindung = reader.GetValue(reader.GetOrdinal("BemerkungInteressenbindung")).ToString();
            person.BemerkungMitgliedschaft = reader.GetValue(reader.GetOrdinal("BemerkungMitgliedschaft")).ToString();
            person.BemerkungPersonendatenAdmin = reader.GetValue(reader.GetOrdinal("BemerkungPersonendatenAdmin")).ToString();
            person.BemerkungInteressenbindungAdmin = reader.GetValue(reader.GetOrdinal("BemerkungInteressenbindungAdmin")).ToString();
            person.BemerkungMitgliedschaftAdmin = reader.GetValue(reader.GetOrdinal("BemerkungMitgliedschaftAdmin")).ToString();
            person.Interessenbindung_Alt = reader.GetValue(reader.GetOrdinal("Interessenbindung_Alt")).ToString();
            person.LastupdateUser = reader.GetValue(reader.GetOrdinal("LastupdateUser")).ToString();

            var mappedPerson = MigrationMapping.ToPerson(person);

            // makes separated occupation handling easier
            if (!string.IsNullOrWhiteSpace(mappedPerson.Occupation))
            {
                mappedPerson.Occupation = mappedPerson.Occupation.Replace(";", ",");
                var splittedOccupation = mappedPerson.Occupation!.Split(",");

                foreach (var occupation in splittedOccupation)
                {
                    var hit = _occupationRepository.GetBySearchStringForMigration(occupation);

                    if (hit != null)
                    {
                        mappedPerson.Occupations.Add(hit);
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(mappedPerson.SalutationText))
            {
                mappedPerson.SalutationText = await _salutationGeneratorService.CreateSalutationTextForPerson(mappedPerson.GenderId, mappedPerson.CorrespondenceLanguageId, mappedPerson.Surname, mappedPerson.Title);
            }

            _personRepository.CreateForMigration(mappedPerson);
        }

        _logger.LogInformation("Persons migrated completely.");
    }
}
