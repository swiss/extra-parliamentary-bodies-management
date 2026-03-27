using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bk.APG.Infrastructure.DataSource.Migrations
{
    /// <inheritdoc />
    public partial class ExtendMembershipAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ArgumentNullException.ThrowIfNull(migrationBuilder);

            // Remove duplicates (Vertretung der Medizinischen Ethik and Vertretung des Gemeindeverbands)
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "80ef61f4-2eca-438a-9f30-eb82d71e4431", column: "is_deleted", value: true);
            migrationBuilder.UpdateData(schema: "data", table: "membership_additions", keyColumn: "id", keyValue: "0d70b752-6591-44c6-9cfc-984693b1b8d5", column: "is_deleted", value: true);

            // Insert new additions
            #pragma warning disable CA1814
            migrationBuilder.InsertData(
                schema: "data",
                table: "membership_additions",
                columns: new[] { "id", "created", "created_by", "modified", "modified_by", "description_de", "description_fr", "description_it", "description_rm", "text_de", "text_fr", "text_it", "text_rm", "uri", "old_id" },
                values: new object[,]
                {
                    { "ebe2225f-f69f-433c-8cf1-b7c7fc0ce0ec", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Apothekerschaft und EAK", "Représentant des pharmaciens et la CFM", "Rappresentanza farmacista e CFM", "", "www.todo.uri.ebe2225f-f69f-433c-8cf1-b7c7fc0ce0ec", 0 },
                    { "78f92885-d367-4fe6-85b6-2681a23a3285", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Analysen-, Mittel- und Gegenständekommission", "Représentant de la Commission des analyses, moyens et appareils", "Rappresentanza della Commissione delle analisi, dei mezzi e degli apparecchi", "", "www.todo.uri.78f92885-d367-4fe6-85b6-2681a23a3285", 0 },
                    { "91507e9a-79ed-44fe-8e5a-a8f49a0ce32b", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Medizintechnikindustrie", "Représentant de l’industrie de la technique médicale", "Rappresentanza dell’industria della tecnica medica", "", "www.todo.uri.91507e9a-79ed-44fe-8e5a-a8f49a0ce32b", 0 },
                    { "e9518654-6f98-4c22-8ef0-6fcb406552fe", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Fakultäten der Medizin und Pharmazie", "Représentant des facultés de médecine et de pharmacie", "Rappresentanza delle facoltà di medicina e di farmacia", "", "www.todo.uri.e9518654-6f98-4c22-8ef0-6fcb406552fe", 0 },
                    { "478b6190-4e75-49a9-a2ac-73548fa175e0", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Ärzteschaft / Komplementärmedizin", "Représentant des médecins / complémentaire", "Rappresentanza dei medici / complementare", "", "www.todo.uri.478b6190-4e75-49a9-a2ac-73548fa175e0", 0 },
                    { "bbb0f9f8-06b9-4c9e-a042-225e7fa35df7", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung des Schweizerischen Heilmittelinstitut (Swissmedic)", "Représentant l’Institut suisse des produits thérapeutiques (Swissmedic)", "Rappresentanza dell’Istituto svizzero per gli agenti terapeutici (Swissmedic)", "", "www.todo.uri.bbb0f9f8-06b9-4c9e-a042-225e7fa35df7", 0 },
                    { "70cc6f22-5145-4cf0-8208-a769eb7b04a0", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Pflegefachpersonen", "Représentant du personnel infirmier", "Rappresentanza del personale infermieristico", "", "www.todo.uri.70cc6f22-5145-4cf0-8208-a769eb7b04a0", 0 },
                    { "ba527ea9-b037-4b25-aa49-110cd0f07238", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Diagnostica-Geräteindustrie", "Représentant de l’industrie des dispositifs diagnostiques", "Rappresentanza dell’industria delle apparecchiature diagnostiche", "", "www.todo.uri.ba527ea9-b037-4b25-aa49-110cd0f07238", 0 },
                    { "43ab22ce-e0a8-4109-9787-f4dc63a74ad5", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Frauenorganisationen", "Représentant des organisations de femmes", "Rappresentanza delle organizzazioni di donne", "", "www.todo.uri.43ab22ce-e0a8-4109-9787-f4dc63a74ad5", 0 },
                    { "8ce56c36-99c7-4fae-a7c5-68d6f227b2e8", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Betroffenen", "Représentant des personnes concernées", "Rappresentanza delle persone interessate", "", "www.todo.uri.8ce56c36-99c7-4fae-a7c5-68d6f227b2e8", 0 },
                    { "90483c00-85ef-46a7-9bda-2563bcef5944", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der Dachverbände der Kinder- und Jugendorganisationen", "Représentant des organisations faîtières des organisations d’enfants et de jeunesse", "Rappresentanza delle organizzazioni mantello delle organizzazioni dell’infanzia e della gioventù", "", "www.todo.uri.90483c00-85ef-46a7-9bda-2563bcef5944", 0 },
                    { "2c7aa564-a168-4d18-b6ce-273fa952911c", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der interkantonalen Fachkonferenzen im Kinder- und Jugendbereich", "Représentant des conférences spécialisées intercantonales dans le domaine de l’enfance et de la jeunesse", "Rappresentanza delle conferenze specialistiche intercantonali nel settore dell’infanzia e della gioventù", "", "www.todo.uri.2c7aa564-a168-4d18-b6ce-273fa952911c", 0 },
                    { "f92e75f7-be91-4222-9342-70afdab3652c", DateTime.UtcNow, "migration", DateTime.UtcNow, "migration", "", "", "", "", "Vertretung der politischen (Jung-)Parteien", "Représentant des partis politiques (jeunes)", "Rappresentanza dei partiti politici (giovani)", "", "www.todo.uri.f92e75f7-be91-4222-9342-70afdab3652c", 0 },
                });
#pragma warning restore CA1814
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
