using System.Security.Claims;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Configuration;
using Bk.APG.CrossCutting.Exception;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bk.APG.Business.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly AuthorizationOptions _authorizationOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEiamAssignmentRepository _eiamAssignmentRepository;
    private readonly ILogger<AuthorizationService> _logger;

    private IEnumerable<Committee>? _assignedCommittees;

    public AuthorizationService(
        IHttpContextAccessor httpContextAccessor,
        IEiamAssignmentRepository eiamAssignmentRepository,
        IOptions<AuthorizationOptions> authorizationOptions,
        ILogger<AuthorizationService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _eiamAssignmentRepository = eiamAssignmentRepository;
        _authorizationOptions = authorizationOptions.Value;
        _logger = logger;
    }

    public bool IsAdmin
    {
        get
        {
            var (hasAllowRole, hasAdminRole, hasSecretariatRole, hasDepartmentRole, hasOfficeRole, hasObserverRole) = HasRoles();
            return hasAllowRole && hasAdminRole && !hasSecretariatRole && !hasDepartmentRole && !hasOfficeRole && !hasObserverRole;
        }
    }

    public bool IsSecretariat
    {
        get
        {
            var (hasAllowRole, hasAdminRole, hasSecretariatRole, hasDepartmentRole, hasOfficeRole, hasObserverRole) = HasRoles();
            return hasAllowRole && !hasAdminRole && hasSecretariatRole && !hasDepartmentRole && !hasOfficeRole && !hasObserverRole;
        }
    }

    public bool IsDepartment
    {
        get
        {
            var (hasAllowRole, hasAdminRole, hasSecretariatRole, hasDepartmentRole, hasOfficeRole, hasObserverRole) = HasRoles();
            return hasAllowRole && !hasAdminRole && !hasSecretariatRole && hasDepartmentRole && !hasOfficeRole && !hasObserverRole;
        }
    }

    public bool IsOffice
    {
        get
        {
            var (hasAllowRole, hasAdminRole, hasSecretariatRole, hasDepartmentRole, hasOfficeRole, hasObserverRole) = HasRoles();
            return hasAllowRole && !hasAdminRole && !hasSecretariatRole && !hasDepartmentRole && hasOfficeRole && !hasObserverRole;
        }
    }

    public bool IsObserver
    {
        get
        {
            var (hasAllowRole, hasAdminRole, hasSecretariatRole, hasDepartmentRole, hasOfficeRole, hasObserverRole) = HasRoles();
            return hasAllowRole && !hasAdminRole && !hasSecretariatRole && !hasDepartmentRole && !hasOfficeRole && hasObserverRole;
        }
    }

    private string GetCurrentExternalId()
    {
        return IsAdmin ? "Admin" : ExtractClaim(CustomClaimTypes.ProfiledUnitExtId).Split(@"\")[^1];
    }

    public string GetCurrentUserName()
    {
        var email = ExtractClaim(ClaimTypes.Email);

        return string.IsNullOrWhiteSpace(email)
            ? throw new InvalidOperationException("Claims for user name where not found")
            : email;
    }

    public async Task<Department?> GetDepartment()
    {
        _assignedCommittees ??= await LoadCommittees();
        return _assignedCommittees.FirstOrDefault()?.Department;
    }

    public async Task<Office?> GetOffice()
    {
        var eiamAssignment = await GetCurrentEiamAssignment();

        return eiamAssignment.Office;
    }

    public async Task<bool> IsCommitteeAssigned(Guid committeeId)
    {
        _assignedCommittees ??= await LoadCommittees();
        return _assignedCommittees.Any(y => y.Id == committeeId);
    }

    public async Task<IEnumerable<Committee>> LoadCommittees()
    {
        if (IsAdmin || IsObserver)
        {
            return new List<Committee>();
        }

        var eiamAssignment = await GetCurrentEiamAssignment();

        return eiamAssignment.Role switch
        {
            Role.Department => eiamAssignment.Department!.Committees,
            Role.Office => eiamAssignment.Office!.Committees,
            Role.Secretariat => [eiamAssignment.Committee!],
            _ => new List<Committee>()
        };
    }

    public async Task<bool> HasAccessToCommittee(Committee committee)
    {
        return IsAdmin ||
            (committee.IsActive && ((IsDepartment && committee.DepartmentId == (await GetDepartment())?.Id) ||
                ((IsOffice || IsSecretariat) && await IsCommitteeAssigned(committee.Id))));
    }

    public async Task<EiamAssignment> GetCurrentEiamAssignment()
    {
        var externalId = GetCurrentExternalId();
        var eiamAssignment = await _eiamAssignmentRepository.GetByExternalId(externalId);

        var currentRole = GetCurrentRole();
        if (currentRole != null && eiamAssignment.Role != currentRole)
        {
            _logger.LogError(
                "Role mismatch: user has role '{UserRole}' but assignment has role '{AssignmentRole}' for external ID '{ExternalId}'",
                currentRole, eiamAssignment.Role, externalId);
            throw new AuthorizationException(
                $"Role mismatch: user has role '{currentRole}' but assignment has role '{eiamAssignment.Role}' for external ID '{externalId}'");
        }

        return eiamAssignment;
    }

    private Role? GetCurrentRole()
    {
        if (IsAdmin)
        {
            return Role.Admin;
        }

        if (IsDepartment)
        {
            return Role.Department;
        }

        if (IsOffice)
        {
            return Role.Office;
        }

        if (IsSecretariat)
        {
            return Role.Secretariat;
        }

        if (IsObserver)
        {
            return Role.Observer;
        }

        return null;
    }

    private string ExtractClaim(string claimType)
    {
        var claim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == claimType);
        return claim?.Value ?? throw new InvalidOperationException($"Claim with type '{claimType}' not found!");
    }

    private (bool hasAllowRole, bool hasAdminRole, bool hasSecretariatRole, bool hasDepartmentRole, bool hasOfficeRole, bool hasObserverRole) HasRoles()
    {
        var hasAllowRole = _httpContextAccessor.HttpContext?.User.IsInRole(_authorizationOptions.Allow) ?? false;
        var hasAdminRole = _httpContextAccessor.HttpContext?.User.IsInRole(_authorizationOptions.Admin) ?? false;
        var hasSecretariatRole = _httpContextAccessor.HttpContext?.User.IsInRole(_authorizationOptions.Secretariat) ?? false;
        var hasDepartmentRole = _httpContextAccessor.HttpContext?.User.IsInRole(_authorizationOptions.Department) ?? false;
        var hasOfficeRole = _httpContextAccessor.HttpContext?.User.IsInRole(_authorizationOptions.Office) ?? false;
        var hasObserverRole = _httpContextAccessor.HttpContext?.User.IsInRole(_authorizationOptions.Observer) ?? false;

        return (hasAllowRole, hasAdminRole, hasSecretariatRole, hasDepartmentRole, hasOfficeRole, hasObserverRole);
    }
}
