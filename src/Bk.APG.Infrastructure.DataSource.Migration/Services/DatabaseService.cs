using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class DatabaseService(DataContext targetContext, ILogger<DatabaseService> logger) : IDatabaseService
{
    public async Task<SqlConnection> OpenConnection(string connectionString)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        return connection;
    }

    public async Task CloseConnection(SqlConnection connection)
    {
        await connection!.CloseAsync();
        connection.Dispose();
    }

    /// This function executes cleanup and fixing tasks on the existing database.
    /// It also creates 2 helper tables, as no GUIDs were created for address table.
    public void PrepareAndFixSourceDatabase(SqlConnection connection)
    {
        logger.LogInformation("Creating helper tables for mappings");

        // to be able to get some proper names, we add an additional column for the last name(for DataProtectionOfficers and so on)
        var commandText = "if not exists (SELECT 1 FROM sysobjects INNER JOIN syscolumns ON sysobjects.id = syscolumns.id " +
                          "WHERE sysobjects.name = N'Sekretariat' AND syscolumns.name = N'Nachname') " +
                          "ALTER TABLE [dbo].[Sekretariat] ADD Nachname NVARCHAR(150) ";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "if not exists (SELECT 1 FROM sysobjects INNER JOIN syscolumns ON sysobjects.id = syscolumns.id " +
                      "WHERE sysobjects.name = N'Sekretariat' AND syscolumns.name = N'Sektion') " +
                      "ALTER TABLE [dbo].[Sekretariat] ADD Sektion NVARCHAR(150) ";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "if not exists (SELECT 1 FROM sysobjects INNER JOIN syscolumns ON sysobjects.id = syscolumns.id " +
                      "WHERE sysobjects.name = N'Sekretariat' AND syscolumns.name = N'Datenschutzberater') " +
                      "ALTER TABLE [dbo].[Sekretariat] ADD Datenschutzberater BIT DEFAULT 0";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "IF OBJECT_ID('[dbo].[OfficeMapping]', 'U') IS NOT NULL DROP TABLE [dbo].[OfficeMapping]";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "CREATE TABLE [dbo].[OfficeMapping] ( [GUID] [uniqueidentifier] NOT NULL, [Old_ID] [int] NULL ) ON[PRIMARY] ";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "UPDATE Adresse SET Kanton = 'Ausl' WHERE Kanton = 'FL' ";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = FillOfficeMapping();
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "IF OBJECT_ID('[dbo].[PersonAddressMapping]', 'U') IS NOT NULL DROP TABLE [dbo].[PersonAddressMapping]";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "CREATE TABLE [dbo].[PersonAddressMapping]( [GUID] [uniqueidentifier] NOT NULL, [PersonID] [int] NULL, [AddressID] [int] NULL, " +
                      "[CommunicationAddress] [bit] NULL, [PrivateAddress] [bit] NULL ) ON [PRIMARY]";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "" +
                      "INSERT INTO [dbo].[PersonAddressMapping] " +
                      "SELECT newid(), PersId, Id, AnschriftAdresse, Case WHEN AdrGeschäftlich = 1 THEN 0 ELSE 1 END as PrivateAddress " +
                      "FROM [dbo].[Adresse] " +
                      "WHERE PersId is not null " +
                      "AND (" +
                      "(Strasse is not null AND Strasse <> '') OR " +
                      "(LänderCode is not null AND LänderCode <> '') OR " +
                      "(Plz is not null AND Plz <> '') OR " +
                      "(Ort is not null AND Ort <> '') OR " +
                      "(Kanton is not null AND Kanton <> '') OR " +
                      "(Mobile is not null AND Mobile <> '') OR " +
                      "(Telefon is not null AND Telefon <> '') OR " +
                      "(Email is not null AND Email <> '') OR " +
                      "(Firma is not null AND Firma <> '') OR " +
                      "(Postfach is not null AND Postfach <> '') OR " +
                      "(PlzZusatz is not null AND PlzZusatz <> '') " +
                      ") ";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "UPDATE Gremiumart SET DEThreshold = 0 WHERE DEThreshold is null";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "UPDATE Gremiumart SET FRThreshold = 0 WHERE FRThreshold is null";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "UPDATE Gremiumart SET ITThreshold = 0 WHERE ITThreshold is null";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "UPDATE Gremiumart SET RMThreshold = 0 WHERE RMThreshold is null";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "UPDATE Gremiumart SET MenThreshold = 0 WHERE MenThreshold is null";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "UPDATE Gremiumart SET WomenThreshold = 0 WHERE WomenThreshold is null";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Fill new field Nachname, init on Datenschutzberater
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Nachname = NameOrganisation, Datenschutzberater = 0";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Mark all dataprotection officers
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Datenschutzberater = 1,  NameOrganisation = '' where NameOrganisation like '%Datenschutz%'";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Mark the one dataprotection officer having it in the surname
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Datenschutzberater = 1,  NameOrganisation = '', Nachname = 'Quan', Vorname = 'Isabel' where Nachname like '%Datenschutzberaterin%'";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup the NameOrganisation column, active and inactive committees
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set NameOrganisation = '' where NameOrganisation in (" +
                      "'Aeschlimann','Albisetti','Amport','Anrig','Arni','Bacher','Baer Bösch','Bär','Baumann','Baumgartner','Benzi Schmid','Biscontin','Blaser','Böhler'," +
                      "'Bossart','Brink','Brodmann','Bruneau','Brunner','Carosella','Castra','Cerratti','Christen','Correvon Friderici','De Berardinis','Dietsche ','Dolt'," +
                      "'Durstberger','Egger','Elmazi','Elmiger','Feuz','Fierz Wengert','Fluck','Francey','Frei','Friedländer','Gälli Purghart','Gerber','Giani','Gilg','Glaser'," +
                      "'Gosteli','Grossenbacher-Mansuy','Guggisberg','Häni','Haueter','Herrmann','Hess','Holocher','Hubacher','Humair','Humbert','Hutter','Jacot','Kaempfer','Kaufmann'," +
                      "'Klaassen','Koltsidas','Kunze','Lehmann','Lehmann','Leonarz','Link','Looser','Lundsgaard-Hansen','Lustenberger','Maillard','Markwalder','Marti','Mathieu'," +
                      "'Maurer','Maus','Meglen','Meier','Meister','Michel','Montani','Montini','Moser','Moser','Muller','Müller Könz','Münch','Nenning','Niederhäuser','Noguet'," +
                      "'Nolde','Nyffenegger','Oberholzer','Oeschger','Olivier','Passaplan','Pauchard','Peduzzi','Richter','Rieder','Ries','Rime','Robert-Tissot','Rodriguez', " +
                      "'Rohrbach','Romeril','Rothen','Schärer','Schibli','Schleiss','Schmidiger','Schönenberger','Schumacher','Siffert','Spycher','Storch','Stritt','Suter'," +
                      "'Taboada Gomez','Taschetta','Triverio','Voegeli','Vögele','Wallimann','Wegmüller','Wiecken','Wigger-Häusler','Willemsen','Winzenried','Witschi','Wölfli'," +
                      "'Wüthrich','Ziegler','Zingg','Zuber','Zumbrunnen'," +
                      "'André Poirot','Aréstegui','Asal-Steger ','Baader','Bänziger','Barras Duc','Baumeler','Ben Achour','Bill','Bischoff','Bläuer','Bloch','Boutellier','Büchel','Burkhard','Carattini'," +
                      "'Chmelova Carron','Colombo','Conod','Criblez','Crivelli','Dannecker','de Chambrier Amoretti','d''Hooghe ','Eckerz','Elmer','Fässler','Fischer','Flessenkämper','Forsch','Frank'," +
                      "'Furrer','Gast','Gasteyger','Gremminger','Grossenbacher','Grubenmann','Haenggi','Hänggi','Hintermüller','Högger','Huber','Hunziker','Hürlimann','Hüsler, Dipl.phil.nat. Biologin','Imoberdorf'," +
                      "'Jeanneret','Keller','Kläy','König','Kunz','Leiser-Moser','Mäder','Matyassy','Meuli','Meyrat','Monnier','Müller','Nagel','Nepfer','Neuhaus','Noël','Obexer-Ruff'," +
                      "'Papp','Paupe','Pfäffli','Pisan','Portner','Probst','Prodolliet','Salerno','Schaffner','Schärli','Schätzle','Schmidlin','Spaar','Spicher','Staub','Steiner','Sterchi','Taramarcaz'," +
                      "'Thalmann','Vogt','von Allmen','von Greyerz','Weber','Wechsler','Wicht','Wigger')";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Empty all organizations from Nachname field
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Nachname = '' where NameOrganisation != ''";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup name fields
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Basak', Vorname = 'Berivan' where Vorname = 'Basak Berivan';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Cloux', Vorname = 'Lorenz' where Vorname = 'Cloux Lorenz';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Dubois', Vorname = 'Pierre-Yve' where Vorname = 'Dubois Pierre-Yves';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Thoonen-Tornic', Vorname = 'Ana' where Vorname = 'Thoonen-Tornic Ana';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Stettler', Vorname = 'Andreas' where Vorname = 'Stettler Andreas';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Steiner-Beer', Vorname = 'Sandra' where Vorname = 'Steiner-Beer Sandra';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Brogini', Vorname = 'Eliane' where Vorname = 'Brogini Eliane';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Pickel', Vorname = 'Madeleine' where Vorname = 'Pickel Madeleine';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Hufschmid', Vorname = 'Simon' where Vorname = 'Hufschmid Simon';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Herren', Vorname = 'Dominik' where Vorname = 'Herren Dominik';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Koller', Vorname = 'Thomas' where Vorname = 'Koller Thomas';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Müller', Vorname = 'Simon' where Vorname = 'Müller Simon';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Frey', Vorname = 'Priska' where Vorname = 'Frey Priska';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Nachname = 'Gian-Luca', Vorname = 'Marsella' where Vorname = 'Gian-Luca Marsella';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Vorname = 'Agathe', Nachname = 'Grunder' where Vorname = 'Grunder Agathe';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Vorname = 'Beat', Nachname = 'Wandeler', Briefanrede = 'Herr' where Vorname = 'Herr Beat Wandeler'";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // completely empty record, delete
        commandText = "DELETE FROM [PAPGBK].[dbo].[Sekretariat] WHERE Id = 6";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // remove all the null as string
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Briefanrede = NULL where Briefanrede = 'null'";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // fix PO boxes...
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Strasse = NULL, Postfach = 'Postfach' where Strasse = 'Postfach'";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // ... and the french counterparts
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Strasse = NULL, Postfach = 'Case postale' where Strasse = 'Case postale';";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // fill the new Sektion field whenever needed
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Amt für Landschaft und Natur', Strasse = '' where Strasse = 'Amt für Landschaft und Natur';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'BAFU, Abteilung Wald', Strasse = '' where Strasse = 'BAFU . Abteilung Wald';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'BAFU, Abt. Gefahrenprävention', Strasse = '' where Strasse = 'BAFU, Abt. Gefahrenprävention';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'BAFU, Abteilung Wald', Strasse = '' where Strasse = 'BAFU, Abteilung Wald';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'BAFU, Abteilungschef Wald', Strasse = '' where Strasse = 'BAFU, Abteilungschef Wald';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'BAFU, Sektion umweltgefährdende Stoffe', Strasse = '' where Strasse = 'BAFU, Sektion umweltgefährdende Stoffe';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Bundesamt für Strassen ASTRA', Strasse = '' where Strasse = 'Bundesamt für Strassen ASTRA';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Bundesamt für Umwelt BAFU', Strasse = '' where Strasse = 'Bundesamt für Umwelt BAFU';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Dip. Finanze ed Economia', Strasse = '' where Strasse = 'Dip. Finanze ed Economia';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Direzione del diritto internazionale pubblico', Strasse = '' where Strasse = 'Direzione del diritto internazionale pubblico';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Hauptabteilung für die Sicherheit der KNE', Strasse = '' where Strasse = 'Hauptabteilung für die Sicherheit der KNE';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Landwirtschaftsamt', Strasse = '' where Strasse = 'Landwirtschaftsamt';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'swisstopo', Strasse = '' where Strasse = 'swisstopo';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Bundesamt für Sozialversicherungen BSV', Vorname = '' where Vorname = 'Bundesamt für Sozialversicherungen BSV';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'c/o Bundesamt für Gesundheit BAG', Vorname = '' where Vorname = 'c/o Bundesamt für Gesundheit BAG';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Information', Vorname = '' where Vorname = 'Information';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Sektion Vollzug Gesundheitsberufe', Vorname = '' where Vorname = 'Sektion Vollzug Gesundheitsberufe';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'pa UNIL - Dept. de Pharmacologie et Toxicologie', Vorname = '' where Vorname = 'pa UNIL - Dept. de Pharmacologie et Toxicologie';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Stilllegungsfonds für Kernanlagen', Vorname = '' where Vorname = 'Stilllegungsfonds für Kernanlagen';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Generaldirektion', Vorname = '' where Vorname = 'Generaldirektion';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'GS UVEK', Vorname = '' where Vorname = 'GS UVEK';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Postreg', Vorname = '' where Vorname = 'Postreg';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Sektion Altlasten und Industrieabfälle', Vorname = '' where Vorname = 'Sektion Altlasten und Industrieabfälle';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Abteilung Wald', Vorname = '' where Vorname = 'Abteilung Wald';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Sektion Fischerei und Artenmanagement', Vorname = '' where Vorname = 'Sektion Fischerei undd Artenmanagement';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Sektion Fischerei und Artenmanagement', Vorname = '' where Vorname = 'Sektion Fischerei und Artenmanagement';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'Sektion Präventionsstrategien', Vorname = '' where Vorname = 'Sektion Präventionsstrategien';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = 'c/o Martin Müller, Notariat Advokatur', Vorname = '' where Vorname = 'c/o Martin Müller, Notariat Advokatur';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set NameOrganisation = 'BAG Direktionsbereich Öffentliche Gesundheit', Nachname = 'Wüthrich', Vorname = 'Astrid' where NameOrganisation = 'Astrid Wüthrich';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Strasse = Firma, Firma = '' where Firma in ('Rue du Bugnon 27', 'Eichenweg 5');" +
                      "update [PAPGBK].[dbo].[Sekretariat] set NameOrganisation = Firma, Firma = '' where NameOrganisation = '' and Firma != ''" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = Firma, Firma = '' where Sektion is null and Firma != '';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = '', Firma = '' where NameOrganisation = 'Sekretariat der Wettbewerbskommission';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = '', Firma = '' where NameOrganisation = 'Schweizerischer Wissenschaftsrat SWR';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Sektion = '', Firma = '' where NameOrganisation = 'ESBK';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // fix Addresses in PO box fields
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Strasse = 'Eichenweg 5', Postfach = '' where Postfach = 'Eichenweg 5';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Strasse = 'Hochschulstrasse 6', Postfach = '' where Postfach = 'Hochschulstrasse 6';" +
                      "update [PAPGBK].[dbo].[Sekretariat] set Strasse = 'Seftigenstrasse 264', Postfach = '' where Postfach = 'Seftigenstrasse 264';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup genders male in contact points...
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set GeschlechtId = 0 where GeschlechtId = 2 and Vorname in ( " +
                      "'Andreas', 'Beat', 'Daniel', 'Guido', 'Guillermo', 'Pascal', 'Pierre', 'Richard', 'Thomas', 'Ulrich')"
            ;
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup genders female in contact points...
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set GeschlechtId = 1 where GeschlechtId = 2 and Vorname in ( " +
                      "'Agathe','Andrea','Annette','Irène','Margita','Miriam','Monica','Romina','Ruth','Sonja','Sophie')"
            ;
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup genders male in persons...
        commandText = "update [PAPGBK].[dbo].[Person] set GeschlechtId = 0 where GeschlechtId = 2 and Vorname in ( " +
                      "'Alberto', 'Andreas ', 'Arthur', 'Benjamin ', 'Bernd', 'Chatelain Richard', 'Cornelis', 'Daniel', 'Ernst', 'Heinz', 'Hermann', 'Hutzli Peter'," +
                      "'Jiri', 'Joan S', 'Jost', 'Jürg', 'Kovari', 'Leo', 'Manfred', 'Mark', 'Markus', 'Martin ', 'Messmer Werner', 'Michael ', 'Müller Erwin', 'Paul', 'Peter'," +
                      "'René', 'Ronald', 'Samuel ', 'Staehelin Adrian', 'Sulser Giorgio', 'Thomas', 'Ulrich', 'Urs', 'Walter')";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup genders female in persons...
        commandText = "update [PAPGBK].[dbo].[Person] set GeschlechtId = 1 where GeschlechtId = 2 and Vorname in ( " +
                      "'Anne Cécile', 'Barbara', 'Marlène', 'Ruth', 'Susan')";
        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // fix Addresses in American/French style "34, rue du Gare" to "rue du gare 34"
        commandText = "UPDATE Adresse SET Strasse = LTRIM(RTRIM(SUBSTRING(Strasse, CHARINDEX(',', Strasse) + 1, LEN(Strasse)) + ' ' + LEFT(Strasse, CHARINDEX(',', Strasse) - 1))) " +
                      "WHERE (ISNUMERIC(LEFT(Strasse, 1)) = 1 AND SUBSTRING(Strasse, 2, 1) = ',') OR (ISNUMERIC(LEFT(Strasse, 2)) = 1 AND SUBSTRING(Strasse, 3, 1) = ',');\n";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // ... and the same, if there is no comma: "34 rue du Gare" to "rue du gare 34 "
        commandText = "UPDATE Adresse SET Strasse = LTRIM(RTRIM(STUFF(Strasse, 1, PATINDEX('%[^0-9]%', Strasse) - 1, '') + ' ' + LEFT(Strasse, PATINDEX('%[^0-9]%', Strasse) - 1))) "
                      + "WHERE (Strasse LIKE '[0-9] %' OR Strasse LIKE '[0-9][0-9] %');";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // ... and the same, if there is 3 numbers, with or without comma: "134, rue du Gare" to "rue du gare 134 "
        commandText = "update Adresse set Strasse = LTRIM(RTRIM(SUBSTRING(Strasse, PATINDEX('%[^0-9A-Z-]%', Strasse) + CASE WHEN SUBSTRING(strasse, PATINDEX('%[^0-9A-Z-]%', strasse), 1) = ',' THEN 2 ELSE 1 END, LEN(strasse)) + ' ' + LEFT(strasse, PATINDEX('%[^0-9A-Z-]%', strasse) - 1))) " +
                      "WHERE PATINDEX('[0-9]%', strasse) = 1 AND PATINDEX('%[^0-9A-Z-]%', strasse) > 1 AND Kanton != 'Ausl';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup phone numbers and mobiles according to the new format
        // Replace all zeros in brackets with nothing
        commandText = "update[PAPGBK].[dbo].[Adresse] set Telefon = Replace(telefon, '(0)', '') where telefon like '%(0)%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // remove fix 41 at the beginning, will be completed later
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = STUFF(Telefon, 1, 2, '') where telefon like '41%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Remove all slashes
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = Replace(telefon, '/', ' ')  where telefon  like '%/%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Remove all double zeroes at the beginning
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = STUFF(Telefon, 1, 2, '+') where telefon  like '00%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Remove 3 spaces
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = Replace(telefon, '   ', ' ')  where telefon  like '%   %'";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Remove invalid content (as good as it gets)
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = '+41 61 267 35 92' where telefon  = '061 267 35 92 34 45';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Complete the one with blank at the beginning
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = '+41 32 471 10 64' where telefon  = ' 032 471 10 64';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // now complete the records
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = STUFF(Telefon, 1, 1, '+41 ') where telefon is not null and telefon != '' and telefon not like '%+%' and telefon not like '00%' and telefon not like '%41%' and telefon like '0%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Add +41 to the rest
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = STUFF(Telefon, 1, 1, '+41 ') where telefon like '0%' and telefon not like '00%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // and the ones with blank
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = STUFF(Telefon, 1, 1, '+41 ') where telefon like ' %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // the ones with (0)
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = Replace(telefon, '(0)', '')  where telefon like '%(0)%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Add a blank after "+41" to the ones without
        commandText = "update [PAPGBK].[dbo].[Adresse] set Telefon = STUFF(Telefon, 1, 3, '+41 ') where Telefon like '+41%' and Telefon not like '+41 %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // And the same for the mobile
        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = STUFF(Mobile, 1, 2, '') where Mobile like '41%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = Replace(Mobile, '/', ' ') where Mobile  like '%/%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = STUFF(Mobile, 1, 2, '+') where Mobile  like '00%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = Replace(Mobile, '   ', ' ')  where Mobile  like '%   %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = STUFF(Mobile, 1, 1, '+41 ') where Mobile is not null and Mobile != '' and Mobile not like '%+%' and Mobile not like '00%' and Mobile not like '%41%' and Mobile like '0%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = STUFF(Mobile, 1, 1, '+41 ') where Mobile like '0%' and Mobile not like '00%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = STUFF(Mobile, 1, 1, '+41 ')  where Mobile like ' %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = STUFF(Mobile, 1, 2, '') where Mobile  like '41%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        commandText = "update [PAPGBK].[dbo].[Adresse] set Mobile = STUFF(Mobile, 1, 3, '+41 ') where mobile like '+41%' and mobile not like '+41 %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Cleanup phone numbers and mobiles of contact points according to the new format
        // Replace the String "Tel. " :)
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = Replace(Tel, 'Tel. ', '') where Tel like '%Tel.%';;";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Replace (0)
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = Replace(Tel, '(0)', '') where Tel like '%(0)%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Replace the ones starting with 41 (completed later)
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = STUFF(Tel, 1, 2, '') where Tel like '41%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Replace Slashes
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = Replace(Tel, '/', ' ') where Tel  like '%/%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Replace the 00... records
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = STUFF(Tel, 1, 2, '+') where Tel  like '00%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Replace 3 blanks with 1
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = Replace(Tel, '   ', ' ') where Tel  like '%   %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Replace 2 blanks with 1
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = Replace(Tel, '  ', ' ') where Tel  like '%  %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Replace the ones starting with blank
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = STUFF(Tel, 1, 1, '') where Tel  like ' %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // No comment to that
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = '+41 31 322 99 79' where Tel = '031 3229979 031 32';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // was left over after fixes
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = '+41 58 469 28 87' where Tel = '58 469 28 87';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // start all the records with "+41 "
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = STUFF(Tel, 1, 1, '+41 ') where Tel is not null and Tel != '' and Tel not like '%+%' and Tel not like '00%' and Tel not like '%41%' and Tel like '0%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // and the ones starting with 0 but not 00
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = STUFF(Tel, 1, 1, '+41 ') where Tel like '0%' and Tel not like '00%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // add a blank to "+41 "
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Tel = STUFF(Tel, 1, 3, '+41 ') where Tel like '+41%' and Tel not like '+41 %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Mobile: fix the ones with "07"
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Mobile = Stuff(Mobile, 1, 2, '+41 7') where Mobile  like '07%';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        // Add blank to mobile after "+41 "
        commandText = "update [PAPGBK].[dbo].[Sekretariat] set Mobile = STUFF(Mobile, 1, 3, '+41 ') where Mobile like '+41%' and Mobile not like '+41 %';";

        using (var command = new SqlCommand(commandText, connection))
        {
            command.ExecuteNonQuery();
        }

        logger.LogInformation("Helper tables created and filled successfully.");
    }

    public async Task EmptyDatabase()
    {
        logger.LogInformation("Empty target database before migration.");

        // INFO: instead of dropping the whole database (with EnsureDeleted) it is more sane to leave the database intact and just drop/create the schema.
        // CF target environment puts special database in place and does not allow the user to create / drop databases.
        const string schemaName = "data";
        await using var command = targetContext.Database.GetDbConnection().CreateCommand();

        command.CommandText = $"SELECT count(schema_name) FROM information_schema.schemata WHERE schema_name = '{schemaName}';";
        command.CommandType = CommandType.Text;

        if (command.Connection == null)
        {
            logger.LogInformation("Error! Connection to target database could not be opened.");
            throw new InvalidOperationException("Command with no connection.");
        }

        if (command.Connection.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync();
        }

        var schemaCount = (long)(await command.ExecuteScalarAsync())!;

        // When DB is not existing, we need this.
        await targetContext.Database.MigrateAsync();

        if (schemaCount > 0)
        {
            logger.LogInformation("Found schema '{SchemaName}', now clearing tables...", schemaName);
            // in case the GeneralElection is running, we also have to clean these table! Ensure to reset the running flag on the TermOfOfficeData table!
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"worklist_tasks\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"membership_candidate_log_messages\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"membership_candidates\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"general_election_committees\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"eiam_assignments\" WHERE Role = 'Secretariat'");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"appointment_decisions\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"contact_points\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"interests\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"memberships\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"committees\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"persons\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"addresses\"");
            await targetContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"{schemaName}\".\"document_storages\"");
            // reset a running general election
            await targetContext.Database.ExecuteSqlRawAsync($"UPDATE  \"{schemaName}\".\"term_of_office_dates\" set is_general_election = null where is_general_election = true");

            logger.LogInformation("Cleared tables");
        }
    }

    public async Task DataFixesInTargetAfterMigration()
    {
        // Here we update migrated data with stuff, which turned up during the implementation

        const string schemaName = "data";
        await using var command = targetContext.Database.GetDbConnection().CreateCommand();

        command.CommandText = $"SELECT count(schema_name) FROM information_schema.schemata WHERE schema_name = '{schemaName}';";
        command.CommandType = CommandType.Text;

        if (command.Connection == null)
        {
            logger.LogInformation("Error! Connection to target database could not be opened.");
            throw new InvalidOperationException("Command with no connection.");
        }

        if (command.Connection.State != ConnectionState.Open)
        {
            await command.Connection.OpenAsync();
        }

        var sqlQuery = "" +
                       "UPDATE data.persons SET no_interest = true WHERE id in (" +
                       "SELECT person_id FROM data.interests " +
                       "WHERE (text = 'Keine Interessenbindungen' or text = 'keine Interessenbindungen' OR text = 'Keine Interessensbindungen' OR text = 'Keine Interssenbindungen') " +
                       "AND interest_committee_id = '6f30d4a5-03dc-490b-8360-1c4e72c0363a' " +
                       "AND interest_function_id = 'b33e1480-278d-439b-99aa-f94f382e0246' " +
                       "AND interest_legal_form_id = 'aafe7fb7-da54-4398-b330-2863366e839b'" +
                       ");";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        sqlQuery = "" +
                   "DELETE FROM data.interests " +
                   "WHERE (text = 'Keine Interessenbindungen' or text = 'keine Interessenbindungen' OR text = 'Keine Interessensbindungen' OR text = 'Keine Interssenbindungen')" +
                   "AND interest_committee_id = '6f30d4a5-03dc-490b-8360-1c4e72c0363a'" +
                   "AND interest_function_id = 'b33e1480-278d-439b-99aa-f94f382e0246'" +
                   "AND interest_legal_form_id = 'aafe7fb7-da54-4398-b330-2863366e839b';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        sqlQuery = "UPDATE data.memberships SET function_id = 'A282A0CD-4A7D-48B6-9B52-9B216E9454FE' WHERE function_id = 'BE7006BC-33B8-4621-A2E7-3A06956A7F21';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '17F63CD3-F254-4E6E-BD84-37311B38041C' WHERE function_id = '50231C33-E549-4D1D-8484-1A11884B8030';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = 'C54D3FDD-5819-49C5-9848-8A6D3F892925' WHERE function_id = '31AF5151-5A15-45A7-82CF-84B2EB0FB8B8';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = 'B0D157EE-A887-49DD-AC09-B05A39F92CB5' WHERE function_id = '002E07A4-DBF8-47EA-A286-B191718A96E3';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '4E5DC4E3-4563-4B8F-87AB-0F70B138927F' WHERE function_id = '62BB29AC-C193-474B-8A12-FC2E88A5D072';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '3949A0FD-6961-4BFE-9046-8A2BFD86F9CF' WHERE function_id = '39257906-C1BF-4008-B01F-4CECAA640681';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '7944D508-C784-4FA9-8885-6BFEB41EC0BD' WHERE function_id = 'DA3C2E89-A3CA-44C6-AACD-1126C23B8C2B';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = 'C36B8B7A-FD7F-4056-909D-9F4BC4C91380' WHERE function_id = '40BC9B3E-EEC2-49A4-A260-7BA84BA763EE';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '2991B76B-CE7E-44F0-9255-D2A57A567C2C' WHERE function_id = '1172AF61-3D3F-4F18-B352-092A950D1A01';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '61F627CF-43C5-4746-9D29-4202A4DA0C27' WHERE function_id = '7492E69B-7A21-4F83-ACBF-BB3BF9643EBA';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '255D79D3-00FB-467E-8B2F-1F01683E0B27' WHERE function_id = 'AAFF2635-1AF3-4421-B5F3-0C08E00A6470';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '3812EA80-BF90-41E7-AF4A-E65B45779C27' WHERE function_id = '349AB80E-B9D4-4F18-933F-F7D8D0EC4B33';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '324C68B8-E136-4D98-B76E-2488D794E0A4' WHERE function_id = 'D7099A50-8426-4A8D-943D-2022CEBB24A7';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = '3826BC5D-5ACE-4371-891B-4AD225186450' WHERE function_id = '97AFD32F-418D-4C36-AA17-47BF70C4AFC9';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = 'A4971074-C1CC-4372-8AB3-A8FF0E9182E8' WHERE function_id = '93F54C77-E851-4E39-AB0B-ED40D0B56C50';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = 'FAA8EDC2-F784-4E4A-B308-B4DEC1C56A54' WHERE function_id = '0C3900CC-D582-4995-A174-3C9EFF5C4B66';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
        sqlQuery = "UPDATE data.memberships SET function_id = 'C2E8D46D-D827-412E-997B-D8AFADAF41A7' WHERE(end_date > now() OR end_date IS NULL) AND function_id NOT IN ('A282A0CD-4A7D-48B6-9B52-9B216E9454FE','43B6EA02-0933-4E6E-83CB-62BF70405FB9','17F63CD3-F254-4E6E-BD84-37311B38041C');";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        // just to be sure, update any, who might be on the doubled office
        sqlQuery = "update data.persons set office_id = 'a7d800aa-be0d-4ac9-bae3-85786aba0b48' where office_id = '2b0a5c57-e6b5-4fdb-9a34-3a6ad9b23f3a';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        sqlQuery = "update data.committees set office_id = 'a7d800aa-be0d-4ac9-bae3-85786aba0b48' where office_id = '2b0a5c57-e6b5-4fdb-9a34-3a6ad9b23f3a';";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        // this updates the results from the UID check to the existing interests. As the migrations are executed before the DataMigration, the Update is not possible in EF.
        sqlQuery = await File.ReadAllTextAsync("Scripts/UpdateInterests.sql");
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        sqlQuery = "update data.interests set interest_text =  uid_organisation_name_closest_match  where uid_organisation_name_closest_match is not null and uid_organisation_name_closest_match != ''";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        // As interest function "Beirat" is not active anymore, we set the values of active persons to "Mitglied" and Committee to "Beirat" (BKDO-2230)
        sqlQuery = "update data.interests set interest_function_id = 'a6f17fdc-f15a-42c2-98f1-1e4f7256696d'," +
            "interest_committee_id = '99be6d2f-6dff-4417-9bd6-7232e820b8b4'" +
            "where interest_function_id = '71d18d15-9003-43fd-907c-e740683e1690'" +
            "and id in " +
            "('194b47d0-7804-43b5-9b77-1459e33bc067'," +
            "'d95c5fc5-04b9-42d9-af99-4b15cb78b332'," +
            "'492fb14a-f631-41e2-ae08-7f8600f41383'," +
            "'2ad58c51-80df-4a7b-8f5f-c96abce7b59a'," +
            "'d87367ca-8acc-4f61-aabe-11d91ccbb95f'," +
            "'903aa952-7f32-4332-b327-b80ea080dde9'," +
            "'9971b44b-9620-4bf8-93ec-dc65337eaec8'," +
            "'23a54b04-7917-4d17-b482-cae769d7b985'," +
            "'254a6b64-3a07-4684-a949-5aac68470dfc'," +
            "'eeba607d-6cba-49f6-abbf-2cb520b0d015'," +
            "'7dab2be6-e3fc-411e-b1d7-7568837f53ab'," +
            "'d4a992c9-1afb-431b-a591-354787d50535'," +
            "'60e08b35-b413-4f99-b034-954fbc732d0a'," +
            "'8020b9b0-da76-4d25-8e23-b8454ce90b95'," +
            "'3f8d16b4-8d1f-421c-8d09-e7b5ae15a640'," +
            "'2758dfd4-3b19-4adf-b959-c0e55984f3bc'," +
            "'f56b379e-8b33-469f-b0ce-0097a926ec80'" +
            ");";

        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        // Set membership correlation flag for persons with federal duty
        sqlQuery = "UPDATE data.memberships m SET in_correlation_with_federal_duty = true FROM data.persons p WHERE p.id = m.person_id AND p.federal_duty = true;";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);

        // Set all the country codes with a 4 digit ZIP to 'CH'
        sqlQuery = "UPDATE data.addresses set country_code = 'CH' where country_code = '' and LENGTH(zip) = 4;";
        await targetContext.Database.ExecuteSqlRawAsync(sqlQuery);
    }

    private static string FillOfficeMapping()
    {
        return "" +
               "INSERT INTO OfficeMapping VALUES ('2d7992e7-0e23-4e2b-b628-a358b5f30111', 42)" +
               "INSERT INTO OfficeMapping VALUES ('53803b38-cfe2-4f52-8ad4-071c79d8a510', 46)" +
               "INSERT INTO OfficeMapping VALUES ('bba51d65-38b6-4804-b4ae-40ace3d8c0ac', 47)" +
               "INSERT INTO OfficeMapping VALUES ('e6f3a670-05a7-4b53-9fda-005faaae612d', 48)" +
               "INSERT INTO OfficeMapping VALUES ('87292271-4828-4d40-aaca-c54adb0ae6c6', 45)" +
               "INSERT INTO OfficeMapping VALUES ('565064b7-542d-4a88-be30-bc825f3eb8d7', 44)" +
               "INSERT INTO OfficeMapping VALUES ('2c925cd4-8a53-4c82-a752-0987359bed11', 9)" +
               "INSERT INTO OfficeMapping VALUES ('454e91ca-8df2-4545-b945-1f5eb9437602', 19)" +
               "INSERT INTO OfficeMapping VALUES ('f29ab7c9-f0eb-45c6-bde5-ad365ed805ec', 15)" +
               "INSERT INTO OfficeMapping VALUES ('e7e37d90-e446-4fa3-9efd-4fb9dba18717', 0)" +
               "INSERT INTO OfficeMapping VALUES ('0b021f9f-3f0e-4a55-97f5-8ae78166087e', 0)" +
               "INSERT INTO OfficeMapping VALUES ('b90c9ce0-1dae-4bec-8e74-0b81adf4f695', 14)" +
               "INSERT INTO OfficeMapping VALUES ('4903d59d-071c-49ae-910f-02df552499bf', 18)" +
               "INSERT INTO OfficeMapping VALUES ('da884156-4d9a-4bcd-9f5d-c93661344c8c', 16)" +
               "INSERT INTO OfficeMapping VALUES ('0ea9da45-118e-47df-9087-32a526b33bf7', 17)" +
               "INSERT INTO OfficeMapping VALUES ('68da9ecb-9dbe-4647-bd63-3d85ecc67ad9', 12)" +
               "INSERT INTO OfficeMapping VALUES ('231e7093-23f8-42b3-adad-4388ba1986e3', 11)" +
               "INSERT INTO OfficeMapping VALUES ('bc05061d-99ea-4cd9-b4f3-bcb5c92569d5', 13)" +
               "INSERT INTO OfficeMapping VALUES ('e927d1ab-201b-46be-b357-a304595ad49c', 0)" +
               "INSERT INTO OfficeMapping VALUES ('1c355e7b-4299-4e51-a393-7fadc61b27ba', 6)" +
               "INSERT INTO OfficeMapping VALUES ('f8f1a95a-cd0f-4a0c-aaac-fe037ab89c46', 7)" +
               "INSERT INTO OfficeMapping VALUES ('4e8e4661-7963-4502-94ec-f102d354d4ce', 0)" +
               "INSERT INTO OfficeMapping VALUES ('9c98d535-0c3d-422b-83ac-2928435851e8', 8)" +
               "INSERT INTO OfficeMapping VALUES ('b1f4c68c-a8e2-48ff-8780-0d28b717c070', 0)" +
               "INSERT INTO OfficeMapping VALUES ('63201173-e911-4263-b42b-5393ebd959ef', 0)" +
               "INSERT INTO OfficeMapping VALUES ('9e8fc982-a439-40da-8a7f-3ee16cd509d6', 0)" +
               "INSERT INTO OfficeMapping VALUES ('94b7873f-ebbc-4ae6-9c55-902eeadf0120', 0)" +
               "INSERT INTO OfficeMapping VALUES ('c57856b8-e2f4-4992-bf61-5f98b35aaf38', 0)" +
               "INSERT INTO OfficeMapping VALUES ('fc644f51-258a-4e2c-8b07-7325080a03ef', 0)" +
               "INSERT INTO OfficeMapping VALUES ('52df43c7-9e50-48d7-9186-411dd622529c', 37)" +
               // 39 "Luftwaffe" is mapped to "Gruppe Verteidigung"
               "INSERT INTO OfficeMapping VALUES ('2a1a785e-6f4e-4c17-a305-71d707edc812', 39)" +
               "INSERT INTO OfficeMapping VALUES ('c635ba6b-045e-4096-a74f-b71f836bd993', 0)" +
               "INSERT INTO OfficeMapping VALUES ('c6ce0072-3aab-4dce-8b88-c459406ecd0d', 0)" +
               "INSERT INTO OfficeMapping VALUES ('1745b0ee-620b-4aec-8850-3503e0ce86b0', 0)" +
               // 41 "LBA" is mapped to "Gruppe Verteidigung"
               "INSERT INTO OfficeMapping VALUES ('2a1a785e-6f4e-4c17-a305-71d707edc812', 41)" +
               "INSERT INTO OfficeMapping VALUES ('3d441f62-a537-4a53-b4b9-f1f90b082efa', 38)" +
               "INSERT INTO OfficeMapping VALUES ('459254f4-a9d0-4756-9075-41674fa4d536', 50)" +
               "INSERT INTO OfficeMapping VALUES ('8939ff5b-e93e-498b-ac68-9f233bad357b', 40)" +
               "INSERT INTO OfficeMapping VALUES ('b01f59f7-7502-4c71-ba35-ba55f3ecc010', 0)" +
               "INSERT INTO OfficeMapping VALUES ('a7d800aa-be0d-4ac9-bae3-85786aba0b48', 0)" +
               "INSERT INTO OfficeMapping VALUES ('8e4c3621-10d0-428f-af4c-aa07f0f5a52e', 0)" +
               "INSERT INTO OfficeMapping VALUES ('2b0a5c57-e6b5-4fdb-9a34-3a6ad9b23f3a', 0)" +
               "INSERT INTO OfficeMapping VALUES ('0a6aa17d-8ef6-42f0-9712-f75c13531e7f', 20)" +
               "INSERT INTO OfficeMapping VALUES ('cc077087-4b6c-43da-a4d3-3b4ffc47b599', 26)" +
               "INSERT INTO OfficeMapping VALUES ('700196ad-171e-4d08-878a-94b8afd5240b', 28)" +
               "INSERT INTO OfficeMapping VALUES ('558d0b0c-2186-4864-af1b-c36b14fd6f0d', 22)" +
               "INSERT INTO OfficeMapping VALUES ('574460e9-ce22-4294-a54f-4f4b3a3bbdb1', 23)" +
               "INSERT INTO OfficeMapping VALUES ('60181868-2c5b-48bf-afdb-5ca7949f3f0a', 0)" +
               "INSERT INTO OfficeMapping VALUES ('ccb89b97-f263-4696-ba31-c361ffc302a4', 30)" +
               "INSERT INTO OfficeMapping VALUES ('1f6cf7e3-9a1c-4d53-b268-e6ac6b48f332', 31)" +
               "INSERT INTO OfficeMapping VALUES ('ff3dbf60-478d-4069-98e6-83b712c4a240', 32)" +
               "INSERT INTO OfficeMapping VALUES ('44458477-2241-447e-bcc1-0715a47d5fa7', 21)" +
               "INSERT INTO OfficeMapping VALUES ('8ae5fcae-01db-486f-bf4d-1638e02674b1', 33)" +
               "INSERT INTO OfficeMapping VALUES ('4c372f4e-d25a-4238-bd1b-11d92f494e42', 27)" +
               "INSERT INTO OfficeMapping VALUES ('2a1d1b3d-b8ee-450d-a780-f585c7053f5f', 1)" +
               "INSERT INTO OfficeMapping VALUES ('76b69f6d-3384-471c-9b1d-6f0ad69ea8c7', 0)" +
               "INSERT INTO OfficeMapping VALUES ('ba9c17e6-7819-4efb-9722-ebf01e01dd3b', 2)" +
               "INSERT INTO OfficeMapping VALUES ('233920fe-ef89-4d2f-b700-f6db0f51abd2', 3)" +
               "INSERT INTO OfficeMapping VALUES ('95f92c87-3fe3-4d29-8f4b-5cd0cdc122ac', 4)" +
               "INSERT INTO OfficeMapping VALUES ('abbec37b-92f6-4ba8-aab8-17fe61a2d268', 0)" +
               "INSERT INTO OfficeMapping VALUES ('233fabe1-8037-42d9-bb5e-1c5a0bb40174', 5)" +
               "INSERT INTO OfficeMapping VALUES ('c99a0627-ab5b-4f54-9866-103dd8f58d75', 0)" +
               "INSERT INTO OfficeMapping VALUES ('c762d893-2b60-4c50-8ba2-89bb6655ef6d', 0)" +
               "INSERT INTO OfficeMapping VALUES ('5802e496-9da9-4b71-b121-0064cd735ed0', 0)" +
               "INSERT INTO OfficeMapping VALUES ('f41baca5-056f-4fce-bed2-96905a9cb2d8', 0)" +
               "INSERT INTO OfficeMapping VALUES ('3ad0d0dd-55e1-42c7-9524-7cefe913fa98', 0)" +
               "INSERT INTO OfficeMapping VALUES ('2b764a82-e1b3-49dc-8d19-ca850a14a12e', 0)" +
               "INSERT INTO OfficeMapping VALUES ('596c8b27-4470-49d7-894a-509357695139', 0)" +
               "INSERT INTO OfficeMapping VALUES ('52c2ee1b-9938-4437-8815-98f24e6906ff', 0)" +
               "INSERT INTO OfficeMapping VALUES ('4484dbdd-7711-4235-afb0-eba47e263556', 0)" +
               "INSERT INTO OfficeMapping VALUES ('e004086f-2487-482b-aad8-c11b43d3345d', 0)" +
               "INSERT INTO OfficeMapping VALUES ('49170c0c-0c83-4983-802e-deb5839015dd', 0)" +
               "INSERT INTO OfficeMapping VALUES ('7deba95a-49a4-4d6e-88d1-196c0bb509c7', 0)" +
               "INSERT INTO OfficeMapping VALUES ('3e0b0e20-0d10-42ff-a248-7ce9da165d0b', 0)" +
               "INSERT INTO OfficeMapping VALUES ('5f48561f-f036-4b0f-9830-63cd155d3ecb', 0)" +
               "INSERT INTO OfficeMapping VALUES ('f50f34e1-4515-43c1-a3b3-7d9bb6f90c53', 34)" +
               "INSERT INTO OfficeMapping VALUES ('7d3a8bf5-9b35-4e92-9f57-fc814739f576', 54)" +
               "INSERT INTO OfficeMapping VALUES ('202b2a1f-af42-47e5-8222-7294a00193af', 51)" +
               "INSERT INTO OfficeMapping VALUES ('cdb04668-bfdf-4b9c-8c13-adbd59db25a7', 53)" +
               "INSERT INTO OfficeMapping VALUES ('8647d45a-0b7c-4401-8cdb-3734f6c2c544', 55)" +
               "INSERT INTO OfficeMapping VALUES ('6aeca62f-e75e-4f72-b8ad-a8d60e4b3bcc', 52)" +
               "INSERT INTO OfficeMapping VALUES ('2aef4107-3dea-4fed-b21f-57158df8293a', 36)" +
               "INSERT INTO OfficeMapping VALUES ('a831a490-f9b7-4060-8e41-3b4bb17d8194', 35)" +
               "INSERT INTO OfficeMapping VALUES ('3d1ad068-5fb8-4f2c-8ed9-6b80de119900', 0)" +
               "INSERT INTO OfficeMapping VALUES ('b9b83867-5db5-4c32-a833-c643e3aebe35', 0)" +
               "INSERT INTO OfficeMapping VALUES ('ea5b85a3-7de0-4d74-8463-c7565b6ed979', 0)" +
               "INSERT INTO OfficeMapping VALUES ('2c925cd4-8a53-4c82-a752-0987359bed11', 10)";
    }
}
