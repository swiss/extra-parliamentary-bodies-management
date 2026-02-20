using System.Text;
using System.Text.Json;
using Bk.APG.Business.Repositories;
using Bk.APG.Infrastructure.DataSource.Migration.Models;
using Bk.APG.Infrastructure.DataSource.Repositories;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public class OccupationImportService : IOccupationImportService
{
    private readonly ILogger<OccupationImportService> _logger;
    private readonly IOccupationRepository _occupationRepository;

    public OccupationImportService(ILogger<OccupationImportService> logger, IOccupationRepository occupationRepository)
    {
        _logger = logger;
        _occupationRepository = occupationRepository;
    }

    public void MigrateOccupationsFromExcelSource()
    {
        // this function reads the excel file and creates insert scripts, which then have to be copied to a EF migration.
        _logger.LogInformation("Start migration occupations from Excel file");

        var filePath = "e:\\temp\\occupations.xlsx";
        var sb = new StringBuilder();

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(new FileInfo(filePath));
        var worksheet = package.Workbook.Worksheets[0]; // First sheet
        var rowCount = worksheet.Dimension.Rows;

        for (var row = 2; row <= rowCount; row++) // Assuming row 1 is headers
        {
            var entry = new ExcelRow
            {
                Code = worksheet.Cells[row, 1].Text,
                TextDe = worksheet.Cells[row, 2].Text,
                TextFr = worksheet.Cells[row, 3].Text,
                TextIt = worksheet.Cells[row, 4].Text,
            };

            var germanTexts = entry.TextDe.Split('|');
            var germanMale = germanTexts[0].Trim();
            var germanFemale = germanTexts.Length > 1 ? germanTexts[1].Trim() : germanMale;

            var frenchTexts = entry.TextFr.Split('|');
            var frenchMale = frenchTexts[0].Trim();
            var frenchFemale = frenchTexts.Length > 1 ? frenchTexts[1].Trim() : frenchMale;

            var italianTexts = entry.TextIt.Split('|');
            var italianMale = italianTexts[0].Trim();
            var italianFemale = italianTexts.Length > 1 ? italianTexts[1].Trim() : italianMale;

            sb.AppendLine(
                "('" + Guid.NewGuid() + "', " +
                "'" + germanMale.Replace("'", "’") + "', " +
                "'" + frenchMale.Replace("'", "’") + "', " +
                "'" + italianMale.Replace("'", "’") + "', " +
                "'', " +
                "'" + germanFemale.Replace("'", "’") + "', " +
                "'" + frenchFemale.Replace("'", "’") + "', " +
                "'" + italianFemale.Replace("'", "’") + "', " +
                "'', " +
                "now(), 'migration', now(), 'migration', '', '', '', '', 0, " +
                "'https://register.ld.admin.ch/i14y/ch-isco-19/" + entry.Code + "', 0),"
            );
        }

        var copyMeInMigration = sb.ToString();
    }

    private class ExcelRow
    {
        public string Code { get; set; } = null!;
        public string TextDe { get; set; } = null!;
        public string TextFr { get; set; } = null!;
        public string TextIt { get; set; } = null!;
    }

    public void MigrateOccupationsFromJsonSource()
    {
        const string occupationUri = "https://register.ld.admin.ch/i14y/ch-isco-19/";

        // this function reads the json file and creates insert scripts, which then have to be copied to a EF migration.
        _logger.LogInformation("Start migration occupations from json file");

        var jsonString = File.ReadAllText("e:\\temp\\occupations_big.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var root = JsonSerializer.Deserialize<Root>(jsonString, options);

        if (root != null)
        {
            var sb = new StringBuilder();

            foreach (var job in root.data)
            {
                var code = job.code;

                // Only import records of this level 33105224
                if (code.Length != 8)
                {
                    continue;
                }

                var uri = occupationUri + code;

                var existing = _occupationRepository.GetByUri(uri);

                if (existing == null)
                {
                    var maleAnnotation = job.annotations.Find(a => a.type == "GENDER_M");
                    var femaleAnnotation = job.annotations.Find(a => a.type == "GENDER_F");

                    if (maleAnnotation != null && femaleAnnotation != null)
                    {
                        var maleDe = maleAnnotation.text != null && maleAnnotation.text.de != null ? maleAnnotation.text.de : code;
                        var maleFr = maleAnnotation.text != null && maleAnnotation.text.fr != null ? maleAnnotation.text.fr : code;
                        var maleIt = maleAnnotation.text != null && maleAnnotation.text.it != null ? maleAnnotation.text.it : code;
                        var femaleDe = femaleAnnotation.text != null && femaleAnnotation.text.de != null ? femaleAnnotation.text.de : code;
                        var femaleFr = femaleAnnotation.text != null && femaleAnnotation.text.fr != null ? femaleAnnotation.text.fr : code;
                        var femaleIt = femaleAnnotation.text != null && femaleAnnotation.text.it != null ? femaleAnnotation.text.it : code;

                        sb.AppendLine(
                            "('" + Guid.NewGuid() + "', " +
                            "'" + maleDe.Replace("'", "’") + "', " +
                            "'" + maleFr.Replace("'", "’") + "', " +
                            "'" + maleIt.Replace("'", "’") + "', " +
                            "'', " +
                            "'" + femaleDe.Replace("'", "’") + "', " +
                            "'" + femaleFr.Replace("'", "’") + "', " +
                            "'" + femaleIt.Replace("'", "’") + "', " +
                            "'', " +
                            "now(), 'migration', now(), 'migration', '', '', '', '', 0, " +
                            "'https://register.ld.admin.ch/i14y/ch-isco-19/" + code + "', 0),"
                        );
                    }
                    // You could also use direct inserts to the table, but then the GUIDs are different in all systems!
                    // var occupation = MigrationMapping.ToOccupation(maleAnnotation, femaleAnnotation, code);
                    // await _occupationRepository.Create(occupation);
                    // await _occupationRepository.CommitChanges();
                }
            }

            var completeScript = sb.ToString();
            File.WriteAllText("e:\\temp\\delta_insert_script_occupations.txt", completeScript);
        }

        _logger.LogInformation("Occupations migrated completely.");
    }

    public async Task CleanUpGermanDuplicates()
    {
        await _occupationRepository.RemoveAllGermanDuplicatesForCleanup();
    }
}
