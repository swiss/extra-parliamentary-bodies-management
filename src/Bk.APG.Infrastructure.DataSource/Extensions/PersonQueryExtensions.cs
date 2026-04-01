using Bk.APG.Business.Models;
using Bk.APG.CrossCutting;
using Microsoft.EntityFrameworkCore;

namespace Bk.APG.Infrastructure.DataSource.Extensions;

public static class PersonQueryExtensions
{
    public static IQueryable<Person> FilterPersons(this IQueryable<Person> query, PersonFilterParameters? filterParameters)
    {
        if (filterParameters is null)
        {
            return query;
        }

        if (!string.IsNullOrWhiteSpace(filterParameters.FreeText))
        {
            var filters = filterParameters.FreeText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var filter in filters)
            {
#pragma warning disable CA1305 //culture does not work in EF queries
                query = query.Where(y => EF.Functions.ILike(y.GivenName, $"%{filter}%")
                    || EF.Functions.ILike(y.Surname, $"%{filter}%")
                    || y.BirthYear.ToString().Contains(filter)
                    || (!string.IsNullOrEmpty(y.CorrespondenceAddress!.City) && EF.Functions.ILike(y.CorrespondenceAddress.City, $"%{filter}%"))
                    || EF.Functions.ILike(y.Id.ToString(), $"%{filter}%"));
#pragma warning restore CA1305
            }
        }

        if (filterParameters.LanguageIds is not null && filterParameters.LanguageIds.Any())
        {
            query = query.Where(y => filterParameters.LanguageIds.Contains(y.LanguageId));
        }

        if (filterParameters.CantonIds is not null && filterParameters.CantonIds.Any())
        {
            query = query.Where(y => y.CorrespondenceAddress!.CantonId.HasValue
                && filterParameters.CantonIds.Contains(y.CorrespondenceAddress.CantonId.Value));
        }

        if (filterParameters.HasActiveMembership is not null && filterParameters.HasActiveMembership.Any())
        {
            query = query.Where(y => (filterParameters.HasActiveMembership.Contains(false) && (y.Memberships.Count == 0 || !y.Memberships.AsQueryable().Any(Membership.IsActiveExpression)))
                || (filterParameters.HasActiveMembership.Contains(true) && y.Memberships.AsQueryable().Any(Membership.IsActiveExpression)));
        }

        return query;
    }

    public static IQueryable<Person> SortPersons(this IQueryable<Person> query, string sort, SortDirection sortDirection)
    {
        ArgumentNullException.ThrowIfNull(sort);

#pragma warning disable CA1308
        return sort.ToLowerInvariant() switch
#pragma warning restore CA1308
        {
            "surname" => sortDirection == SortDirection.Asc
                ? query.OrderBy(item => item.Surname)
                : query.OrderByDescending(item => item.Surname),
            "givenname" => sortDirection == SortDirection.Asc
                ? query.OrderBy(item => item.GivenName)
                : query.OrderByDescending(item => item.GivenName),
            "birthyear" => sortDirection == SortDirection.Asc
                ? query.OrderBy(item => item.BirthYear)
                : query.OrderByDescending(item => item.BirthYear),
            "hasactivemembership" => sortDirection == SortDirection.Asc
                ? query.OrderBy(item => item.Memberships.AsQueryable().Any(Membership.IsActiveExpression))
                : query.OrderByDescending(item => item.Memberships.AsQueryable().Any(Membership.IsActiveExpression)),
            "canton" => sortDirection == SortDirection.Asc
                ? query.OrderBy(item => item.CorrespondenceAddress!.Canton!.Sort)
                    .ThenBy(item => item.CorrespondenceAddress!.Canton!.TextDe) // TODO: sprachabhängige Sortierung
                : query.OrderByDescending(item => item.CorrespondenceAddress!.Canton!.Sort)
                    .ThenByDescending(item => item.CorrespondenceAddress!.Canton!.TextDe), // TODO: sprachabhängige Sortierung
            "city" => sortDirection == SortDirection.Asc
                ? query.OrderBy(item => item.CorrespondenceAddress!.City)
                : query.OrderByDescending(item => item.CorrespondenceAddress!.City),
            "language" => sortDirection == SortDirection.Asc
                ? query.OrderBy(item => item.CorrespondenceLanguage!.Sort)
                : query.OrderByDescending(item => item.CorrespondenceLanguage!.Sort),
            _ => query
        };
    }

    public static IQueryable<Person> FilterPersonDataByPermission(this IQueryable<Person> query, DateOnly startDate, Guid departmentId, Guid officeId, Guid committeeId)
    {
        if (departmentId == Guid.Empty && officeId == Guid.Empty && committeeId == Guid.Empty)
        {
            return query;
        }

        if (departmentId != Guid.Empty)
        {
            query = query.Where(c => c.Memberships.Any(m => m.Committee!.DepartmentId == departmentId && m.BeginDate <= startDate && m.EndDate >= startDate));
        }

        if (officeId != Guid.Empty)
        {
            query = query.Where(c => c.Memberships.Any(m => m.Committee!.OfficeId == officeId && m.BeginDate <= startDate && m.EndDate >= startDate));
        }

        if (committeeId != Guid.Empty)
        {
            query = query.Where(c => c.Memberships.Any(m => m.Committee!.Id == committeeId && m.BeginDate <= startDate && m.EndDate >= startDate));
        }

        return query;
    }
}
