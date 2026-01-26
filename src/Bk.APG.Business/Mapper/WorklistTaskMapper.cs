using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class WorklistTaskMapper
{
    public static WorklistTaskDto ToWorklistTaskDto(WorklistTask worklistTask)
    {
        return new WorklistTaskDto
        {
            Id = worklistTask.Id,
            AssignedTo = worklistTask.AssignedTo!.GetText(),
            AssignedBy = worklistTask.AssignedBy!.GetText(),
            NavigationUrl = worklistTask.GetNavigationUrl(),
            Committee = worklistTask.Committee?.GetDescription(),
            Department = worklistTask.Department?.GetText(),
            Office = worklistTask.Office?.GetText(),
            ParentTaskId = worklistTask.ParentTaskId,
            DueDate = worklistTask.DueDate,
            WorklistTaskType = worklistTask.GetTaskTypeText(),
            WorklistTaskState = worklistTask.WorklistTaskState!.GetText(),
            CreatedBy = worklistTask.CreatedBy,
            Created = worklistTask.Created,
            IsInactive = worklistTask.WorklistTaskStateId == WorklistTaskState.Inactive,
            IsCompleted = worklistTask.WorklistTaskStateId == WorklistTaskState.Completed,
            IsOverdue = worklistTask.IsOverdue
        };
    }

    public static WorklistFilterParameters? ToWorklistTaskFilterParameters(WorklistFilterParametersDto? dto)
    {
        if (dto is null)
        {
            return null;
        }

        return new WorklistFilterParameters
        {
            Committee = dto.Committee,
            DepartmentIds = dto.DepartmentIds,
            OfficeIds = dto.OfficeIds,
            WorklistTaskStateIds = dto.WorklistTaskStateIds,
            WorklistTaskTypeIds = dto.WorklistTaskTypeIds,
            AssignedBy = dto.AssignedBy,
            AssignedTo = dto.AssignedTo,
            CreatedFrom = dto.CreatedFrom,
            CreatedTo = dto.CreatedTo,
            DueDateFrom = dto.DueDateFrom,
            DueDateTo = dto.DueDateTo,
        };
    }

    public static WorklistTask ToWorklistTask(WorklistTaskCreateDto worklistTaskCreateDto, Guid assignedById, string currentUserName)
    {
        return new WorklistTask
        {
            AssignedToId = worklistTaskCreateDto.AssignedToId!.Value,
            AssignedById = assignedById,
            DueDate = worklistTaskCreateDto.DueDate,
            Description = worklistTaskCreateDto.Description ?? string.Empty,
            WorklistTaskTypeId = worklistTaskCreateDto.WorklistTaskTypeId,
            WorklistTaskStateId = worklistTaskCreateDto.WorklistTaskStateId ?? WorklistTaskState.Active,
            ParentTaskId = worklistTaskCreateDto.ParentTaskId,
            DepartmentId = worklistTaskCreateDto.DepartmentId,
            OfficeId = worklistTaskCreateDto.OfficeId,
            CommitteeId = worklistTaskCreateDto.CommitteeId,
            MembershipId = worklistTaskCreateDto.MembershipId,
            PersonId = worklistTaskCreateDto.PersonId,
            GeneralElectionCommitteeId = worklistTaskCreateDto.GeneralElectionCommitteeId,
            MembershipCandidateId = worklistTaskCreateDto.MembershipCandidateId,
            TermOfOfficeDateId = worklistTaskCreateDto.TermOfOfficeDateId,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        };
    }

    public static WorklistTaskUpdateDto ToWorklistTaskUpdateDto(WorklistTask worklistTask)
    {
        return new WorklistTaskUpdateDto
        {
            Id = worklistTask.Id,
            Description = worklistTask.Description,
            DueDate = worklistTask.DueDate,
            WorklistTaskType = worklistTask.WorklistTaskType!.GetText(),
            WorklistTaskState = worklistTask.WorklistTaskState!.GetText(),
            AssignedTo = worklistTask.AssignedTo!.GetText(),
            AssignedBy = worklistTask.AssignedBy!.GetText()
        };
    }

    public static WorklistTaskCreateDto CreateGeneralElectionMainWorklistTaskDto(Guid termOfOfficeDateId, DateOnly dueDate, string description)
    {
        return new WorklistTaskCreateDto
        {
            AssignedToId = EiamAssignment.AdminId,
            WorklistTaskTypeId = WorklistTaskType.GeneralElectionStart,
            // As this task has no immediate action, we set it to "Inactive" until all children are done
            WorklistTaskStateId = WorklistTaskState.Inactive,
            Description = description,
            DueDate = dueDate,
            TermOfOfficeDateId = termOfOfficeDateId
        };
    }

    public static WorklistTaskCreateDto CreateGeneralElectionDepartmentWorklistTaskDto(Guid parentId, string parentTaskDescription, Guid departmentId, Guid assignedToId, Guid termOfOfficeDateId)
    {
        return new WorklistTaskCreateDto
        {
            AssignedToId = assignedToId,
            WorklistTaskTypeId = WorklistTaskType.GeneralElectionDispatch,
            WorklistTaskStateId = WorklistTaskState.Active,
            ParentTaskId = parentId,
            DepartmentId = departmentId,
            Description = parentTaskDescription,
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            TermOfOfficeDateId = termOfOfficeDateId
        };
    }
}
