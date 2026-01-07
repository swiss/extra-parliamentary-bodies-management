using Bk.APG.Business.Dtos;
using Bk.APG.Business.Repositories;
using Bk.APG.Business.Services;
using Bk.APG.Infrastructure.DataSource.Migration.Mapping;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class ContactPointService : IContactPointService
{
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly IContactPointRepository _contactPointRepository;
    private readonly IPostService _postService;
    private readonly ILogger<PersonService> _logger;

    public ContactPointService(IContactPointRepository contactPointRepository, IPostService postService, ILogger<PersonService> logger)
    {
        _contactPointRepository = contactPointRepository;
        _postService = postService;
        _logger = logger;
    }

    public void MigrateContactPointsForCommittee(SqlConnection connection, Guid committeeId)
    {
        _logger.LogInformation("Start migrating contact points.");

        var commandText = "SELECT s.*, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Gremium WHERE Id = s.GremId ) as GremiumGuid, " +
                          "Case when Datenschutzberater = 1 then 'CC71DE49-4144-41C2-987D-9A5E584F948F' " +
                          "else 'A52067BF-5819-4567-8650-CA042C2FF2C7' end as ContactPointTypeGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Sprache WHERE Id = SpracheId ) as SpracheGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Geschlecht WHERE Id = GeschlechtId ) as GeschlechtGuid, " +
                          "null as AnredeGuid " +
                          "FROM Sekretariat s Inner Join Gremium g on g.Id = s.GremId " +
                          "WHERE g.Guid = '" + committeeId + "';";

        using var command = new SqlCommand(commandText, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var sekretariat = new Sekretariat { NameOrganisation = string.Empty, Vorname = string.Empty };

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Id")).ToString(), out var validNumber))
            {
                sekretariat.Id = validNumber;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("Guid")).ToString(), out var validGuid))
            {
                sekretariat.Guid = validGuid;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("GeschlechtId")).ToString(), out validNumber))
            {
                sekretariat.GeschlechtId = validNumber;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GeschlechtGuid")).ToString(), out validGuid))
            {
                sekretariat.GeschlechtGuid = validGuid;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("SpracheId")).ToString(), out validNumber))
            {
                sekretariat.SpracheId = validNumber;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("SpracheGuid")).ToString(), out validGuid))
            {
                sekretariat.SpracheGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GremiumGuid")).ToString(), out validGuid))
            {
                sekretariat.GremiumGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("ContactPointTypeGuid")).ToString(), out validGuid))
            {
                sekretariat.ContactPointTypeGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("AnredeGuid")).ToString(), out validGuid))
            {
                sekretariat.AnredeGuid = validGuid;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Seit")).ToString(), out var validDate))
            {
                sekretariat.Seit = validDate;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Bis")).ToString(), out validDate))
            {
                sekretariat.Bis = validDate;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("InsertDate")).ToString(), out validDate))
            {
                sekretariat.InsertDate = validDate.ToUniversalTime();
            }
            else
            {
                sekretariat.InsertDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("UpdateDate")).ToString(), out validDate))
            {
                sekretariat.UpdateDate = validDate.ToUniversalTime();
            }
            else
            {
                sekretariat.UpdateDate = _now;
            }

            sekretariat.NameOrganisation = reader.GetValue(reader.GetOrdinal("NameOrganisation")).ToString() ?? string.Empty;
            sekretariat.Firma = reader.GetValue(reader.GetOrdinal("Firma")).ToString() ?? string.Empty;
            sekretariat.Sektion = reader.GetValue(reader.GetOrdinal("Sektion")).ToString() ?? string.Empty;
            sekretariat.Vorname = reader.GetValue(reader.GetOrdinal("Vorname")).ToString() ?? string.Empty;
            sekretariat.Nachname = reader.GetValue(reader.GetOrdinal("Nachname")).ToString() ?? string.Empty;
            sekretariat.AkadTitel = reader.GetValue(reader.GetOrdinal("AkadTitel")).ToString();
            sekretariat.TitelBriefanrede = reader.GetValue(reader.GetOrdinal("TitelBriefanrede")).ToString();
            sekretariat.Briefanrede = reader.GetValue(reader.GetOrdinal("Briefanrede")).ToString();
            sekretariat.Strasse = reader.GetValue(reader.GetOrdinal("Strasse")).ToString();
            sekretariat.Postfach = reader.GetValue(reader.GetOrdinal("Postfach")).ToString();
            sekretariat.Fax = reader.GetValue(reader.GetOrdinal("Fax")).ToString();
            sekretariat.Plz = reader.GetValue(reader.GetOrdinal("Plz")).ToString();
            sekretariat.Ort = reader.GetValue(reader.GetOrdinal("Ort")).ToString();

            // if both person names are filled, we store the values in the person fields
            if (!string.IsNullOrEmpty(sekretariat.Vorname) && !string.IsNullOrEmpty(sekretariat.Nachname))
            {
                sekretariat.PersonTel = reader.GetValue(reader.GetOrdinal("Tel")).ToString();
                sekretariat.PersonMobile = reader.GetValue(reader.GetOrdinal("Mobile")).ToString();
                sekretariat.PersonEMail = reader.GetValue(reader.GetOrdinal("EMail")).ToString();
            }
            else
            {
                sekretariat.Tel = reader.GetValue(reader.GetOrdinal("Tel")).ToString();
                sekretariat.Fax = reader.GetValue(reader.GetOrdinal("Fax")).ToString();
                sekretariat.Mobile = reader.GetValue(reader.GetOrdinal("Mobile")).ToString();
                sekretariat.EMail = reader.GetValue(reader.GetOrdinal("EMail")).ToString();
            }

            var mappedContactPoint = MigrationMapping.ToContactPoint(sekretariat, committeeId);

            _contactPointRepository.CreateForMigration(mappedContactPoint);
        }

        _logger.LogInformation("Contact points migrated completely.");
    }

    public async Task VerifyContactPointAddresses()
    {
        _logger.LogInformation("Now verifying ContactPoint addresses with PostService.");

        var contactPoints = await _contactPointRepository.GetAllUnverifiedContactPoints();
        var countValidations = 0;
        var countInvalid = 0;
        var countAmbiguous = 0;

        foreach (var contactPoint in contactPoints)
        {
            if (!string.IsNullOrEmpty(contactPoint.City) && !string.IsNullOrEmpty(contactPoint.Zip))
            {
                countValidations++;

                var dto = new AddressSearchDto { City = contactPoint.City, Zip = contactPoint.Zip, Street = contactPoint.Street };
                var result = await _postService.Verify(dto);

                if (result.Status == AddressVerificationStatus.Invalid)
                {
                    _logger.LogInformation("ContactPoint ID {ContactPointId} got validated invalid: {Street}, {Zip}, {City}", contactPoint.Id, contactPoint.Street, contactPoint.Zip, contactPoint.City);
                    countInvalid++;
                    contactPoint.VerificationCode = (int?)AddressVerificationStatus.Invalid;
                }
                else if (result.Status == AddressVerificationStatus.Ambiguous)
                {
                    _logger.LogInformation("ContactPoint ID {ContactPointId} has an ambiguous result: {Street}, {Zip}, {City}", contactPoint.Id, contactPoint.Street, contactPoint.Zip, contactPoint.City);
                    countAmbiguous++;
                    contactPoint.VerificationCode = (int?)AddressVerificationStatus.Ambiguous;
                }
                else
                {
                    contactPoint.VerifiedSuccessfully = true;
                }

                // currently, we do not log OK or Corrected results
                //if (result.Status == AddressVerificationStatus.Corrected)
                //{
                //    _logger.LogInformation("ContactPoint ID {0} has a corrected result: {1}, {2}, {3} ", contactPoint.Id, contactPoint.Street, contactPoint.Zip, contactPoint.City);
                //    countCorrected++;
                //}
            }
        }

        await _contactPointRepository.CommitChanges();

        _logger.LogInformation("{ValidatedCount} contactpoints verified : {InvalidCount} invalid, {AmbiguousCount} ambiguous...", countValidations, countInvalid, countAmbiguous);
    }
}
