using System.Globalization;
using System.Linq.Expressions;
using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Extensions;

public static class CommitteeQueryExtensions
{
    public static IQueryable<Committee> SortCommittees(this IQueryable<Committee> committees, string sort, SortDirection sortDirection, CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(sort);
        ArgumentNullException.ThrowIfNull(cultureInfo);

#pragma warning disable CA1308
        return sort.ToLowerInvariant() switch
#pragma warning restore CA1308
        {
            "committeeid" => sortDirection == SortDirection.Asc
                ? committees.OrderBy(c => c.CommitteeNumber)
                : committees.OrderByDescending(c => c.CommitteeNumber),
            "description" => committees.SortByProperty(c => c.DescriptionGerman, c => c.DescriptionFrench, c => c.DescriptionItalian, sortDirection, cultureInfo),
            "department" => committees.SortByProperty(c => c.Department!.TextDe, c => c.Department!.TextFr, c => c.Department!.TextIt, sortDirection, cultureInfo),
            "level" => committees.SortByProperty(c => c.CommitteeLevel!.TextDe, c => c.CommitteeLevel!.TextFr, c => c.CommitteeLevel!.TextIt, sortDirection, cultureInfo),
            "office" => committees.SortByProperty(c => c.Office!.TextDe, c => c.Office!.TextFr, c => c.Office!.TextIt, sortDirection, cultureInfo),
            "committeetype" => committees.SortByProperty(c => c.CommitteeType!.TextDe, c => c.CommitteeType!.TextFr, c => c.CommitteeType!.TextIt, sortDirection, cultureInfo),
            "term" => committees.SortByProperty(c => c.TermOfOffice!.TextDe, c => c.TermOfOffice!.TextFr, c => c.TermOfOffice!.TextIt, sortDirection, cultureInfo),
            "isactive" => sortDirection == SortDirection.Asc
                ? committees.OrderBy(Committee.IsActiveExpression)
                : committees.OrderByDescending(Committee.IsActiveExpression),
            "ismarketorientated" => sortDirection == SortDirection.Asc
                ? committees.OrderBy(c => c.MarketOrientated)
                : committees.OrderByDescending(c => c.MarketOrientated),
            "hassupervisionduty" => sortDirection == SortDirection.Asc
                ? committees.OrderBy(c => c.SupervisionDuty)
                : committees.OrderByDescending(c => c.SupervisionDuty),
            _ => committees
        };
    }

    private static IQueryable<Committee> SortByProperty(
        this IQueryable<Committee> committees,
        Expression<Func<Committee, string>> propertyDe,
        Expression<Func<Committee, string>> propertyFr,
        Expression<Func<Committee, string>> propertyIt,
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
            ? committees.OrderBy(selectedProperty)
            : committees.OrderByDescending(selectedProperty);
    }



    public static IQueryable<Committee> FilterCommittees(this IQueryable<Committee> query, CommitteeFilterParameters? filterParameter)
    {
        if (filterParameter is null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filterParameter.FreeText))
        {
            var filters = filterParameter.FreeText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var filter in filters)
            {
#pragma warning disable CA1305 //Culture does not work in EF queries
                query = query.Where(y => EF.Functions.ILike(y.DescriptionGerman, $"%{filter}%")
                    || EF.Functions.ILike(y.DescriptionFrench, $"%{filter}%")
                    || EF.Functions.ILike(y.DescriptionItalian, $"%{filter}%")
                    || EF.Functions.ILike(y.DescriptionRomansh, $"%{filter}%")
                    || y.CommitteeNumber.ToString().Contains(filter));
#pragma warning restore CA1305
            }
        }

        if (filterParameter.LevelIds is not null && filterParameter.LevelIds.Any())
        {
            query = query.Where(c => filterParameter.LevelIds.Contains(c.CommitteeLevelId));
        }

        if (filterParameter.DepartmentIds is not null && filterParameter.DepartmentIds.Any())
        {
            query = query.Where(c => filterParameter.DepartmentIds.Contains(c.DepartmentId));
        }

        if (filterParameter.OfficeIds is not null && filterParameter.OfficeIds.Any())
        {
            query = query.Where(c => filterParameter.OfficeIds.Contains(c.OfficeId));
        }

        if (filterParameter.CommitteeTypeIds is not null && filterParameter.CommitteeTypeIds.Any())
        {
            query = query.Where(c => filterParameter.CommitteeTypeIds.Contains(c.CommitteeTypeId));
        }

        if (filterParameter.TermIds is not null && filterParameter.TermIds.Any())
        {
            query = query.Where(c => filterParameter.TermIds.Contains(c.TermOfOfficeId));
        }

        if (filterParameter.IsActive is not null && filterParameter.IsActive.Any() && filterParameter.IsActive.Count() < 2)
        {
            if (filterParameter.IsActive.Contains(false))
            {
                query = query.Where(Expression.Lambda<Func<Committee, bool>>(Expression.Not(Committee.IsActiveExpression.Body), Committee.IsActiveExpression.Parameters));
            }

            if (filterParameter.IsActive.Contains(true))
            {
                query = query.Where(Committee.IsActiveExpression);
            }
        }

        if (filterParameter.IsMarketOrientated is not null && filterParameter.IsMarketOrientated.Any())
        {
            if (filterParameter.IsMarketOrientated.Contains(true) && filterParameter.IsMarketOrientated.Contains(false))
            {
                query = query.Where(c => c.MarketOrientated == true || c.MarketOrientated == false);
            }
            else if (filterParameter.IsMarketOrientated.Contains(true))
            {
                query = query.Where(c => c.MarketOrientated == true);
            }
            else if (filterParameter.IsMarketOrientated.Contains(false))
            {
                query = query.Where(c => c.MarketOrientated == false);
            }
        }

        if (filterParameter.HasSupervisionDuty is not null && filterParameter.HasSupervisionDuty.Any())
        {
            if (filterParameter.HasSupervisionDuty.Contains(true) && filterParameter.HasSupervisionDuty.Contains(false))
            {
                query = query.Where(c => c.SupervisionDuty == true || c.SupervisionDuty == false);
            }
            else if (filterParameter.HasSupervisionDuty.Contains(true))
            {
                query = query.Where(c => c.SupervisionDuty == true);
            }
            else if (filterParameter.HasSupervisionDuty.Contains(false))
            {
                query = query.Where(c => c.SupervisionDuty == false);
            }
        }

        return query;
    }

    public static IQueryable<Committee> FilterCommitteeByPermission(this IQueryable<Committee> query, Guid departmentId, Guid officeId, Guid committeeId, ReportFilterParametersDto? filterDto = null)
    {
        if (filterDto != null)
        {
            var reportIsGeneralElectionOnly = false;

            if (filterDto.DocumentType == ReportType.Vacancies)
            {
                reportIsGeneralElectionOnly = true;
                query = query.Where(c => c.VacanciesGeneralElection > 0);
            }

            if (reportIsGeneralElectionOnly)
            {
                query = query.Where(c => c.CommitteeLevelId == CommitteeLevel.FederalCouncilGuid && c.TermOfOfficeId == TermOfOffice.Period4YearsInGeneralElectionGuid);
            }

            if (filterDto.DepartmentIds is not null && filterDto.DepartmentIds.Any())
            {
                query = query.Where(c => filterDto.DepartmentIds.Contains(c.DepartmentId));
            }

            if (filterDto.OfficeIds is not null && filterDto.OfficeIds.Any())
            {
                query = query.Where(c => filterDto.OfficeIds.Contains(c.OfficeId));
            }

            if (filterDto.CommitteeTypeIds is not null && filterDto.CommitteeTypeIds.Any())
            {
                query = query.Where(c => filterDto.CommitteeTypeIds.Contains(c.CommitteeTypeId));
            }
        }

        if (departmentId == Guid.Empty && officeId == Guid.Empty && committeeId == Guid.Empty)
        {
            return query;
        }

        if (departmentId != Guid.Empty)
        {
            query = query.Where(c => c.DepartmentId == departmentId);
        }

        if (officeId != Guid.Empty)
        {
            query = query.Where(c => c.OfficeId == officeId);
        }

        if (committeeId != Guid.Empty)
        {
            query = query.Where(c => c.Id == committeeId);
        }

        return query;
    }

    public static IQueryable<Committee> FilterCommitteeByReportFilterParametersDto(this IQueryable<Committee> query, ReportFilterParametersDto filterDto, Guid departmentId, Guid officeId, Guid committeeId)
    {
        if (filterDto != null)
        {
            if (filterDto.DepartmentIds is not null && filterDto.DepartmentIds.Any())
            {
                query = query.Where(c => filterDto.DepartmentIds.Contains(c.DepartmentId));
            }

            if (filterDto.OfficeIds is not null && filterDto.OfficeIds.Any())
            {
                query = query.Where(c => filterDto.OfficeIds.Contains(c.OfficeId));
            }

            if (filterDto.CommitteeTypeIds is not null && filterDto.CommitteeTypeIds.Any())
            {
                query = query.Where(c => filterDto.CommitteeTypeIds.Contains(c.CommitteeTypeId));
            }

            if (filterDto.AnalysisDate1 is not null)
            {
                query = query.Where(c => c.BeginDate < filterDto.AnalysisDate1 && (c.EndDate == null || c.EndDate > filterDto.AnalysisDate1));
            }

            if (filterDto.CommitteeIds is not null && filterDto.CommitteeIds.Any())
            {
                query = query.Where(c => filterDto.CommitteeIds.Contains(c.Id));
            }
        }

        if (departmentId == Guid.Empty && officeId == Guid.Empty && committeeId == Guid.Empty)
        {
            return query;
        }

        if (departmentId != Guid.Empty)
        {
            query = query.Where(c => c.DepartmentId == departmentId);
        }

        if (officeId != Guid.Empty)
        {
            query = query.Where(c => c.OfficeId == officeId);
        }

        if (committeeId != Guid.Empty)
        {
            query = query.Where(c => c.Id == committeeId);
        }

        return query;
    }

    public static IQueryable<Committee> FilterCommitteesForFormLetter(this IQueryable<Committee> query, FormLetterFilterParameters filterParameter, List<Guid> electionTypeIds)
    {
        ArgumentNullException.ThrowIfNull(filterParameter);

        if (electionTypeIds != null)
        {
            if (filterParameter.ElectionTypeIds != null && filterParameter.ElectionTypeIds.Any())
            {
                electionTypeIds = electionTypeIds
                    .Where(id => filterParameter.ElectionTypeIds.Contains(id))
                    .ToList();
            }

            query = query.Where(c => c.Memberships.Any(m => electionTypeIds!.Contains(m.ElectionTypeId)));
        }

        // here, only memberships, ending exactly at the end of the term of office are relevant.
        query = query.Where(c => c.Memberships.Any(m => m.EndDate == filterParameter.EndDateCurrentTermOfOfficeDate));

        if (filterParameter.CorrespondenceLanguageIds is not null && filterParameter.CorrespondenceLanguageIds.Any())
        {
            query = query.Where(c => c.Memberships.Any(m => filterParameter.CorrespondenceLanguageIds.Contains(m.Person!.CorrespondenceLanguageId)));
        }

        if (filterParameter.DepartmentIds is not null && filterParameter.DepartmentIds.Any())
        {
            query = query.Where(c => filterParameter.DepartmentIds.Contains(c.DepartmentId));
        }

        if (filterParameter.OfficeIds is not null && filterParameter.OfficeIds.Any())
        {
            query = query.Where(c => filterParameter.OfficeIds.Contains(c.OfficeId));
        }

        if (filterParameter.CommitteeTypeIds is not null && filterParameter.CommitteeTypeIds.Any())
        {
            query = query.Where(c => filterParameter.CommitteeTypeIds.Contains(c.CommitteeTypeId));
        }

        if (filterParameter.CommitteeIds is not null && filterParameter.CommitteeIds.Any())
        {
            query = query.Where(c => filterParameter.CommitteeIds.Contains(c.Id));
        }

        return query;
    }
}
