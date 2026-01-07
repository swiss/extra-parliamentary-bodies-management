namespace Bk.APG.Business.Models;

public class Department : MasterDataBase
{
    public const string BkUri = "https://ld.admin.ch/FCh";
    public const string EdaUri = "https://ld.admin.ch/department/I";
    public const string EdiUri = "https://ld.admin.ch/department/II";
    public const string EjpdUri = "https://ld.admin.ch/department/III";
    public const string VbsUri = "https://ld.admin.ch/department/IV";
    public const string EfdUri = "https://ld.admin.ch/department/V";
    public const string WbfUri = "https://ld.admin.ch/department/VI";
    public const string UvekUri = "https://ld.admin.ch/department/VII";

    public static readonly Guid BkGuid = Guid.Parse("a7a9fab3-981b-49fe-81e9-4941464f4764");
    public static readonly Guid EdaGuid = Guid.Parse("93e9c092-5e1f-4f70-9351-00c8ea433b75");
    public static readonly Guid EdiGuid = Guid.Parse("3d10ff41-7b7b-4768-b191-24848c143762");
    public static readonly Guid EjpdGuid = Guid.Parse("db7b93e6-7ccd-4bd3-809e-4d756b6421d8");
    public static readonly Guid VbsGuid = Guid.Parse("01f341c2-e642-4605-aea8-a3aa605831a8");
    public static readonly Guid EfdGuid = Guid.Parse("c4e9fac9-da8d-4a65-a22b-e2c32bc4dd2c");
    public static readonly Guid WbfGuid = Guid.Parse("d1948cf7-a3c0-4299-809e-f40970bb82d0");
    public static readonly Guid UvekGuid = Guid.Parse("6f0c278c-0e59-4056-b59d-ae66cdef261a");

    public EiamAssignment? EiamAssignment { get; set; }
    public required Guid EiamAssignmentId { get; set; }

    public required bool IsBigDepartment { get; set; }

    public GeneralGenderMeasure? GeneralGenderMeasure { get; set; }
    public Guid? GeneralGenderMeasureId { get; set; }
    public GeneralLanguageMeasure? GeneralLanguageMeasure { get; set; }
    public Guid? GeneralLanguageMeasureId { get; set; }

    public ICollection<Office> Offices { get; set; } = new List<Office>();
    public ICollection<Committee> Committees { get; set; } = new List<Committee>();
    public ICollection<GeneralElectionCommittee> GeneralElectionCommittees { get; set; } = new List<GeneralElectionCommittee>();
}
