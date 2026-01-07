using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;

namespace Bk.APG.Business.Services;

public interface IWorklistTaskService
{
    Task<PagedResultDto<WorklistTaskDto>> GetWorklistTasks(PagingParametersDto paging, WorklistFilterParametersDto? filterParametersDto, string? sort, SortDirection? sortDirection);
    Task<WorklistTaskUpdateDto> GetWorklistTaskForUpdate(Guid id);
    Task<WorklistTask> CreateWorklistTaskByAdmin(WorklistTaskCreateDto worklistTaskCreateDto);
    Task RemoveAllGeneralElectionTasks(Guid termOfOfficeDateId);
    Task<WorklistTaskUpdateDto> UpdateWorklistTask(Guid id, WorklistTaskUpdateDto updateDto);
    Task ForwardWorklistTask(Guid id, WorklistTaskForwardDto forwardDto);
}
