using Lucene.Net.QueryParsers.Flexible.Core.Nodes;
using Lucene.Net.Search;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Migrations;
using VDS.RDF.Query.Algebra;
using static System.Runtime.InteropServices.JavaScript.JSType;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class AddCommitteeDissolutionWithdrawal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            migrationBuilder.Sql(@$"

            INSERT INTO data.election_types (""id"", ""created"", ""created_by"", ""modified"", ""modified_by"", ""is_deleted"", ""text_de"", ""text_fr"", ""text_it"", ""text_rm"", ""description_de"", ""description_fr"", ""description_it"", ""description_rm"", ""sort"", ""uri"", ""old_id"")
                VALUES
                    ('1EB590BB-FED2-4834-9A8E-E8E35E7BDE93', now(), 'migration', now(), 'migration', false, 'Ausscheidung aufgrund Gremienauflösung', 'Retrait en raison de la dissolution de l''organe', 'Cessazione a causa dello scioglimento dell''organo', '', '', '', '', '', 0, 'https://politics.ld.admin.ch/fch/apg/vocabulary/election-type/9', 102)
             ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
