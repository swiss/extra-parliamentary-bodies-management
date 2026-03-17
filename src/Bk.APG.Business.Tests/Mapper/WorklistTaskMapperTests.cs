using Bk.APG.Business.Dtos;
using Bk.APG.Business.Extensions;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting.Tests.Builders;

namespace Bk.APG.Business.Tests.Mapper;

[TestFixture]
internal class WorklistTaskMapperTests
{
    [Test]
    public void ToWorklistTaskDto_ShouldMapCorrectly()
    {
        var worklistTask = new WorklistTaskBuilder().Build();

        var worklistTaskDto = WorklistTaskMapper.ToWorklistTaskDto(worklistTask);

        Assert.That(worklistTaskDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(worklistTaskDto.Id, Is.EqualTo(worklistTask.Id));
            Assert.That(worklistTaskDto.AssignedTo, Is.EqualTo(worklistTask.AssignedTo!.GetText()));
            Assert.That(worklistTaskDto.AssignedBy, Is.EqualTo(worklistTask.AssignedBy!.GetText()));
            Assert.That(worklistTaskDto.Committee, Is.EqualTo(worklistTask.Committee?.GetDescription()));
            Assert.That(worklistTaskDto.Department, Is.EqualTo(worklistTask.Department?.GetText()));
            Assert.That(worklistTaskDto.Office, Is.EqualTo(worklistTask.Office?.GetText()));
            Assert.That(worklistTaskDto.ParentTaskId, Is.EqualTo(worklistTask.ParentTaskId));
            Assert.That(worklistTaskDto.DueDate, Is.EqualTo(worklistTask.DueDate));
            Assert.That(worklistTaskDto.WorklistTaskType, Is.EqualTo(worklistTask.WorklistTaskType?.GetText()));
            Assert.That(worklistTaskDto.WorklistTaskState, Is.EqualTo(worklistTask.WorklistTaskState?.GetText()));
            Assert.That(worklistTaskDto.Section, Is.EqualTo(worklistTask.GetSection()));
            Assert.That(worklistTaskDto.Created, Is.EqualTo(worklistTask.Created));
            Assert.That(worklistTaskDto.CreatedBy, Is.EqualTo(worklistTask.CreatedBy));
            Assert.That(worklistTaskDto.NavigationUrl, Is.EqualTo(worklistTask.GetNavigationUrl()));
            Assert.That(worklistTaskDto.IsInactive, Is.EqualTo(worklistTask.WorklistTaskStateId == WorklistTaskState.Inactive));
            Assert.That(worklistTaskDto.IsCompleted, Is.EqualTo(worklistTask.WorklistTaskStateId == WorklistTaskState.Completed));
            Assert.That(worklistTaskDto.IsOverdue, Is.EqualTo(worklistTask.IsOverdue));
        });
    }

    [Test]
    public void ToWorklistTaskCreateDto_ShouldMapCorrectly()
    {
        var worklistTaskCreateDto = new WorklistTaskCreateDto
        {
            AssignedToId = Guid.NewGuid(),
            WorklistTaskTypeId = Guid.NewGuid(),
            WorklistTaskStateId = Guid.NewGuid(),
            Description = "description",
            DepartmentId = Guid.NewGuid(),
            OfficeId = Guid.NewGuid(),
            ParentTaskId = Guid.NewGuid(),
            CommitteeId = Guid.NewGuid(),
            MembershipId = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            GeneralElectionCommitteeId = Guid.NewGuid(),
            CandidateListId = Guid.NewGuid(),
            MembershipCandidateId = Guid.NewGuid(),
            TermOfOfficeDateId = Guid.NewGuid(),
            DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-300))
        };

        var assignedById = Guid.NewGuid();
        var worklistTask = WorklistTaskMapper.ToWorklistTask(worklistTaskCreateDto, assignedById, "Fritz Tester");

        Assert.That(worklistTask, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(worklistTask.AssignedToId, Is.EqualTo(worklistTaskCreateDto.AssignedToId));
            Assert.That(worklistTask.AssignedById, Is.EqualTo(assignedById));
            Assert.That(worklistTask.WorklistTaskTypeId, Is.EqualTo(worklistTaskCreateDto.WorklistTaskTypeId));
            Assert.That(worklistTask.WorklistTaskStateId, Is.EqualTo(worklistTaskCreateDto.WorklistTaskStateId));
            Assert.That(worklistTask.Description, Is.EqualTo(worklistTaskCreateDto.Description));
            Assert.That(worklistTask.DepartmentId, Is.EqualTo(worklistTaskCreateDto.DepartmentId));
            Assert.That(worklistTask.OfficeId, Is.EqualTo(worklistTaskCreateDto.OfficeId));
            Assert.That(worklistTask.ParentTaskId, Is.EqualTo(worklistTaskCreateDto.ParentTaskId));
            Assert.That(worklistTask.CommitteeId, Is.EqualTo(worklistTaskCreateDto.CommitteeId));
            Assert.That(worklistTask.MembershipId, Is.EqualTo(worklistTaskCreateDto.MembershipId));
            Assert.That(worklistTask.PersonId, Is.EqualTo(worklistTaskCreateDto.PersonId));
            Assert.That(worklistTask.GeneralElectionCommitteeId, Is.EqualTo(worklistTaskCreateDto.GeneralElectionCommitteeId));
            Assert.That(worklistTask.MembershipCandidateId, Is.EqualTo(worklistTaskCreateDto.MembershipCandidateId));
            Assert.That(worklistTask.TermOfOfficeDateId, Is.EqualTo(worklistTaskCreateDto.TermOfOfficeDateId));
            Assert.That(worklistTask.DueDate, Is.EqualTo(worklistTaskCreateDto.DueDate));
        });
    }

    [Test]
    public void ToWorklistTaskUpdateDto_ShouldMapCorrectly()
    {
        var worklistTask = new WorklistTaskBuilder()
            .Build();

        var worklistTaskUpdateDto = WorklistTaskMapper.ToWorklistTaskUpdateDto(worklistTask);

        Assert.That(worklistTaskUpdateDto, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(worklistTaskUpdateDto.Id, Is.EqualTo(worklistTask.Id));
            Assert.That(worklistTaskUpdateDto.Description, Is.EqualTo(worklistTask.Description));
            Assert.That(worklistTaskUpdateDto.DueDate, Is.EqualTo(worklistTask.DueDate));
        });
    }

    [Test]
    public void CreateGeneralElectionMainWorklistTaskDto_WithDto_ShouldReturnResult()
    {
        var termOfOfficeDateId = Guid.NewGuid();
        var dueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
        const string description = "description";

        var result = WorklistTaskMapper.CreateGeneralElectionMainWorklistTaskDto(termOfOfficeDateId, dueDate, description);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.AssignedToId, Is.EqualTo(EiamAssignment.AdminId));
            Assert.That(result.WorklistTaskTypeId, Is.EqualTo(WorklistTaskType.GeneralElectionStart));
            Assert.That(result.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Inactive));
            Assert.That(result.Description, Is.EqualTo(description));
            Assert.That(result.DueDate, Is.EqualTo(dueDate));
            Assert.That(result.TermOfOfficeDateId, Is.EqualTo(termOfOfficeDateId));
        });
    }

    [Test]
    public void CreateGeneralElectionEndWorklistTaskDto_WithDto_ShouldReturnResult()
    {
        var parentId = Guid.NewGuid();
        var termOfOfficeDateId = Guid.NewGuid();
        var dueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
        const string description = "description";

        var result = WorklistTaskMapper.CreateGeneralElectionEndWorklistTaskDto(parentId, termOfOfficeDateId, dueDate, description);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.AssignedToId, Is.EqualTo(EiamAssignment.AdminId));
            Assert.That(result.WorklistTaskTypeId, Is.EqualTo(WorklistTaskType.GeneralElectionEnd));
            Assert.That(result.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(result.ParentTaskId, Is.EqualTo(parentId));
            Assert.That(result.Description, Is.EqualTo(description));
            Assert.That(result.DueDate, Is.EqualTo(dueDate));
            Assert.That(result.TermOfOfficeDateId, Is.EqualTo(termOfOfficeDateId));
        });
    }

    [Test]
    public void CreateGeneralElectionDepartmentWorklistTaskDto_WithDto_ShouldReturnResult()
    {
        var parentId = Guid.NewGuid();
        const string parentTaskDescription = "parentTaskDescription";
        var termOfOfficeDateId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var assignedToId = Guid.NewGuid();

        var result = WorklistTaskMapper.CreateGeneralElectionDepartmentWorklistTaskDto(parentId, parentTaskDescription, departmentId, assignedToId, termOfOfficeDateId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.AssignedToId, Is.EqualTo(assignedToId));
            Assert.That(result.WorklistTaskTypeId, Is.EqualTo(WorklistTaskType.GeneralElectionDispatch));
            Assert.That(result.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(result.ParentTaskId, Is.EqualTo(parentId));
            Assert.That(result.DepartmentId, Is.EqualTo(departmentId));
            Assert.That(result.Description, Is.EqualTo(parentTaskDescription));
            Assert.That(result.TermOfOfficeDateId, Is.EqualTo(termOfOfficeDateId));
        });
    }

    [Test]
    public void CreateGeneralMeasureDepartmentCheckWorklistTaskDto_WithDto_ShouldReturnResult()
    {
        var parentId = Guid.NewGuid();
        var termOfOfficeDateId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var assignedToId = Guid.NewGuid();
        var dueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7));

        var result = WorklistTaskMapper.CreateGeneralMeasureDepartmentCheckWorklistTaskDto(parentId, departmentId, assignedToId, termOfOfficeDateId, dueDate);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.AssignedToId, Is.EqualTo(assignedToId));
            Assert.That(result.WorklistTaskTypeId, Is.EqualTo(WorklistTaskType.GeneralMeasureCheck));
            Assert.That(result.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Active));
            Assert.That(result.ParentTaskId, Is.EqualTo(parentId));
            Assert.That(result.DepartmentId, Is.EqualTo(departmentId));
            Assert.That(result.Description, Is.EqualTo(string.Empty));
            Assert.That(result.DueDate, Is.EqualTo(dueDate));
            Assert.That(result.TermOfOfficeDateId, Is.EqualTo(termOfOfficeDateId));
        }
    }

    [Test]
    public void CreateGeneralMeasureAdminValidationWorklistTaskDto_WithDto_ShouldReturnResult()
    {
        var parentId = Guid.NewGuid();
        var termOfOfficeDateId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var dueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14));

        var result = WorklistTaskMapper.CreateGeneralMeasureAdminValidationWorklistTaskDto(parentId, departmentId, termOfOfficeDateId, dueDate);

        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.AssignedToId, Is.EqualTo(EiamAssignment.AdminId));
            Assert.That(result.WorklistTaskTypeId, Is.EqualTo(WorklistTaskType.GeneralMeasureValidate));
            Assert.That(result.WorklistTaskStateId, Is.EqualTo(WorklistTaskState.Inactive));
            Assert.That(result.ParentTaskId, Is.EqualTo(parentId));
            Assert.That(result.DepartmentId, Is.EqualTo(departmentId));
            Assert.That(result.Description, Is.EqualTo(string.Empty));
            Assert.That(result.DueDate, Is.EqualTo(dueDate));
            Assert.That(result.TermOfOfficeDateId, Is.EqualTo(termOfOfficeDateId));
        }
    }
}
