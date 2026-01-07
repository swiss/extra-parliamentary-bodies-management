using Bk.APG.Common.Resources;

namespace Bk.APG.Business.Models;

public class EiamAssignment
{
    public static readonly Guid AdminId = new("fc03976b-ee16-4e49-8c23-27938fa5392b");
    public static readonly Guid ApgId = new("f4accd20-067e-49c0-ab68-4f53f0dc01cd");

    public static readonly Guid BkId = new("F143698D-E8A0-4105-B955-996B65F5C287");
    public static readonly Guid EdaId = new("008F58A9-4BF5-45CF-8870-77978836572E");
    public static readonly Guid EdiId = new("0B4FE383-7C96-42CD-9B80-ECCDD570AACD");
    public static readonly Guid EjpdId = new("891CFCDE-FE5B-4D07-BA7A-89C7E698C2AB");
    public static readonly Guid VbsId = new("B5C100EF-EC93-4FCB-AB11-D48A6DD9D373");
    public static readonly Guid EfdId = new("5DE0AFDC-DD5D-4CA5-806C-F56BB5E57583");
    public static readonly Guid WbfId = new("4AE2A423-2BAC-4B88-9334-65114EE08812");
    public static readonly Guid UvekId = new("C1CAC091-0F9C-4B14-95DB-EDBBC2F2E972");

    public required Guid Id { get; init; }
    public required string ExternalId { get; init; }
    public required Role Role { get; init; }

    public EiamAssignment? Parent { get; init; }
    public Guid? ParentId { get; init; }
    public ICollection<EiamAssignment> Children { get; init; } = new List<EiamAssignment>();

    public Department? Department { get; init; }
    public Guid? DepartmentId { get; init; }
    public Office? Office { get; init; }
    public Guid? OfficeId { get; init; }
    public Committee? Committee { get; init; }
    public Guid? CommitteeId { get; init; }

    public ICollection<WorklistTask> ReceivedTasks { get; init; } = new List<WorklistTask>();
    public ICollection<WorklistTask> DistributedTasks { get; init; } = new List<WorklistTask>();

    public string GetText()
    {
        return Role switch
        {
            Role.Admin => ExternalId,
            Role.Department => BusinessTexts.Worklist_DepartmentRole,
            Role.Office => BusinessTexts.Worklist_OfficeRole,
            Role.Secretariat => BusinessTexts.Worklist_SecretariatRole,
            Role.Observer => null,
            _ => throw new ArgumentOutOfRangeException()
        } ?? ExternalId;
    }

    public string GetDescription()
    {
        return Role switch
        {
            Role.Admin => ExternalId,
            Role.Department => Department?.Offices.FirstOrDefault(x => x.IsGeneralSecretariat)?.GetText() ?? Department?.GetText(),
            Role.Office => Office?.GetText(),
            Role.Secretariat => Committee?.GetDescription(),
            Role.Observer => null,
            _ => throw new ArgumentOutOfRangeException()
        } ?? ExternalId;
    }
}
