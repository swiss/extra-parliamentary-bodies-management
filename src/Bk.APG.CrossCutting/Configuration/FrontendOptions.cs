using System.ComponentModel.DataAnnotations;

namespace Bk.APG.CrossCutting.Configuration;

public class FrontendOptions
{
    public const string SectionKey = "Frontend";

    public string? Banner { get; init; }

    [Required]
    public required bool IsProduction { get; init; }

    [Required]
    public required string FroalaKey { get; init; }

    [Required]
    public required string KeyCloakUrl { get; init; }

    [Required]
    public required string KeyCloakClientId { get; init; }

    [Required]
    public required string KeyCloakRedirectUrl { get; init; }

    [Required]
    public required string KeyCloakRoleAllow { get; init; }

    [Required]
    public required string KeyCloakRoleAdmin { get; init; }

    [Required]
    public required string KeyCloakRoleDepartment { get; init; }

    [Required]
    public required string KeyCloakRoleOffice { get; init; }

    [Required]
    public required string KeyCloakRoleSecretariat { get; init; }

    [Required]
    public required string KeyCloakRoleObserver { get; init; }

    [Required]
    public required string EiamMyAccountUrl { get; init; }

    [Required]
    public required EntityIdOptions EntityIds { get; init; }

    [Required]
    public required OpenDataStackFrontendOptions OpenDataStack { get; init; }
}
