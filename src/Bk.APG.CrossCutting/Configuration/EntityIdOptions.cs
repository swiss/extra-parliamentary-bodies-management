using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class EntityIdOptions
{
    [Required]
    public required AppointmentDecisionTypeOptions AppointmentDecisionType { get; init; }

    [Required]
    public required AppointmentDecisionLinkTypeOptions AppointmentDecisionLinkType { get; init; }

    [Required]
    public required LanguageOptions Language { get; init; }

    [Required]
    public required GenderOptions Gender { get; init; }

    [Required]
    public required SalutationOptions Salutation { get; init; }

    [Required]
    public required CantonOptions Canton { get; init; }

    [Required]
    public required ElectionOfficeOptions ElectionOffice { get; init; }

    [Required]
    public required CommitteeTypeOptions CommitteeType { get; init; }

    [Required]
    public required CommitteeLevelOptions CommitteeLevel { get; init; }

    [Required]
    public required ElectionTypeOptions ElectionType { get; init; }

    [Required]
    public required WorklistTaskTypeOptions WorklistTaskType { get; init; }

    [Required]
    public required TermOfOfficeOptions TermOfOffice { get; init; }

    [Required]
    public required ContactPointIdConfig ContactPoint { get; init; }

    [Required]
    public required CountryOptions Country { get; init; }
}

public class ContactPointIdConfig
{
    [Required]
    public required string SecretariatId { get; init; }

    [Required]
    public required string DpoId { get; init; }
}

public class TermOfOfficeOptions
{
    [Required]
    public required string Period4YearsInGeneralElectionId { get; init; }
}

public class WorklistTaskTypeOptions
{
    [Required]
    public required string GeneralElectionStartId { get; init; }

    [Required]
    public required string GeneralElectionEndId { get; init; }
}

public class LanguageOptions
{
    [Required]
    public required string GermanLanguageId { get; init; }

    [Required]
    public required string FrenchLanguageId { get; init; }

    [Required]
    public required string ItalianLanguageId { get; init; }

    [Required]
    public required string RomanshLanguageId { get; init; }
}

public class AppointmentDecisionTypeOptions
{
    [Required]
    public required string DecisionFederalCouncilId { get; init; }

    [Required]
    public required string InstitutionId { get; init; }

    [Required]
    public required string ReportId { get; init; }

    [Required]
    public required string OtherId { get; init; }

    [Required]
    public required string RegulationsId { get; init; }
}

public class AppointmentDecisionLinkTypeOptions
{
    [Required]
    public required string ExeLinkTypeId { get; init; }

    [Required]
    public required string StandardLinkTypeId { get; init; }
}

public class GenderOptions
{
    [Required]
    public required string MaleId { get; init; }

    [Required]
    public required string FemaleId { get; init; }
}

public class SalutationOptions
{
    [Required]
    public required string MaleId { get; init; }

    [Required]
    public required string FemaleId { get; init; }

    [Required]
    public required string ManualId { get; init; }
}

public class CantonOptions
{
    [Required]
    public required string AbroadId { get; init; }

}

public class ElectionOfficeOptions
{
    [Required]
    public required string FederalGovernmentId { get; init; }

    [Required]
    public required string OtherId { get; init; }
}

public class CommitteeTypeOptions
{
    [Required]
    public required string AuthorityId { get; init; }

    [Required]
    public required string AdministrationId { get; init; }

    [Required]
    public required string ManagementId { get; init; }

    [Required]
    public required string FederalAgenciesId { get; init; }

    [Required]
    public required string FederalAgenciesCrossBorderId { get; init; }
}

public class CommitteeLevelOptions
{
    [Required]
    public required string FederalCouncilId { get; init; }
}

public class ElectionTypeOptions
{
    [Required]
    public required string NewElectionId { get; init; }

    [Required]
    public required string ReElectionId { get; init; }

    [Required]
    public required string MaximumDutyRetirementId { get; init; }

    [Required]
    public required string DeceasedId { get; init; }
}

public class CountryOptions
{
    [Required]
    public required string SwitzerlandId { get; init; }
}
