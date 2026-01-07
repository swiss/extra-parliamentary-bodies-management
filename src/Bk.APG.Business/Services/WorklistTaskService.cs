using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class WorklistTaskService : IWorklistTaskService
{
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IEiamAssignmentRepository _eiamAssignmentRepository;
    private readonly IGeneralElectionCommitteeRepository _generalElectionCommitteeRepository;
    private readonly ILogger<WorklistTaskService> _logger;

    public WorklistTaskService(
        IWorklistTaskRepository worklistTaskRepository,
        IAuthorizationService authorizationService,
        ICommitteeRepository committeeRepository,
        IEiamAssignmentRepository eiamAssignmentRepository,
        IGeneralElectionCommitteeRepository generalElectionCommitteeRepository,
        ILogger<WorklistTaskService> logger)
    {
        _worklistTaskRepository = worklistTaskRepository;
        _authorizationService = authorizationService;
        _committeeRepository = committeeRepository;
        _eiamAssignmentRepository = eiamAssignmentRepository;
        _generalElectionCommitteeRepository = generalElectionCommitteeRepository;
        _logger = logger;
    }

    public async Task<PagedResultDto<WorklistTaskDto>> GetWorklistTasks(PagingParametersDto paging, WorklistFilterParametersDto? filterParametersDto, string? sort, SortDirection? sortDirection)
    {
        var filter = WorklistTaskMapper.ToWorklistTaskFilterParameters(filterParametersDto);
        var pagingParameters = PagingMapper.ToPagingParameters(paging);
        var worklistTasks = await _worklistTaskRepository.GetAll(pagingParameters, filter, sort, sortDirection);

        return new PagedResultDto<WorklistTaskDto>
        {
            Index = worklistTasks.Index,
            Total = worklistTasks.Total,
            Items = worklistTasks.Items.Select(WorklistTaskMapper.ToWorklistTaskDto)
        };
    }

    public async Task<WorklistTaskUpdateDto> GetWorklistTaskForUpdate(Guid id)
    {
        var worklistTask = await _worklistTaskRepository.GetByIdForUpdate(id);
        var dto = WorklistTaskMapper.ToWorklistTaskUpdateDto(worklistTask);
        var currentExternalId = _authorizationService.GetCurrentExternalId();
        dto.CanEdit = worklistTask.AssignedBy!.ExternalId == currentExternalId;
        dto.CanForward = worklistTask.GetCanBeForwarded(currentExternalId);
        dto.IsBigDepartment = worklistTask.AssignedTo!.Role == Role.Department && (worklistTask.AssignedTo.Department?.IsBigDepartment ?? false);

        return dto;
    }

    public async Task<WorklistTask> CreateWorklistTaskByAdmin(WorklistTaskCreateDto worklistTaskCreateDto)
    {
        _logger.LogInformation("Create worklist task by admin");

        var currentUserName = _authorizationService.GetCurrentUserName();
        if (worklistTaskCreateDto.DepartmentId is null)
        {
            var assignedTo = await _eiamAssignmentRepository.GetById(worklistTaskCreateDto.AssignedToId!.Value);
            worklistTaskCreateDto.DepartmentId = assignedTo.DepartmentId;
        }

        var worklistTask = await _worklistTaskRepository.Create(WorklistTaskMapper.ToWorklistTask(worklistTaskCreateDto, EiamAssignment.AdminId, currentUserName));
        _logger.LogInformation("Created task {WorklistTaskId} by admin", worklistTask.Id);

        return worklistTask;
    }

    public async Task RemoveAllGeneralElectionTasks(Guid termOfOfficeDateId)
    {
        _logger.LogInformation("Remove all general election tasks for term of office date {TermOfOfficeDateId}", termOfOfficeDateId);

        var parents = await _worklistTaskRepository.GetByTermOfOfficeDateId(termOfOfficeDateId);

        _worklistTaskRepository.DeleteRange(parents);
        _logger.LogInformation("Removed all general election tasks for term of office date {TermOfOfficeDateId}", termOfOfficeDateId);
    }

    public async Task<WorklistTaskUpdateDto> UpdateWorklistTask(Guid id, WorklistTaskUpdateDto updateDto)
    {
        _logger.LogInformation("Update worklist task {WorklistTaskId}", id);

        var currentUserName = _authorizationService.GetCurrentUserName();
        var worklistTask = await _worklistTaskRepository.GetByIdForUpdate(id);

        worklistTask.Description = updateDto.Description ?? string.Empty;
        worklistTask.DueDate = updateDto.DueDate;
        worklistTask.Modified = DateTime.UtcNow;
        worklistTask.ModifiedBy = currentUserName;

        await _worklistTaskRepository.Update(worklistTask);
        _logger.LogInformation("Updated worklist task {WorklistTaskId}", id);

        return updateDto;
    }

    public async Task ForwardWorklistTask(Guid id, WorklistTaskForwardDto forwardDto)
    {
        _logger.LogInformation("Forward worklist task {WorklistTaskId}", id);

        var worklistTask = await _worklistTaskRepository.GetByIdForForward(id);
        var currentExternalId = _authorizationService.GetCurrentExternalId();
        if (!worklistTask.GetCanBeForwarded(currentExternalId))
        {
            throw new NotSupportedException($"Worklist task {id} can not be forwarded by external id {currentExternalId}");
        }

        if (worklistTask.WorklistTaskTypeId == WorklistTaskType.GeneralElectionDispatch)
        {
            await ForwardGeneralElectionDispatch(forwardDto, worklistTask);
            _logger.LogInformation("Forwarded worklist task {WorklistTaskId}", id);
        }
        else
        {
            throw new NotSupportedException($"Worklist task type {worklistTask.WorklistTaskTypeId} not supported for forward.");
        }
    }

    private async Task ForwardGeneralElectionDispatch(WorklistTaskForwardDto forwardDto, WorklistTask worklistTask)
    {
        _logger.LogDebug("Forwarding general election dispatch task {WorklistTaskId}", worklistTask.Id);
        var currentUserName = _authorizationService.GetCurrentUserName();
        var isDepartment = worklistTask.AssignedTo!.Role == Role.Department;
        if (isDepartment && worklistTask.Department!.IsBigDepartment)
        {
            await ForwardGeneralElectionDispatchTaskToOffice(forwardDto, worklistTask, currentUserName);
        }
        else
        {
            await ForwardGeneralElectionDispatchToSecretariat(forwardDto, worklistTask, isDepartment, currentUserName);
        }
    }

    private async Task ForwardGeneralElectionDispatchTaskToOffice(WorklistTaskForwardDto forwardDto, WorklistTask worklistTask, string currentUserName)
    {
        var currentEiamAssignment = worklistTask.AssignedTo!;
        if (currentEiamAssignment.Role != Role.Department)
        {
            throw new NotSupportedException("Can only forward to office for department");
        }

        var newTasks = currentEiamAssignment.Children.Where(x => x.Role == Role.Office)
            .Select(officeEiamAssignment => new WorklistTask
            {
                AssignedToId = officeEiamAssignment.Id,
                AssignedById = currentEiamAssignment.Id,
                DueDate = forwardDto.CandidateListDueDate,
                Description = forwardDto.CandidateListDescription,
                WorklistTaskTypeId = WorklistTaskType.GeneralElectionDispatch,
                WorklistTaskStateId = WorklistTaskState.Active,
                ParentTaskId = worklistTask.Id,
                DepartmentId = worklistTask.DepartmentId,
                OfficeId = officeEiamAssignment.OfficeId,
                Created = DateTime.UtcNow,
                CreatedBy = currentUserName,
                Modified = DateTime.UtcNow,
                ModifiedBy = currentUserName
            }).ToList();

        await _worklistTaskRepository.CreateRange(newTasks);
        _logger.LogInformation("Created {Count} tasks for office {OfficeId}", newTasks.Count, worklistTask.OfficeId);

        worklistTask.WorklistTaskStateId = WorklistTaskState.Completed;
        worklistTask.Modified = DateTime.UtcNow;
        worklistTask.ModifiedBy = currentUserName;
        await _worklistTaskRepository.Update(worklistTask);

        await SetOfficeReadyForProposalDueDates(forwardDto.CommitteeDueDate, currentEiamAssignment.DepartmentId!.Value);
    }

    private async Task SetOfficeReadyForProposalDueDates(DateOnly officeReadyForProposalDueDate, Guid departmentId)
    {
        var generalElectionCommittees = await _generalElectionCommitteeRepository.GetByDepartmentId(departmentId);

        foreach (var generalElectionCommittee in generalElectionCommittees)
        {
            generalElectionCommittee.OfficeReadyForProposalDueDate = officeReadyForProposalDueDate;
        }

        await _generalElectionCommitteeRepository.CommitChanges();
    }

    private async Task ForwardGeneralElectionDispatchToSecretariat(WorklistTaskForwardDto forwardDto,
        WorklistTask worklistTask, bool isDepartment, string currentUserName)
    {
        var committees = (isDepartment
            ? await _committeeRepository.GetForGeneralElectionByDepartmentId(worklistTask.DepartmentId!.Value)
            : await _committeeRepository.GetForGeneralElectionByOfficeId(worklistTask.OfficeId!.Value)).Where(c => c.IsActive);
        var forwarderId = worklistTask.AssignedToId;

        List<WorklistTask> newTasks = [];
        foreach (var committee in committees)
        {
            if (isDepartment)
            {
                CreateWorklistTasksForCommitteeSmallDepartment(forwardDto, worklistTask, newTasks, committee, forwarderId, currentUserName);
            }
            else
            {
                CreateWorklistTasksForCommitteeBigDepartment(forwardDto, worklistTask, newTasks, committee, forwarderId, currentUserName);
            }
        }

        await _worklistTaskRepository.CreateRange(newTasks);
        _logger.LogInformation("Created {Count} tasks for secretariats in department {DepartmentId}", newTasks.Count, worklistTask.DepartmentId);

        worklistTask.WorklistTaskStateId = WorklistTaskState.Completed;
        worklistTask.Modified = DateTime.UtcNow;
        worklistTask.ModifiedBy = currentUserName;
        await _worklistTaskRepository.Update(worklistTask);

        await SetSecretariatReadyForProposalDueDates(forwardDto.CommitteeDueDate, isDepartment, worklistTask.DepartmentId, worklistTask.OfficeId);
    }

    private async Task SetSecretariatReadyForProposalDueDates(DateOnly secretariatReadyForProposalDueDate, bool isDepartment, Guid? departmentId, Guid? officeId)
    {
        var generalElectionCommittees = isDepartment
            ? await _generalElectionCommitteeRepository.GetByDepartmentId(departmentId!.Value)
            : await _generalElectionCommitteeRepository.GetByOfficeId(officeId!.Value);

        foreach (var generalElectionCommittee in generalElectionCommittees)
        {
            generalElectionCommittee.SecretariatReadyForProposalDueDate = secretariatReadyForProposalDueDate;
        }

        await _generalElectionCommitteeRepository.CommitChanges();
    }

    private void CreateWorklistTasksForCommitteeSmallDepartment(WorklistTaskForwardDto forwardDto,
        WorklistTask worklistTask, List<WorklistTask> newTasks, Committee committee, Guid forwarderId, string currentUserName)
    {
        _logger.LogDebug("Creating worklist tasks for small department committee {CommitteeId}", committee.Id);
        newTasks.Add(new WorklistTask
        {
            AssignedToId = committee.EiamAssignmentId!.Value,
            AssignedById = forwarderId,
            DueDate = forwardDto.CandidateListDueDate,
            Description = forwardDto.CandidateListDescription,
            WorklistTaskTypeId = WorklistTaskType.CandidateListCreate,
            WorklistTaskStateId = WorklistTaskState.Active,
            ParentTaskId = worklistTask.Id,
            GeneralElectionCommitteeId = committee.GeneralElectionCommittees.First().Id,
            DepartmentId = committee.DepartmentId,
            OfficeId = committee.OfficeId,
            CommitteeId = committee.Id,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        });
        newTasks.Add(new WorklistTask
        {
            AssignedToId = forwarderId,
            AssignedById = EiamAssignment.ApgId,
            DueDate = forwardDto.CandidateListDueDate.AddDays(7),
            Description = $"GEW Wahlvorschlag genehmigen für Gremium: {committee.DescriptionGerman}",
            WorklistTaskTypeId = WorklistTaskType.CandidateListApprove,
            WorklistTaskStateId = WorklistTaskState.Inactive,
            ParentTaskId = worklistTask.Id,
            GeneralElectionCommitteeId = committee.GeneralElectionCommittees.First().Id,
            DepartmentId = committee.DepartmentId,
            OfficeId = committee.OfficeId,
            CommitteeId = committee.Id,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        });
    }

    private void CreateWorklistTasksForCommitteeBigDepartment(WorklistTaskForwardDto forwardDto, WorklistTask worklistTask, List<WorklistTask> newTasks, Committee committee, Guid officeAssignmentId, string currentUserName)
    {
        _logger.LogDebug("Creating worklist tasks for big department committee {CommitteeId}", committee.Id);
        newTasks.Add(new WorklistTask
        {
            AssignedToId = committee.EiamAssignmentId!.Value,
            AssignedById = officeAssignmentId,
            DueDate = forwardDto.CandidateListDueDate,
            Description = forwardDto.CandidateListDescription,
            WorklistTaskTypeId = WorklistTaskType.CandidateListCreate,
            WorklistTaskStateId = WorklistTaskState.Active,
            ParentTaskId = worklistTask.Id,
            GeneralElectionCommitteeId = committee.GeneralElectionCommittees.First().Id,
            DepartmentId = committee.DepartmentId,
            OfficeId = committee.OfficeId,
            CommitteeId = committee.Id,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        });
        newTasks.Add(new WorklistTask
        {
            AssignedToId = officeAssignmentId,
            AssignedById = EiamAssignment.ApgId,
            DueDate = worklistTask.DueDate,
            Description = $"GEW Wahlvorschlag übermitteln für Gremium: {committee.DescriptionGerman}",
            WorklistTaskTypeId = WorklistTaskType.CandidateListForward,
            WorklistTaskStateId = WorklistTaskState.Inactive,
            ParentTaskId = worklistTask.Id,
            GeneralElectionCommitteeId = committee.GeneralElectionCommittees.First().Id,
            DepartmentId = committee.DepartmentId,
            OfficeId = committee.OfficeId,
            CommitteeId = committee.Id,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        });
        newTasks.Add(new WorklistTask
        {
            AssignedToId = worklistTask.AssignedBy!.Id,
            AssignedById = EiamAssignment.ApgId,
            DueDate = worklistTask.ParentTask!.DueDate.AddDays(7),
            Description = $"GEW Wahlvorschlag genehmigen für Gremium: {committee.DescriptionGerman}",
            WorklistTaskTypeId = WorklistTaskType.CandidateListApprove,
            WorklistTaskStateId = WorklistTaskState.Inactive,
            ParentTaskId = worklistTask.Id,
            GeneralElectionCommitteeId = committee.GeneralElectionCommittees.First().Id,
            DepartmentId = committee.DepartmentId,
            OfficeId = committee.OfficeId,
            CommitteeId = committee.Id,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        });
    }
}
