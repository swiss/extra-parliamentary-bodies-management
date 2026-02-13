using Bk.APG.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bk.APG.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly IMasterDataService _masterDataService;
    private readonly ICantonService _cantonService;

    public MasterDataController(IMasterDataService masterDataService, ICantonService cantonService)
    {
        _masterDataService = masterDataService;
        _cantonService = cantonService;
    }

    [HttpGet]
    public async Task<ActionResult> GetMasterData()
    {
        var masterData = new
        {
            Languages = await _masterDataService.GetLanguages(),
            Cantons = await _cantonService.GetAll(),
            Genders = await _masterDataService.GetGenders(),
            Salutations = await _masterDataService.GetSalutations(),
            InterestCommittees = await _masterDataService.GetInterestCommittees(),
            InterestFunctions = await _masterDataService.GetInterestFunctions(),
            InterestLegalForms = await _masterDataService.GetInterestLegalForms(),
            LegalForms = await _masterDataService.GetLegalForms(),
            Levels = await _masterDataService.GetLevels(),
            Departments = await _masterDataService.GetDepartments(),
            PermittedDepartments = await _masterDataService.GetPermittedDepartments(),
            Offices = await _masterDataService.GetOffices(),
            PermittedOffices = await _masterDataService.GetPermittedOffices(),
            CommitteeTypes = await _masterDataService.GetCommitteeTypes(),
            Terms = await _masterDataService.GetTerms(),
            TermDates = await _masterDataService.GetTermDates(),
            ElectionTypes = await _masterDataService.GetElectionTypes(),
            ElectionOffices = await _masterDataService.GetElectionOffices(),
            Functions = await _masterDataService.GetFunctions(),
            MembershipAdditions = await _masterDataService.GetMembershipAdditions(),
            AppointmentDecisionLinkTypes = await _masterDataService.GetAppointmentDecisionLinkTypes(),
            AppointmentDecisionTypes = await _masterDataService.GetAppointmentDecisionTypes(),
            LegislaturePeriods = await _masterDataService.GetLegislaturePeriods(),
            Councils = await _masterDataService.GetCouncils(),
            WorklistTaskTypes = await _masterDataService.GetWorklistTaskTypes(),
            WorklistTaskStates = await _masterDataService.GetWorklistTaskStates(),
            FormLetterSenderFunctions = await _masterDataService.GetFormLetterSenderFunctions()
        };
        return Ok(masterData);
    }

    [HttpGet("offices/search")]
    public async Task<ActionResult> GetOfficesByName([FromQuery] string officeName)
    {
        var offices = await _masterDataService.GetOfficesByName(officeName);
        return Ok(offices);
    }

    [HttpGet("occupations/search")]
    public async Task<ActionResult> GetOccupationsByName([FromQuery] string occupation)
    {
        var occupations = await _masterDataService.GetOccupationsByName(occupation);
        return Ok(occupations);
    }
}
