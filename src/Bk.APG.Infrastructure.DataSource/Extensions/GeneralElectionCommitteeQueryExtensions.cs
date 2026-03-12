using System.Globalization;
using System.Linq.Expressions;
using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Extensions;

public static class GeneralElectionCommitteeQueryExtensions
{
    public static IQueryable<GeneralElectionCommittee> FilterGeneralElectionCommitteeByPermission(this IQueryable<GeneralElectionCommittee> query, Guid departmentId, Guid officeId, Guid committeeId)
    {
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
            query = query.Where(c => c.CommitteeId == committeeId);
        }

        return query;
    }

    public static IQueryable<GeneralElectionCommittee> FilterGeneralElectionCommittees(this IQueryable<GeneralElectionCommittee> query, GeneralElectionCommitteeFilterParameters filterParameter)
    {
        if (!string.IsNullOrWhiteSpace(filterParameter.FreeText))
        {
            var filters = filterParameter.FreeText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var filter in filters)
            {
                query = query.Where(y => EF.Functions.ILike(y.DescriptionGerman, $"%{filter}%")
                    || EF.Functions.ILike(y.DescriptionFrench, $"%{filter}%")
                    || EF.Functions.ILike(y.DescriptionItalian, $"%{filter}%")
                    || EF.Functions.ILike(y.DescriptionRomansh, $"%{filter}%"));
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

        if (filterParameter.IsNew is not null && filterParameter.IsNew.Any())
        {
            if (filterParameter.IsNew.Contains(true) && !filterParameter.IsNew.Contains(false))
            {
                query = query.Where(c => DateOnly.FromDateTime(c.Committee!.Created) >= c.Committee.TermOfOfficeDate!.BeginDate);
            }
            else if (filterParameter.IsNew.Contains(false) && !filterParameter.IsNew.Contains(true))
            {
                query = query.Where(c => DateOnly.FromDateTime(c.Committee!.Created) <= c.Committee!.TermOfOfficeDate!.BeginDate);
            }
        }

        if (filterParameter.Vacancies is not null && filterParameter.Vacancies.Any())
        {
            if (filterParameter.Vacancies.Contains(true) && !filterParameter.Vacancies.Contains(false))
            {
                query = query.Where(c =>
                    c.VacanciesGeneralElection != null
                        ? c.VacanciesGeneralElection > 0
                        : c.MinimalMembers - c.MembershipCandidates.Count(x => x.IsSelected && !x.IsDeleted) > 0);
            }
            else if (filterParameter.Vacancies.Contains(false) && !filterParameter.Vacancies.Contains(true))
            {
                query = query.Where(c =>
                    c.VacanciesGeneralElection != null
                        ? c.VacanciesGeneralElection == 0
                        : c.MinimalMembers - c.MembershipCandidates.Count(x => x.IsSelected && !x.IsDeleted) <= 0);
            }
        }

        if (filterParameter.StatusProposal is not null && filterParameter.StatusProposal.Any())
        {
            if (filterParameter.StatusProposal.Contains(true) && !filterParameter.StatusProposal.Contains(false))
            {
                query = query.Where(c => c.CandidateListStateId == CandidateListState.ReadyForFederalCouncilProposalFinalized);
            }
            else if (filterParameter.StatusProposal.Contains(false) && !filterParameter.StatusProposal.Contains(true))
            {
                query = query.Where(c => c.CandidateListStateId != CandidateListState.ReadyForFederalCouncilProposalFinalized);
            }
        }

        if (filterParameter.CommitteeIds is not null && filterParameter.CommitteeIds.Any())
        {
            query = query.Where(c => filterParameter.CommitteeIds.Contains(c.CommitteeId));
        }

        return query;
    }

    public static IQueryable<GeneralElectionCommittee> FilterGeneralElectionCommitteesForExport(this IQueryable<GeneralElectionCommittee> query, GeneralElectionCommitteeExportFilterParameters filterParameter)
    {
        if (filterParameter.CorrespondenceLanguageIds is not null && filterParameter.CorrespondenceLanguageIds.Any())
        {
            query = query.Where(c => c.MembershipCandidates.Any(m => filterParameter.CorrespondenceLanguageIds.Contains(m.Person!.CorrespondenceLanguageId)));
        }

        if (filterParameter.ElectionTypeIds is not null && filterParameter.ElectionTypeIds.Any())
        {
            query = query.Where(c => c.MembershipCandidates.Any(m => filterParameter.ElectionTypeIds.Contains(m.ElectionTypeId)));
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
            query = query.Where(c => filterParameter.CommitteeIds.Contains(c.CommitteeId));
        }

        return query;
    }

    public static IQueryable<GeneralElectionCommittee> SortGeneralElectionCommittees(this IQueryable<GeneralElectionCommittee> committees, string sort, SortDirection sortDirection, CultureInfo cultureInfo)
    {
        return sort.ToLowerInvariant() switch
        {
            "description" => committees.SortByProperty(c => c.DescriptionGerman, c => c.DescriptionFrench, c => c.DescriptionItalian, sortDirection, cultureInfo),
            "department" => committees.SortByProperty(c => c.Department!.TextDe, c => c.Department!.TextFr, c => c.Department!.TextIt, sortDirection, cultureInfo),
            "level" => committees.SortByProperty(c => c.CommitteeLevel!.TextDe, c => c.CommitteeLevel!.TextFr, c => c.CommitteeLevel!.TextIt, sortDirection, cultureInfo),
            "office" => committees.SortByProperty(c => c.Office!.TextDe, c => c.Office!.TextFr, c => c.Office!.TextIt, sortDirection, cultureInfo),
            "committeetype" => committees.SortByProperty(c => c.CommitteeType!.TextDe, c => c.CommitteeType!.TextFr, c => c.CommitteeType!.TextIt, sortDirection, cultureInfo),
            "ismarketorientated" => sortDirection == SortDirection.Asc
                ? committees.OrderBy(c => c.MarketOrientated)
                : committees.OrderByDescending(c => c.MarketOrientated),
            "hassupervisionduty" => sortDirection == SortDirection.Asc
                ? committees.OrderBy(c => c.SupervisionDuty)
                : committees.OrderByDescending(c => c.SupervisionDuty),
            _ => committees
        };
    }

    private static IQueryable<GeneralElectionCommittee> SortByProperty(
        this IQueryable<GeneralElectionCommittee> committees,
        Expression<Func<GeneralElectionCommittee, string>> propertyDe,
        Expression<Func<GeneralElectionCommittee, string>> propertyFr,
        Expression<Func<GeneralElectionCommittee, string>> propertyIt,
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
}
