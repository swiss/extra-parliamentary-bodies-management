namespace Bk.APG.Infrastructure.DataSource.Migration.Services;

public interface IOccupationImportService
{
    void MigrateOccupationsFromJsonSource();
    void MigrateOccupationsFromExcelSource();
    Task CleanUpGermanDuplicates();
}
