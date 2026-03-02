using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class GeneralMeasureService : IGeneralMeasureService
{
    private readonly IGeneralMeasureRepository _generalMeasureRepository;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly ILogger<GeneralMeasureService> _logger;

    public GeneralMeasureService(
        IGeneralMeasureRepository generalMeasureRepository,
        IWorklistTaskRepository worklistTaskRepository,
        IAuthorizationService authorizationService,
        IMasterDataRepository masterDataRepository,
        ILogger<GeneralMeasureService> logger)
    {
        _generalMeasureRepository = generalMeasureRepository;
        _worklistTaskRepository = worklistTaskRepository;
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

        var currentEiamAssignment = await _authorizationService.GetCurrentEiamAssignment();
        var generalMeasureTasks = await _worklistTaskRepository.GetByDepartmentIdsAndWorklistTaskTypeIds(departmentIds, [WorklistTaskType.GeneralMeasureCheck, WorklistTaskType.GeneralMeasureValidate]);

        var result = departmentIds.Select(departmentId =>
        {
            var departmentTask = generalMeasureTasks.SingleOrDefault(x => x.DepartmentId == departmentId && x.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureCheck);
            var adminTask = generalMeasureTasks.SingleOrDefault(x => x.DepartmentId == departmentId && x.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureValidate);
            var isDepartmentTaskActive = departmentTask?.WorklistTaskStateId == WorklistTaskState.Active;
            var isAdminTaskActive = adminTask?.WorklistTaskStateId == WorklistTaskState.Active;
            var canForwardToAdmin = isDepartmentTaskActive && departmentTask?.AssignedToId == currentEiamAssignment.Id;
            var canForwardToDepartment = isAdminTaskActive && currentEiamAssignment.Role == Role.Admin;
            var canValidate = isAdminTaskActive && currentEiamAssignment.Role == Role.Admin;

            return new GeneralMeasureDto
            {
                DepartmentId = departmentId,
                Department = allDepartments.Single(x => x.Id == departmentId).GetText(),
                JustificationGenders = genderMeasuresByDepartment.TryGetValue(departmentId, out var genders) ? genders : null,
                JustificationLanguages = languageMeasuresByDepartment.TryGetValue(departmentId, out var languages) ? languages : null,
                IsDepartmentTaskActive = isDepartmentTaskActive,
                IsAdminTaskActive = isAdminTaskActive,
                CanForwardToAdmin = canForwardToAdmin,
                CanForwardToDepartment = canForwardToDepartment,
                CanValidate = canValidate
            };
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

    public async Task Forward(Guid departmentId, string message, bool forwardToAdmin)
    {
        _logger.LogInformation("Forward general measure for department {DepartmentId} to {ForwardTarget}", departmentId, forwardToAdmin ? "admin" : "department");

        var tasks = await _worklistTaskRepository.GetByDepartmentIdAndWorklistTaskTypeIdsForUpdate(departmentId, [WorklistTaskType.GeneralMeasureCheck, WorklistTaskType.GeneralMeasureValidate]);
        var departmentTask = tasks.Single(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureCheck);
        var adminTask = tasks.Single(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralMeasureValidate);
        var userName = _authorizationService.GetCurrentUserName();
        var now = DateTime.UtcNow;

        if (forwardToAdmin)
        {
            departmentTask.WorklistTaskStateId = WorklistTaskState.Completed;
            departmentTask.Modified = now;
            departmentTask.ModifiedBy = userName;

            adminTask.WorklistTaskStateId = WorklistTaskState.Active;
            adminTask.Modified = now;
            adminTask.ModifiedBy = userName;
            adminTask.Description = message;
        }
        else
        {
            departmentTask.WorklistTaskStateId = WorklistTaskState.Active;
            departmentTask.Modified = now;
            departmentTask.ModifiedBy = userName;
            departmentTask.Description = message;

            adminTask.WorklistTaskStateId = WorklistTaskState.Inactive;
            adminTask.Modified = now;
            adminTask.ModifiedBy = userName;
        }

        await _worklistTaskRepository.CommitChanges();
    }

    public async Task Validate(Guid departmentId)
    {
        _logger.LogInformation("Validate general measure for department {DepartmentId}", departmentId);

        var adminTask = (await _worklistTaskRepository.GetByDepartmentIdAndWorklistTaskTypeIdsForUpdate(departmentId, [WorklistTaskType.GeneralMeasureValidate])).Single();
        adminTask.WorklistTaskStateId = WorklistTaskState.Completed;
        adminTask.Modified = DateTime.UtcNow;
        adminTask.ModifiedBy = _authorizationService.GetCurrentUserName();
        await _worklistTaskRepository.CommitChanges();
    }
}
