using System.Globalization;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class CommitteeTypeService : ICommitteeTypeService
{
    private readonly ICommitteeTypeRepository _committeeTypeRepository;
    private readonly ICultureService _cultureService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<CommitteeTypeService> _logger;

    public CommitteeTypeService(
        ICommitteeTypeRepository committeeTypeRepository,
        ICultureService cultureService,
        IAuthorizationService authorizationService,
        ILogger<CommitteeTypeService> logger)
    {
        _committeeTypeRepository = committeeTypeRepository;
        _cultureService = cultureService;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<List<CommitteeTypeListDto>> GetCommitteeTypeList()
    {
        var committeeTypes = await _committeeTypeRepository.GetList();

        var mappedCommitteeTypes = committeeTypes.Select(committeeType => CommitteeTypeMapper.ToCommitteeTypeListDto(committeeType, _cultureService.GetCurrentUiCulture())).ToList();

        return mappedCommitteeTypes.OrderBy(ct => ct.Text).ToList();
    }

    public async Task<CommitteeTypeUpdateDto> GetCommitteeTypeForUpdate(Guid id)
    {
        var committeeType = await _committeeTypeRepository.GetByIdForUpdate(id);
        var dto = CommitteeTypeMapper.ToCommitteeTypeUpdateDto(committeeType, new CultureInfo("de"));
        return dto;
    }

    public async Task<CommitteeTypeUpdateDto> UpdateCommitteeType(Guid id, CommitteeTypeUpdateDto updateDto)
    {
        _logger.LogInformation("Update committee type {CommitteeTypeId}", id);

        var existingCommitteeType = await _committeeTypeRepository.GetByIdForUpdate(id, updateDto.RowVersion);

        if (!_authorizationService.IsAdmin)
        {
            _logger.LogError("User is not allowed to edit committee type {CommitteeTypeId}", id);

            throw new AuthorizationException($"User is not allowed to edit committee type with id: {id}");
        }

        if ((updateDto.GermanMinimalThreshold > 0 || updateDto.FrenchMinimalThreshold > 0 || updateDto.ItalianMinimalThreshold > 0 || updateDto.RomanshMinimalThreshold > 0) &&
            (updateDto.GermanThresholdPercentage > 0 || updateDto.FrenchThresholdPercentage > 0 || updateDto.ItalianThresholdPercentage > 0 || updateDto.RomanshThresholdPercentage > 0))
        {
            _logger.LogError("It is not allowed to have minimal and percentage values on the same committee type! Update failed for {CommitteeTypeId}", id);

            throw new BusinessValidationException($"It is not allowed to have Minimal and Percentage values on the same committee type! Update failed, id: {id}");
        }

        if (updateDto.GermanThresholdPercentage + updateDto.FrenchThresholdPercentage + updateDto.ItalianThresholdPercentage + updateDto.RomanshThresholdPercentage > 100)
        {
            _logger.LogError("Update of committee type {CommitteeTypeId} failed, because language percentages were bigger than 100%.", id);

            throw new BusinessValidationException($"Update of committee Typ {id} failed, because language percentages were bigger than 100%.");
        }

        if (updateDto.FemaleThreshold + updateDto.MaleThreshold > 100)
        {
            _logger.LogError("Update of committee type {CommitteeTypeId} failed, because gender percentages were bigger than 100%.", id);

            throw new BusinessValidationException($"Update of committee Typ {id} failed, because gender percentages were bigger than 100%.");
        }

        // here, only changes to the percentages or quota values are allowed, all text fields are readonly
        existingCommitteeType.FemaleThreshold = updateDto.FemaleThreshold;
        existingCommitteeType.MaleThreshold = updateDto.MaleThreshold;
        existingCommitteeType.GermanMinimalThreshold = updateDto.GermanMinimalThreshold;
        existingCommitteeType.FrenchMinimalThreshold = updateDto.FrenchMinimalThreshold;
        existingCommitteeType.ItalianMinimalThreshold = updateDto.ItalianMinimalThreshold;
        existingCommitteeType.RomanshMinimalThreshold = updateDto.RomanshMinimalThreshold;
        existingCommitteeType.GermanThresholdPercentage = updateDto.GermanThresholdPercentage;
        existingCommitteeType.FrenchThresholdPercentage = updateDto.FrenchThresholdPercentage;
        existingCommitteeType.ItalianThresholdPercentage = updateDto.ItalianThresholdPercentage;
        existingCommitteeType.RomanshThresholdPercentage = updateDto.RomanshThresholdPercentage;
        existingCommitteeType.Modified = DateTime.UtcNow;
        existingCommitteeType.ModifiedBy = _authorizationService.GetCurrentUserName();

        await _committeeTypeRepository.CommitChanges();

        _logger.LogInformation("Updated committee type {CommitteeTypeId}", id);
        return await GetCommitteeTypeForUpdate(id);
    }
}
