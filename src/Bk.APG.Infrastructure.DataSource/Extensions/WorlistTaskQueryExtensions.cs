using System.Globalization;
using System.Linq.Expressions;
using Bk.APG.Business.Models;
using Bk.APG.Common.Resources;
using Bk.APG.CrossCutting;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Extensions;

public static class WorklistTaskQueryExtensions
{
    public static IQueryable<WorklistTask> SortWorklistTasks(this IQueryable<WorklistTask> worklistTasks, string sort, SortDirection sortDirection, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        return sort switch
        {
            "assignedBy" => sortDirection == SortDirection.Asc
                ? worklistTasks.OrderBy(w => w.AssignedBy!.Role == Role.Department ? BusinessTexts.Worklist_DepartmentRole :
                    w.AssignedBy!.Role == Role.Office ? BusinessTexts.Worklist_OfficeRole :
                    w.AssignedBy!.Role == Role.Secretariat ? BusinessTexts.Worklist_SecretariatRole :
                    w.AssignedBy.ExternalId)
                : worklistTasks.OrderByDescending(w => w.AssignedBy!.Role == Role.Department ? BusinessTexts.Worklist_DepartmentRole :
                    w.AssignedBy!.Role == Role.Office ? BusinessTexts.Worklist_OfficeRole :
                    w.AssignedBy!.Role == Role.Secretariat ? BusinessTexts.Worklist_SecretariatRole :
                    w.AssignedBy.ExternalId),
            "assignedTo" => sortDirection == SortDirection.Asc
                ? worklistTasks.OrderBy(w => w.AssignedTo!.Role == Role.Department ? BusinessTexts.Worklist_DepartmentRole :
                    w.AssignedTo!.Role == Role.Office ? BusinessTexts.Worklist_OfficeRole :
                    w.AssignedTo!.Role == Role.Secretariat ? BusinessTexts.Worklist_SecretariatRole :
                    w.AssignedTo.ExternalId)
                : worklistTasks.OrderByDescending(w => w.AssignedTo!.Role == Role.Department ? BusinessTexts.Worklist_DepartmentRole :
                    w.AssignedTo!.Role == Role.Office ? BusinessTexts.Worklist_OfficeRole :
                    w.AssignedTo!.Role == Role.Secretariat ? BusinessTexts.Worklist_SecretariatRole :
                    w.AssignedTo.ExternalId),
            "committee" => worklistTasks.SortByProperty(w => w.Committee!.DescriptionGerman, w => w.Committee!.DescriptionFrench, w => w.Committee!.DescriptionItalian, sortDirection, cultureInfo),
            "department" => worklistTasks.SortByProperty(w => w.Department!.TextDe, w => w.Department!.TextFr, w => w.Department!.TextIt, sortDirection, cultureInfo),
            "office" => worklistTasks.SortByProperty(w => w.Office!.TextDe, w => w.Office!.TextFr, w => w.Office!.TextIt, sortDirection, cultureInfo),
            "worklistTaskType" => worklistTasks.SortByProperty(w => w.WorklistTaskType!.TextDe, w => w.WorklistTaskType!.TextFr, w => w.WorklistTaskType!.TextIt, sortDirection, cultureInfo),
            "worklistTaskState" => worklistTasks.SortByProperty(w => w.WorklistTaskState!.TextDe, w => w.WorklistTaskState!.TextFr, w => w.WorklistTaskState!.TextIt, sortDirection, cultureInfo),
            "created" => sortDirection == SortDirection.Asc ? worklistTasks.OrderBy(w => w.Created) : worklistTasks.OrderByDescending(w => w.Created),
            "dueDate" => sortDirection == SortDirection.Asc ? worklistTasks.OrderBy(w => w.DueDate) : worklistTasks.OrderByDescending(w => w.DueDate),
            _ => worklistTasks
        };
    }

    public static IQueryable<WorklistTask> FilterWorklistTask(this IQueryable<WorklistTask> query, WorklistFilterParameters? filterParameter, bool isAdmin, List<Guid>? eiamAssignmentIds = null)
    {
        if (!isAdmin && eiamAssignmentIds is not null && eiamAssignmentIds.Count != 0)
        {
            query = query.Where(x => eiamAssignmentIds.Contains(x.AssignedToId));
        }

        if (filterParameter is null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filterParameter.Committee))
        {
            var filters = filterParameter.Committee.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var filter in filters)
            {
                query = query.Where(y => y.Committee != null &&
                    (
                        EF.Functions.ILike(y.Committee.DescriptionGerman, $"%{filter}%") ||
                        EF.Functions.ILike(y.Committee.DescriptionFrench, $"%{filter}%") ||
                        EF.Functions.ILike(y.Committee.DescriptionItalian, $"%{filter}%") ||
                        EF.Functions.ILike(y.Committee.DescriptionRomansh, $"%{filter}%"))
                );
            }
        }

        if (filterParameter.DepartmentIds is not null && filterParameter.DepartmentIds.Any())
        {
            query = query.Where(wl => wl.DepartmentId.HasValue && filterParameter.DepartmentIds.ToList().Contains(wl.DepartmentId.Value));
        }

        if (filterParameter.OfficeIds is not null && filterParameter.OfficeIds.Any())
        {
            query = query.Where(wl => wl.OfficeId.HasValue && filterParameter.OfficeIds.ToList().Contains(wl.OfficeId.Value));
        }

        if (filterParameter.WorklistTaskStateIds is not null && filterParameter.WorklistTaskStateIds.Any())
        {
            query = query.Where(c => filterParameter.WorklistTaskStateIds.Contains(c.WorklistTaskStateId));
        }

        if (filterParameter.WorklistTaskTypeIds is not null && filterParameter.WorklistTaskTypeIds.Any())
        {
            query = query.Where(c => filterParameter.WorklistTaskTypeIds.Contains(c.WorklistTaskTypeId));
        }

        if (filterParameter.AssignedBy is not null)
        {
            query = query.Where(c => c.AssignedBy != null && c.AssignedBy.Role == (Role)Enum.Parse(typeof(Role), filterParameter.AssignedBy.ToString()));
        }

        if (filterParameter.AssignedTo is not null)
        {
            query = query.Where(c => c.AssignedTo != null && c.AssignedTo.Role == (Role)Enum.Parse(typeof(Role), filterParameter.AssignedTo.ToString()));
        }

        if (filterParameter.CreatedFrom is not null)
        {
            query = query.Where(c => c.Created.Date >= DateTime.SpecifyKind(filterParameter.CreatedFrom.Value.Date, DateTimeKind.Utc));
        }

        if (filterParameter.CreatedTo is not null)
        {
            query = query.Where(c => c.Created.Date <= DateTime.SpecifyKind(filterParameter.CreatedTo.Value.Date, DateTimeKind.Utc));
        }

        if (filterParameter.DueDateFrom is not null)
        {
            query = query.Where(c => c.DueDate >= filterParameter.DueDateFrom.Value);
        }

        if (filterParameter.DueDateTo is not null)
        {
            query = query.Where(c => c.DueDate <= filterParameter.DueDateTo.Value);
        }

        return query;
    }

    private static IQueryable<WorklistTask> SortByProperty(
        this IQueryable<WorklistTask> worklistTasks,
        Expression<Func<WorklistTask, string>> propertyDe,
        Expression<Func<WorklistTask, string>> propertyFr,
        Expression<Func<WorklistTask, string>> propertyIt,
        SortDirection sortDirection,
        CultureInfo cultureInfo)
    {
        var selectedProperty = cultureInfo.TwoLetterISOLanguageName switch
        {
            Language.German => propertyDe,
            Language.French => propertyFr,
            Language.Italian => propertyIt,
            _ => propertyDe
        };

        return sortDirection == SortDirection.Asc
            ? worklistTasks.OrderBy(selectedProperty)
            : worklistTasks.OrderByDescending(selectedProperty);
    }
}
