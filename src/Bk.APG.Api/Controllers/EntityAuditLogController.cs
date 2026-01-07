using System.ComponentModel.DataAnnotations;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Policies;
using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = APGPolicies.RequireAdminDepartmentOfficeOrSecretariatRole)]
public class EntityAuditLogController : ControllerBase
{
    private readonly IEntityAuditLogService _entityAuditLogService;

    public EntityAuditLogController(IEntityAuditLogService entityAuditLogService)
    {
        _entityAuditLogService = entityAuditLogService;
    }

    [HttpGet("{entityId}")]
    public async Task<ActionResult> GetAuditLogsForEntity(string entityId, [FromQuery] string entityType, [FromQuery] IEnumerable<string> relatedEntityIds, [FromQuery, Required] PagingParametersDto pagingParameters, [FromQuery] SortParametersDto sortParameters)
    {
        var auditLogs = await _entityAuditLogService.GetAuditLogsForEntity(entityId, entityType, relatedEntityIds, pagingParameters, sortParameters.Sort, sortParameters.Direction);
        return Ok(auditLogs);
    }
}
