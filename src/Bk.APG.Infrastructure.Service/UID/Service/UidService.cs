using Bk.APG.Business.Dtos;
using Bk.APG.Business.Services;
using Bk.APG.Infrastructure.Service.UID.Configuration;
using Bk.APG.Infrastructure.Service.UID.PublicServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Bk.APG.Infrastructure.Service.UID.Service;

public class UidService : IUidService, IHealthCheck
{
    private readonly IPublicServices _publicService;
    private readonly IMasterDataService _masterDataService;
    private readonly UidConfiguration _uidConfiguration;

    // according to decision of FCh we are limiting the displayed legal forms. This list reflects the masterdata of legalforms (https://bkdev.atlassian.net/browse/BKDO-1518)
    private readonly List<string> _allowedLegalForms = new() { "0103", "0104", "0106", "0107", "0108", "0109", "0110", "0117", "0224", "0234", "0302", "0571" };
    // this information reveals, if the entry is currently active (Item3) or in mutation (Item4).
    private readonly List<uidregStatusEnterpriseDetailType> _allowedStates = new() { uidregStatusEnterpriseDetailType.Item3, uidregStatusEnterpriseDetailType.Item4 };

    public UidService(IPublicServices publicService, IMasterDataService masterDataService, IOptions<UidConfiguration> uidConfiguration)
    {
        ArgumentNullException.ThrowIfNull(uidConfiguration);

        _publicService = publicService;
        _masterDataService = masterDataService;
        _uidConfiguration = uidConfiguration.Value;
    }

    public async Task<IEnumerable<UidDto>> Search(string organizationName)
    {
        var minimalMatchQuality = _uidConfiguration.MinimalMatchQuality;

        var request = new uidEntityPublicSearchRequest
        {
            Item = new uidEntityPublicSearchParameters
            {
                organisationName = organizationName,
            }
        };

        var config = new searchConfiguration
        {
            searchMode = searchMode.Auto,
            maxNumberOfRecords = 100,
            searchNameAndAddressHistory = false,
        };

        var result = await _publicService.SearchAsync(request, config);

        if (result is { uidEntitySearchResultItem: not null })
        {
            var filteredUidResult = result.uidEntitySearchResultItem
                .Where(uid => uid.rating > minimalMatchQuality && _allowedLegalForms.Contains(uid.organisation.organisation.organisationIdentification.legalForm) &&
                    _allowedStates.Contains(uid.organisation.uidregInformation.uidregStatusEnterpriseDetail))
                .Select(uid => new UidDto
                {
                    OrganizationName = uid.organisation.organisation.organisationIdentification.organisationName,
                    LegalFormId = _masterDataService.GetLegalFormGuidByLegalFormId(uid.organisation.organisation.organisationIdentification.legalForm),
                    LegalFormText = _masterDataService.GetLegalFormTextByLegalFormId(uid.organisation.organisation.organisationIdentification.legalForm),
                    Zip = uid.organisation.organisation.address[0]?.Items[0]?.ToString(),
                    City = uid.organisation.organisation.address[0]?.town,
                    UidOrganisationId = uid.organisation.organisation.organisationIdentification?.uid.uidOrganisationId,
                    MatchQuality = uid.rating,
                }
                ).OrderByDescending(uid => uid.MatchQuality).ToList();

            return filteredUidResult;
        }

        return new List<UidDto>();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        // As there is no healthcheck, we start a search with Bundeskanzlei, which should deliver 2 hits
        try
        {
            var request = new uidEntityPublicSearchRequest
            {
                Item = new uidEntityPublicSearchParameters
                {
                    organisationName = "Bundeskanzlei"
                }
            };

            var config = new searchConfiguration
            {
                searchMode = searchMode.Auto,
                maxNumberOfRecords = 1,
                searchNameAndAddressHistory = false
            };

            var result = await _publicService.SearchAsync(request, config);

            if (result != null && result.uidEntitySearchResultItem is not null)
            {
                return HealthCheckResult.Healthy("UID connection is healthy");
            }

            return HealthCheckResult.Unhealthy("Unable to connect to UID service");
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy("An exception occurred while checking UID connection", e);
        }
    }
}
