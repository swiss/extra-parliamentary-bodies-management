using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Services;

public class EntityAuditLogService : IEntityAuditLogService
{
    private readonly IEntityAuditLogRepository _entityAuditLogRepository;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IOccupationRepository _occupationRepository;
    private readonly ICantonRepository _cantonRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IPersonService _personService;

    public EntityAuditLogService(
        IEntityAuditLogRepository entityAuditLogRepository,
        IMasterDataRepository masterDataRepository,
        IOccupationRepository occupationRepository,
        ICantonRepository cantonRepository,
        IPersonRepository personRepository,
        IPersonService personService
    )
    {
        _entityAuditLogRepository = entityAuditLogRepository;
        _masterDataRepository = masterDataRepository;
        _occupationRepository = occupationRepository;
        _cantonRepository = cantonRepository;
        _personRepository = personRepository;
        _personService = personService;
    }

    public async Task<PagedResult<EntityAuditLogDto>> GetAuditLogsForEntity(string entityId, string entityType, IEnumerable<string> relatedEntityIds, PagingParametersDto paging, string? sort, SortDirection? sortDirection)
    {
        var pagingParameters = PagingMapper.ToPagingParameters(paging);
        var entityAuditLogs = await _entityAuditLogRepository.GetEntityAuditLogs(entityId, relatedEntityIds.Where(x => !string.IsNullOrWhiteSpace(x)), pagingParameters, sort, sortDirection);

        var entityAuditLogDtos = entityAuditLogs.Items
            .Select(EntityAuditLogMapper.MapToDto)
            .ToArray();

        foreach (var entityAuditLogDto in entityAuditLogDtos)
        {
            await ResolveForeignKeys(entityAuditLogDto);
        }

        if (entityType.Equals(nameof(Person), StringComparison.InvariantCultureIgnoreCase))
        {
            await MaskAddressData(entityId, entityAuditLogDtos);
        }

        return new PagedResult<EntityAuditLogDto>
        {
            Index = entityAuditLogs.Index,
            Total = entityAuditLogs.Total,
            Items = entityAuditLogDtos
        };
    }

    private async Task ResolveForeignKeys(EntityAuditLogDto dto)
    {
        foreach (var dataBefore in dto.AuditDataBefore.Where(x => !string.IsNullOrWhiteSpace(x.Data)))
        {
            await ResolveForeignKey(dataBefore);
        }

        foreach (var dataAfter in dto.AuditDataAfter.Where(x => !string.IsNullOrWhiteSpace(x.Data)))
        {
            await ResolveForeignKey(dataAfter);
        }
    }

    private async Task ResolveForeignKey(EntityAuditLogDataDto entityAuditLogData)
    {
        if (!Guid.TryParse(entityAuditLogData.Data!, out var id))
        {
            return;
        }

        entityAuditLogData.Data = entityAuditLogData.ColumnName switch
        {
            "committee_level_id" => await GetMasterDataText<CommitteeLevel>(id),
            "committee_type_id" => await GetMasterDataText<CommitteeType>(id),
            "department_id" => await GetMasterDataText<Department>(id),
            "legal_form_id" => await GetMasterDataText<LegalForm>(id),
            "office_id" => await GetMasterDataText<Office>(id),
            "term_of_office_date_id" => await GetMasterDataText<TermOfOfficeDate>(id),
            "term_of_office_id" => await GetMasterDataText<TermOfOffice>(id),
            "appointment_decision_type_id" => await GetMasterDataText<AppointmentDecisionType>(id),
            "appointment_decision_link_type_id" => await GetMasterDataText<AppointmentDecisionLinkType>(id),
            "election_office_id" => await GetMasterDataText<ElectionOffice>(id),
            "election_type_id" => await GetMasterDataText<ElectionType>(id),
            "function_id" => await GetMasterDataText<Function>(id),
            "council_id" => await GetMasterDataText<Council>(id),
            "gender_id" => await GetMasterDataText<Gender>(id),
            "salutation_id" => await GetMasterDataText<Salutation>(id),
            "correspondence_language_id" or "language_id" => await GetMasterDataText<Language>(id),
            "interest_function_id" => await GetMasterDataText<InterestFunction>(id),
            "membership_addition_id" => await GetMasterDataText<MembershipAddition>(id),
            "occupations_id" => (await _occupationRepository.GetById(id))?.GetText() ?? id.ToString(),
            "canton_id" => (await _cantonRepository.GetById(id))?.GetText() ?? id.ToString(),
            _ => id.ToString()
        };
    }

    private async Task<string> GetMasterDataText<T>(Guid id) where T : MasterDataBase
    {
        var masterData = await _masterDataRepository.GetById<T>(id);
        return masterData?.GetText() ?? id.ToString();
    }

    private async Task MaskAddressData(string entityId, EntityAuditLogDto[] entityAuditLogDtos)
    {
        var person = await _personRepository.GetById(new Guid(entityId));
        if (_personService.ShouldMaskAddress(person))
        {
            var maskedAddress = new EntityAuditLogDataDto { ColumnName = "address", Data = BusinessTexts.Person_MaskedAddress };
            foreach (var addressLog in entityAuditLogDtos.Where(x => x.EntityType == nameof(Address)))
            {
                addressLog.AuditDataAfter = [maskedAddress];
                addressLog.AuditDataBefore = [maskedAddress];
            }
        }
    }
}
