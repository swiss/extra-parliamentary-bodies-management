using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class GeneralMeasureService : IGeneralMeasureService
{
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly ILogger<GeneralMeasureService> _logger;

    public GeneralMeasureService(IGeneralMeasureRepository generalMeasureRepository, IAuthorizationService authorizationService, IMasterDataRepository masterDataRepository, ILogger<GeneralMeasureService> logger)
    {
        _generalMeasureRepository = generalMeasureRepository;
        _authorizationService = authorizationService;
        _masterDataRepository = masterDataRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<GeneralMeasureDto>> GetGeneralMeasures()
    {
        var genderMeasures = await _generalMeasureRepository.GetGeneralGenderMeasures();
        var languageMeasures = await _generalMeasureRepository.GetGeneralLanguageMeasures();

        var genderMeasuresByDepartment = genderMeasures.ToDictionary(x => x.DepartmentId, x => x.Description);
        var languageMeasuresByDepartment = languageMeasures.ToDictionary(x => x.DepartmentId, x => x.Description);

        var allDepartments = (await _masterDataRepository.GetDepartments()).ToArray();

        var departmentIds = new List<Guid>();
        if (_authorizationService.IsDepartment)
        {
            var department = await _authorizationService.GetDepartment();
            departmentIds.Add(department?.Id ?? Guid.Empty);
        }
        else if (_authorizationService.IsAdmin)
        {
            departmentIds.AddRange(allDepartments.Select(x => x.Id));
        }

        var result = departmentIds.Select(departmentId => new GeneralMeasureDto
        {
            DepartmentId = departmentId,
            Department = allDepartments.Single(x => x.Id == departmentId).GetText(),
            JustificationGenders = genderMeasuresByDepartment.TryGetValue(departmentId, out var genders) ? genders : null,
            JustificationLanguages = languageMeasuresByDepartment.TryGetValue(departmentId, out var languages) ? languages : null
        }).ToList();

        return result;
    }

    public async Task AddOrUpdateGeneralMeasure(GeneralMeasureUpdateDto generalMeasureUpdate)
    {
        var userName = _authorizationService.GetCurrentUserName();

        var genderMeasure = await _generalMeasureRepository.GetGeneralGenderMeasureForUpdate(generalMeasureUpdate.DepartmentId);
        if (genderMeasure is null)
        {
            _logger.LogInformation("Create new gender measure for department {DepartmentId}", generalMeasureUpdate.DepartmentId);

            genderMeasure = new GeneralGenderMeasure
            {
                DepartmentId = generalMeasureUpdate.DepartmentId,
                Description = generalMeasureUpdate.JustificationGenders ?? string.Empty,
                Created = DateTime.UtcNow,
                CreatedBy = userName,
                Modified = DateTime.UtcNow,
                ModifiedBy = userName
            };

            await _generalMeasureRepository.AddGeneralGenderMeasure(genderMeasure);
            _logger.LogInformation("Created gender measure {MeasureId}", genderMeasure.Id);
        }
        else
        {
            _logger.LogInformation("Update gender measure {MeasureId}", genderMeasure.Id);

            genderMeasure.Description = generalMeasureUpdate.JustificationGenders ?? string.Empty;
            genderMeasure.Modified = DateTime.UtcNow;
            genderMeasure.ModifiedBy = userName;

            await _generalMeasureRepository.CommitChanges();
            _logger.LogInformation("Updated gender measure {MeasureId}", genderMeasure.Id);
        }

        var languageMeasure = await _generalMeasureRepository.GetGeneralLanguageMeasureForUpdate(generalMeasureUpdate.DepartmentId);
        if (languageMeasure is null)
        {
            _logger.LogInformation("Create new language measure for department {DepartmentId}", generalMeasureUpdate.DepartmentId);

            languageMeasure = new GeneralLanguageMeasure
            {
                DepartmentId = generalMeasureUpdate.DepartmentId,
                Description = generalMeasureUpdate.JustificationLanguages ?? string.Empty,
                Created = DateTime.UtcNow,
                CreatedBy = userName,
                Modified = DateTime.UtcNow,
                ModifiedBy = userName
            };

            await _generalMeasureRepository.AddGeneralLanguageMeasure(languageMeasure);
            _logger.LogInformation("Created language measure {MeasureId}", genderMeasure.Id);
        }
        else
        {
            _logger.LogInformation("Update language measure {MeasureId}", genderMeasure.Id);

            languageMeasure.Description = generalMeasureUpdate.JustificationLanguages ?? string.Empty;
            languageMeasure.Modified = DateTime.UtcNow;
            languageMeasure.ModifiedBy = userName;

            await _generalMeasureRepository.CommitChanges();
            _logger.LogInformation("Updated language measure {MeasureId}", genderMeasure.Id);
        }
    }
}
