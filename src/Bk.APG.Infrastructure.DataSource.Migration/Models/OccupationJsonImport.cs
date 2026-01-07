namespace Bk.APG.Infrastructure.DataSource.Migration.Models;

public class OccupationJsonImport;

public class OccupationText
{
    public string de { get; set; } = null!;
    public string fr { get; set; } = null!;
    public string it { get; set; } = null!;
}

public class OccupationAnnotation
{
    public OccupationText text { get; set; } = null!;
    public string type { get; set; } = null!;
}

public class OccupationRecord
{
    public List<OccupationAnnotation> annotations { get; init; } = [];
    public string code { get; set; } = null!;
}

public class Root
{
    public List<OccupationRecord> data { get; init; } = [];
}
