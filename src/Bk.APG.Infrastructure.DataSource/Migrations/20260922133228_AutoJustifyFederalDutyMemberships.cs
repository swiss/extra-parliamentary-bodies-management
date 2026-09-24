using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations;

public partial class AutoJustifyFederalDutyMemberships : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);

        migrationBuilder.Sql("""
            UPDATE data.memberships AS m
            SET
                justification_longer_duty = 'Die Amtszeitbeschränkung gilt nicht für Bundesangestellte, deren Mitgliedschaft für die Aufgabenerfüllung erforderlich ist oder in einem anderen Erlass zwingend vorgeschrieben wird (Art. 8i Abs. 3 RVOV).',
                modified = now(),
                modified_by = 'migration'
            FROM data.committees AS c
            WHERE m.committee_id = c.id
              AND c.committee_type_id IN ('f2e2af70-d1d4-42b5-b23a-793cbc220064', '0a4b7f1d-d8bf-4932-bece-dd2a51cc2d59')
              AND m.in_correlation_with_federal_duty = TRUE;
            """);

        migrationBuilder.Sql("""
            UPDATE data.membership_candidates AS mc
            SET
                justification_longer_duty = 'Die Amtszeitbeschränkung gilt nicht für Bundesangestellte, deren Mitgliedschaft für die Aufgabenerfüllung erforderlich ist oder in einem anderen Erlass zwingend vorgeschrieben wird (Art. 8i Abs. 3 RVOV).',
                modified = now(),
                modified_by = 'migration'
            FROM data.general_election_committees AS gec
            INNER JOIN data.committees AS c ON c.id = gec.committee_id
            WHERE mc.general_election_committee_id = gec.id
              AND c.committee_type_id IN ('f2e2af70-d1d4-42b5-b23a-793cbc220064', '0a4b7f1d-d8bf-4932-bece-dd2a51cc2d59')
              AND mc.in_correlation_with_federal_duty = TRUE;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
    }
}
