using Bk.APG.Business.Repositories;
using Bk.APG.Infrastructure.DataSource.Migration.Mapping;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class CommitteeService : ICommitteeService
{
    private readonly ILogger<AddressService> _logger;
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IMembershipService _membershipService;
    private readonly IContactPointService _contactPointService;
    private readonly DataContext _dataContext;

    public CommitteeService(ICommitteeRepository committeeRepository, IMembershipService membershipService, IContactPointService contactPointService,
        DataContext dataContext, ILogger<AddressService> logger)
    {
        _committeeRepository = committeeRepository;
        _membershipService = membershipService;
        _contactPointService = contactPointService;
        _dataContext = dataContext;
        _logger = logger;
    }

    public void MigrateCommittees(SqlConnection connection)
    {
        _logger.LogInformation("Start migrating committees.");

        var commandText = "SELECT g.*, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'de-CH' and Typ = 'Bezeichnung') as BezeichnungD, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'fr-CH' and Typ = 'Bezeichnung') as BezeichnungF, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'it-CH' and Typ = 'Bezeichnung') as BezeichnungI, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'rm-CH' and Typ = 'Bezeichnung') as BezeichnungR, " +
            "(SELECT Cast(GUID as nvarchar(36)) FROM Stufe WHERE Id = g.StuId ) as StufeGuid, " +
            "(SELECT Cast(GUID as nvarchar(36)) FROM AllgemeineMassnahmenGeschlechter WHERE Id = g.MassnaAllgGeschlechterId ) as MassnaAllgGeschlechterGuid, " +
            "(SELECT Cast(GUID as nvarchar(36)) FROM AllgemeineMassnahmenSprachen WHERE Id = g.MassnaAllgSprachenId ) as MassnaAllgSprachenGuid, " +
            "(SELECT Cast(GUID as nvarchar(36)) FROM Gremiumart WHERE Id = g.GraId ) as GremiumartGuid, " +
            "(SELECT Cast(GUID as nvarchar(36)) FROM Departement WHERE Id = g.DepId ) as DepartementGuid, " +
            "(SELECT Cast(GUID as nvarchar(36)) FROM Amtsperiode WHERE Id = g.AmpId ) as AmtsperiodeGuid, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'de-CH' and Typ = 'LinkHomepage') as HomepageLinkDe, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'fr-CH' and Typ = 'LinkHomepage') as HomepageLinkFr, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'it-CH' and Typ = 'LinkHomepage') as HomepageLinkIt, " +
            "(SELECT Text FROM Translation WHERE OwningObjectGuid = g.GuidTranslation and Sprache = 'rm-CH' and Typ = 'LinkHomepage') as HomepageLinkRm, " +
            "Cast(om.GUID as nvarchar(36)) as AmtGuid " +
            "FROM Gremium g LEFT JOIN OfficeMapping om " +
            "ON om.Old_ID = g.ZustVerwStelleId ";
        // The database contains one record with NULL in ZustVerwStelleId. This will be logged in migration.
        // "WHERE g.ZustVerwStelleId is not null";

        using var command = new SqlCommand(commandText, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var gremium = new Gremium();

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Id")).ToString(), out var validNumber))
            {
                gremium.Id = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("AnzahlVakanzenGEW")).ToString(), out validNumber))
            {
                gremium.AnzahlVakanzenGEW = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("MassnaAllgSprachenId")).ToString(), out validNumber))
            {
                gremium.MassnaAllgSprachenId = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("MassnaAllgGeschlechterId")).ToString(), out validNumber))
            {
                gremium.MassnaAllgGeschlechterId = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("MaxAnzMitglieder")).ToString(), out validNumber))
            {
                gremium.MaxAnzMitglieder = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("MinAnzMitglieder")).ToString(), out validNumber))
            {
                gremium.MinAnzMitglieder = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("GraId")).ToString(), out validNumber))
            {
                gremium.GraId = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("DepId")).ToString(), out validNumber))
            {
                gremium.DepId = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("AmpId")).ToString(), out validNumber))
            {
                gremium.AmpId = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("StatusId")).ToString(), out validNumber))
            {
                gremium.StatusId = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("StuId")).ToString(), out validNumber))
            {
                gremium.StuId = validNumber;
            }
            if (int.TryParse(reader.GetValue(reader.GetOrdinal("ZustVerwStelleId")).ToString(), out validNumber))
            {
                gremium.ZustVerwStelleId = validNumber;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("KomVer")).ToString(), out var validBoolean))
            {
                gremium.KomVer = validBoolean;
            }
            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("Bundesgesetzt")).ToString(), out validBoolean))
            {
                gremium.Bundesgesetz = validBoolean;
            }
            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("ZusätzKantonGewMitglieder")).ToString(), out validBoolean))
            {
                gremium.ZusätzKantonGewMitglieder = validBoolean;
            }
            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("FreigabeSammelantrag")).ToString(), out validBoolean))
            {
                gremium.FreigabeSammelantrag = validBoolean;
            }
            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("PublishIB")).ToString(), out validBoolean))
            {
                gremium.PublishIB = validBoolean;
            }
            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("GEW")).ToString(), out validBoolean))
            {
                gremium.GEW = validBoolean;
            }
            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("Aufsichtsaufgabe")).ToString(), out validBoolean))
            {
                gremium.Aufsichtsaufgabe = validBoolean;
            }

            // this is the only real nullable boolean, which has also valid NULL values.
            var marktOrientiert = reader.GetValue(reader.GetOrdinal("Marktorientiert")).ToString() ?? string.Empty;

            if (bool.TryParse(marktOrientiert, out validBoolean))
            {
                gremium.Marktorientiert = validBoolean;
            }
            else
            {
                gremium.Marktorientiert = null;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("InsertDate")).ToString(), out var validDate))
            {
                gremium.InsertDate = validDate.ToUniversalTime();
            }
            else
            {
                gremium.InsertDate = _now;
            }
            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("UpdateDate")).ToString(), out validDate))
            {
                gremium.UpdateDate = validDate.ToUniversalTime();
            }
            else
            {
                gremium.UpdateDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Histo")).ToString(), out validDate))
            {
                gremium.Histo = validDate;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("BeginnDatum")).ToString(), out validDate))
            {
                gremium.BeginnDatum = validDate;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("EndDatum")).ToString(), out validDate))
            {
                gremium.EndDatum = validDate;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("Guid")).ToString(), out var validGuid))
            {
                gremium.Guid = validGuid;
            }
            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("StufeGuid")).ToString(), out validGuid))
            {
                gremium.StufeGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("MassnaAllgGeschlechterGuid")).ToString(), out validGuid))
            {
                gremium.MassnaAllgGeschlechterGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("MassnaAllgSprachenGuid")).ToString(), out validGuid))
            {
                gremium.MassnaAllgSprachenGuid = validGuid;
            }
            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GremiumartGuid")).ToString(), out validGuid))
            {
                gremium.GremiumartGuid = validGuid;
            }
            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("DepartementGuid")).ToString(), out validGuid))
            {
                gremium.DepartementGuid = validGuid;
            }
            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("AmtsperiodeGuid")).ToString(), out validGuid))
            {
                gremium.AmtsperiodeGuid = validGuid;
            }
            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("AmtGuid")).ToString(), out validGuid))
            {
                gremium.AmtGuid = validGuid;
            }
            else
            {
                // this is a hardcoded fix of the one committee with id 111 from the federal chancellery; we set hier die GUID for the manually created office of FCH.
                gremium.AmtGuid = Guid.Parse("f3b9a8c6-7a0f-4c63-a69f-9dbb872d1125");
            }

            gremium.BezeichnungD = reader.GetValue(reader.GetOrdinal("BezeichnungD")).ToString();
            gremium.BezeichnungF = reader.GetValue(reader.GetOrdinal("BezeichnungF")).ToString();
            gremium.BezeichnungI = reader.GetValue(reader.GetOrdinal("BezeichnungI")).ToString();
            gremium.BezeichnungR = reader.GetValue(reader.GetOrdinal("BezeichnungR")).ToString();
            gremium.BegrAnzMitglieder = reader.GetValue(reader.GetOrdinal("BegrAnzMitglieder")).ToString();
            gremium.BegrSprachen = reader.GetValue(reader.GetOrdinal("BegrSprachen")).ToString();
            gremium.MassnaGremSprachen = reader.GetValue(reader.GetOrdinal("MassnaGremSprachen")).ToString();
            gremium.BegrGeschlechter = reader.GetValue(reader.GetOrdinal("BegrGeschlechter")).ToString();
            gremium.MassnaGremGeschlechter = reader.GetValue(reader.GetOrdinal("MassnaGremGeschlechter")).ToString();
            gremium.Rechtsform = reader.GetValue(reader.GetOrdinal("Rechtsform")).ToString();
            gremium.GesetzlicheGrundlagen = reader.GetValue(reader.GetOrdinal("GesetzlicheGrundlagen")).ToString();
            gremium.BemerkungGrunddaten = reader.GetValue(reader.GetOrdinal("BemerkungGrunddaten")).ToString();
            gremium.BemerkungBegründungen = reader.GetValue(reader.GetOrdinal("BemerkungBegründungen")).ToString();
            gremium.BemerkungEnsetzvBeschl = reader.GetValue(reader.GetOrdinal("BemerkungEnsetzvBeschl")).ToString();
            gremium.BemerkungZusatzInfo = reader.GetValue(reader.GetOrdinal("BemerkungZusatzInfo")).ToString();
            gremium.BemerkungSekretariate = reader.GetValue(reader.GetOrdinal("BemerkungSekretariate")).ToString();
            gremium.BemerkungMitglieder = reader.GetValue(reader.GetOrdinal("BemerkungMitglieder")).ToString();
            gremium.BemerkungGrunddatenAdmin = reader.GetValue(reader.GetOrdinal("BemerkungGrunddatenAdmin")).ToString();
            gremium.BemerkungBegründungenAdmin = reader.GetValue(reader.GetOrdinal("BemerkungBegründungenAdmin")).ToString();
            gremium.BemerkungEnsetzvBeschlAdmin = reader.GetValue(reader.GetOrdinal("BemerkungEnsetzvBeschlAdmin")).ToString();
            gremium.BemerkungZusatzInfoAdmin = reader.GetValue(reader.GetOrdinal("BemerkungZusatzInfoAdmin")).ToString();
            gremium.BemerkungSekretariateAdmin = reader.GetValue(reader.GetOrdinal("BemerkungSekretariateAdmin")).ToString();
            gremium.BemerkungMitgliederAdmin = reader.GetValue(reader.GetOrdinal("BemerkungMitgliederAdmin")).ToString();
            gremium.ZusätzKantonGewMitgliederUrl = reader.GetValue(reader.GetOrdinal("ZusätzKantonGewMitgliederUrl")).ToString();
            gremium.LastupdateUser = reader.GetValue(reader.GetOrdinal("LastupdateUser")).ToString();
            gremium.HomepageLinkDe = reader.GetValue(reader.GetOrdinal("HomepageLinkDe")).ToString();
            gremium.HomepageLinkFr = reader.GetValue(reader.GetOrdinal("HomepageLinkFr")).ToString();
            gremium.HomepageLinkIt = reader.GetValue(reader.GetOrdinal("HomepageLinkIt")).ToString();
            gremium.HomepageLinkRm = reader.GetValue(reader.GetOrdinal("HomepageLinkRm")).ToString();

            var committee = MigrationMapping.ToCommittee(gremium);
            var isCommitteeActive = committee.EndDate < DateOnly.FromDateTime(DateTime.Now);

            _committeeRepository.CreateForMigration(committee);
            _dataContext.SaveChanges();
            _membershipService.MigrateMembershipsForCommittee(connection, committee.Id);
            _dataContext.SaveChanges();
            _contactPointService.MigrateContactPointsForCommittee(connection, committee.Id);
            _dataContext.SaveChanges();
        }

        // TODO, when using direct way to migrate on RHOS, we have a permission issue here (must be owner of table committees).
        //var maxCommitteeNumber = _dataContext.Committees.Max(x => x.CommitteeNumber);
        //_dataContext.Database.ExecuteSqlRaw($"ALTER TABLE {DataContext.Schema}.committees ALTER COLUMN committee_number RESTART WITH {maxCommitteeNumber + 1};");

        _logger.LogInformation("Committees migrated completely.");
    }
}

