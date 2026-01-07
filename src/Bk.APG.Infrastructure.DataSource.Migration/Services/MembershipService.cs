using Bk.APG.Business.Repositories;
using Bk.APG.Infrastructure.DataSource.Migration.Mapping;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class MembershipService : IMembershipService
{
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly IMembershipRepository _membershipRepository;
    private readonly ILogger<MembershipService> _logger;

    private readonly Dictionary<Guid, Guid> _functionMapForCleanup = new()
    {
        { new Guid("BE7006BC-33B8-4621-A2E7-3A06956A7F21"), new Guid("A282A0CD-4A7D-48B6-9B52-9B216E9454FE") },
        { new Guid("50231C33-E549-4D1D-8484-1A11884B8030"), new Guid("17F63CD3-F254-4E6E-BD84-37311B38041C") },
        { new Guid("31AF5151-5A15-45A7-82CF-84B2EB0FB8B8"), new Guid("C54D3FDD-5819-49C5-9848-8A6D3F892925") },
        { new Guid("002E07A4-DBF8-47EA-A286-B191718A96E3"), new Guid("B0D157EE-A887-49DD-AC09-B05A39F92CB5") },
        { new Guid("62BB29AC-C193-474B-8A12-FC2E88A5D072"), new Guid("4E5DC4E3-4563-4B8F-87AB-0F70B138927F") },
        { new Guid("39257906-C1BF-4008-B01F-4CECAA640681"), new Guid("3949A0FD-6961-4BFE-9046-8A2BFD86F9CF") },
        { new Guid("DA3C2E89-A3CA-44C6-AACD-1126C23B8C2B"), new Guid("7944D508-C784-4FA9-8885-6BFEB41EC0BD") },
        { new Guid("40BC9B3E-EEC2-49A4-A260-7BA84BA763EE"), new Guid("C36B8B7A-FD7F-4056-909D-9F4BC4C91380") },
        { new Guid("1172AF61-3D3F-4F18-B352-092A950D1A01"), new Guid("2991B76B-CE7E-44F0-9255-D2A57A567C2C") },
        { new Guid("7492E69B-7A21-4F83-ACBF-BB3BF9643EBA"), new Guid("61F627CF-43C5-4746-9D29-4202A4DA0C27") },
        { new Guid("AAFF2635-1AF3-4421-B5F3-0C08E00A6470"), new Guid("255D79D3-00FB-467E-8B2F-1F01683E0B27") },
        { new Guid("349AB80E-B9D4-4F18-933F-F7D8D0EC4B33"), new Guid("3812EA80-BF90-41E7-AF4A-E65B45779C27") },
        { new Guid("D7099A50-8426-4A8D-943D-2022CEBB24A7"), new Guid("324C68B8-E136-4D98-B76E-2488D794E0A4") },
        { new Guid("97AFD32F-418D-4C36-AA17-47BF70C4AFC9"), new Guid("3826BC5D-5ACE-4371-891B-4AD225186450") },
        { new Guid("93F54C77-E851-4E39-AB0B-ED40D0B56C50"), new Guid("A4971074-C1CC-4372-8AB3-A8FF0E9182E8") },
        { new Guid("0C3900CC-D582-4995-A174-3C9EFF5C4B66"), new Guid("FAA8EDC2-F784-4E4A-B308-B4DEC1C56A54") }
    };

    public MembershipService(IMembershipRepository membershipRepository, ILogger<MembershipService> logger)
    {
        _membershipRepository = membershipRepository;
        _logger = logger;
    }

    public void MigrateMembershipsForCommittee(SqlConnection connection, Guid committeeId)
    {
        _logger.LogInformation("Start migrating memberships for committee {CommitteeId}", committeeId);

        var commandText = "SELECT m.*, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Person WHERE Id = m.PerId ) as PersonGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Gremium WHERE Id = m.GremId ) as GremiumGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Wahlart WHERE Id = m.WaaId ) as WahlartGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Funktion WHERE Id = m.FnkId ) as FunktionGuid, " +
                          "(SELECT Cast(GUID as nvarchar(36)) FROM Wahlbehörde WHERE Id = m.WabId ) as WahlbehördeGuid " +
                          "FROM Mitglied m Inner Join Gremium g on g.Id = m.GremId " +
                          // do not select the incomplete records without a name/office. As soon as these are fixed in the data, the selection will be valid!
                          "WHERE m.PerId in (SELECT Id FROM Person WHERE Vorname is not null AND Name is not null) " +
                          "AND m.GremID in (SELECT Id FROM Gremium WHERE ZustVerwStelleId is not null)" +
                          "AND g.Guid = '" + committeeId + "';";

        using var command = new SqlCommand(commandText, connection);
        command.CommandTimeout = 60;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var mitglied = new Mitglied();

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("Id")).ToString(), out var validNumber))
            {
                mitglied.Id = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("MgzId")).ToString(), out validNumber))
            {
                mitglied.MgzId = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("BeschäftigungsGradVon")).ToString(), out validNumber))
            {
                mitglied.BeschäftigungsGradVon = validNumber;
            }

            if (int.TryParse(reader.GetValue(reader.GetOrdinal("BeschäftigungsGradBis")).ToString(), out validNumber))
            {
                mitglied.BeschäftigungsGradBis = validNumber;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("InsertDate")).ToString(), out var validDate))
            {
                mitglied.InsertDate = validDate.ToUniversalTime();
            }
            else
            {
                mitglied.InsertDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("UpdateDate")).ToString(), out validDate))
            {
                mitglied.UpdateDate = validDate.ToUniversalTime();
            }
            else
            {
                mitglied.UpdateDate = _now;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("Histo")).ToString(), out validDate))
            {
                mitglied.Histo = validDate.ToUniversalTime();
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("SeitDate")).ToString(), out validDate))
            {
                mitglied.SeitDate = validDate;
            }

            if (DateTime.TryParse(reader.GetValue(reader.GetOrdinal("BisDate")).ToString(), out validDate))
            {
                mitglied.BisDate = validDate;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("PersonGuid")).ToString(), out var validGuid))
            {
                mitglied.PersonGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("GremiumGuid")).ToString(), out validGuid))
            {
                mitglied.GremiumGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("WahlartGuid")).ToString(), out validGuid))
            {
                mitglied.WahlartGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("WahlbehördeGuid")).ToString(), out validGuid))
            {
                mitglied.WahlbehördeGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("FunktionGuid")).ToString(), out validGuid))
            {
                if (_functionMapForCleanup.TryGetValue(validGuid, out var value))
                {
                    validGuid = value;
                }

                mitglied.FunktionGuid = validGuid;
            }

            if (Guid.TryParse(reader.GetValue(reader.GetOrdinal("Guid")).ToString(), out validGuid))
            {
                mitglied.Guid = validGuid;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("KomVer")).ToString(), out var validBoolean))
            {
                mitglied.KomVer = validBoolean;
            }

            if (bool.TryParse(reader.GetValue(reader.GetOrdinal("GEW")).ToString(), out validBoolean))
            {
                mitglied.GEW = validBoolean;
            }

            mitglied.Mitgliedzusatz = reader.GetValue(reader.GetOrdinal("Mitgliedzusatz")).ToString();
            mitglied.BemerkungStatus = reader.GetValue(reader.GetOrdinal("BemerkungStatus")).ToString();
            mitglied.BegrAmtszeit = reader.GetValue(reader.GetOrdinal("BegrAmtszeit")).ToString();
            mitglied.BegrBvers = reader.GetValue(reader.GetOrdinal("BegrBvers")).ToString();
            mitglied.BegrAlter = reader.GetValue(reader.GetOrdinal("BegrAlter")).ToString();
            mitglied.BegrBangest = reader.GetValue(reader.GetOrdinal("BegrBangest")).ToString();
            mitglied.Bemerkung = reader.GetValue(reader.GetOrdinal("Bemerkung")).ToString();
            mitglied.RechtsverhältnisBund = reader.GetValue(reader.GetOrdinal("RechtsverhältnisBund")).ToString();
            mitglied.RechtsverhältnisGrem = reader.GetValue(reader.GetOrdinal("RechtsverhältnisGrem")).ToString();
            mitglied.LastupdateUser = reader.GetValue(reader.GetOrdinal("LastupdateUser")).ToString();

            var mappedMembership = MigrationMapping.ToMembership(mitglied);

            _membershipRepository.CreateForMigration(mappedMembership);
        }

        _logger.LogInformation("Memberships for committee {CommitteeId} migrated completely", committeeId);
    }

    public async Task<IEnumerable<Guid>> GetUniquePersonIdsForAllActiveMemberships()
    {
        var allActiveMemberships = await _membershipRepository.GetAllActiveMemberships();

        var personIdList = allActiveMemberships.Select(m => m.PersonId).Distinct().ToList();

        return personIdList;
    }
}
