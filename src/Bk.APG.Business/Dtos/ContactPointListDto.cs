namespace Bk.APG.Business.Dtos;

public class ContactPointListDto
{
    public required Guid Id { get; set; }
    public required string ContactPointType { get; set; }
    public string? CompanyName { get; set; }
    public string? Section { get; set; }
    public DateOnly BeginDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? ZipCity { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? PersonName { get; set; }

    public bool IsActive
    {
        get
        {
            return EndDate is null || EndDate > DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
