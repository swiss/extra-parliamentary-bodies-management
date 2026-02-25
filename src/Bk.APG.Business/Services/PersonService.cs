using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.Common.Utilities;
using Bk.APG.CrossCutting;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IGeneralElectionService _generalElectionService;
    private readonly ITermOfOfficeDateService _termOfOfficeDateService;
    private readonly ISalutationGeneratorService _salutationGeneratorService;
    private readonly IMasterDataRepository _masterDataRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMembershipCandidateRepository _membershipCandidateRepository;
    private readonly IInterestRepository _interestRepository;
    private readonly IMembershipCandidateLogMessageRepository _membershipCandidateLogMessageRepository;
    private readonly IWorklistTaskRepository _worklistTaskRepository;
    private readonly ILogger<PersonService> _logger;

    public PersonService(
        IAuthorizationService authorizationService,
        IGeneralElectionService generalElectionService,
        ITermOfOfficeDateService termOfOfficeDateService,
        ISalutationGeneratorService salutationGeneratorService,
        IMasterDataRepository masterDataRepository,
        IPersonRepository personRepository,
        IMembershipRepository membershipRepository,
        IMembershipCandidateRepository membershipCandidateRepository,
        IInterestRepository interestRepository,
        IMembershipCandidateLogMessageRepository membershipCandidateLogMessageRepository,
        IWorklistTaskRepository worklistTaskRepository,
        ILogger<PersonService> logger)
    {
        _authorizationService = authorizationService;
        _generalElectionService = generalElectionService;
        _termOfOfficeDateService = termOfOfficeDateService;
        _salutationGeneratorService = salutationGeneratorService;
        _masterDataRepository = masterDataRepository;
        _personRepository = personRepository;
        _membershipRepository = membershipRepository;
        _membershipCandidateRepository = membershipCandidateRepository;
        _interestRepository = interestRepository;
        _membershipCandidateLogMessageRepository = membershipCandidateLogMessageRepository;
        _worklistTaskRepository = worklistTaskRepository;
        _logger = logger;
    }

    public async Task<PagedResultDto<PersonListDto>> GetPersonList(PagingParametersDto paging, PersonFilterParametersDto? filterParametersDto, string? sort, SortDirection? sortDirection)
    {
        if (filterParametersDto is null ||
            (string.IsNullOrWhiteSpace(filterParametersDto.FreeText) && filterParametersDto.CantonIds is null && filterParametersDto.LanguageIds is null && filterParametersDto.HasActiveMembership is null))
        {
            return new PagedResultDto<PersonListDto>();
        }

        var filter = PersonMapper.ToPersonFilterParameters(filterParametersDto);
        var pagingParameters = PagingMapper.ToPagingParameters(paging);
        var persons = await _personRepository.GetAll(pagingParameters, filter, sort, sortDirection);
        return new PagedResultDto<PersonListDto>
        {
            Index = persons.Index,
            Total = persons.Total,
            Items = persons.Items.Select(PersonMapper.ToPersonListDto)
        };
    }

    public async Task<PersonDetailDto> GetPersonDetail(Guid id)
    {
        var person = await _personRepository.GetById(id);

        return ToPersonDetailDto(person);
    }

    public PersonCreateDto GetEmpty()
    {
        return new PersonCreateDto
        {
            Surname = string.Empty,
            GivenName = string.Empty
        };
    }

    public async Task<PersonUpdateDto> GetPersonForUpdate(Guid id)
    {
        var person = await _personRepository.GetByIdForUpdate(id);

        var membershipCandidates = await _membershipCandidateRepository.GetByPersonIdForUpdate(id);
        var canDelete = await CanCurrentUserDeletePerson(person, membershipCandidates);

        return PersonMapper.ToPersonUpdateDto(person, ShouldMaskAddress(person), canDelete);
    }

    public async Task<PersonDetailDto> CreatePerson(PersonCreateDto createDto)
    {
        _logger.LogInformation("Create person");

        var mappedPerson = PersonMapper.FromPersonCreateDto(createDto);
        var currentUserName = _authorizationService.GetCurrentUserName();

        mappedPerson.LegislaturePeriods = (await _masterDataRepository.GetLegislaturePeriodsByIds(createDto.LegislaturePeriodIds)).ToList();
        mappedPerson.Occupations = await _masterDataRepository.GetOccupationsByIds(createDto.Occupations.Select(o => o.Id).ToList());

        mappedPerson.CreatedBy = currentUserName;
        mappedPerson.Created = DateTime.UtcNow;
        mappedPerson.ModifiedBy = currentUserName;
        mappedPerson.Modified = DateTime.UtcNow;

        if (mappedPerson.OfficeAddress is not null)
        {
            mappedPerson.OfficeAddress.CreatedBy = currentUserName;
            mappedPerson.OfficeAddress.Created = DateTime.UtcNow;
            mappedPerson.OfficeAddress.ModifiedBy = currentUserName;
            mappedPerson.OfficeAddress.Modified = DateTime.UtcNow;
        }

        if (mappedPerson.PrivateAddress is not null)
        {
            mappedPerson.PrivateAddress.CreatedBy = currentUserName;
            mappedPerson.PrivateAddress.Created = DateTime.UtcNow;
            mappedPerson.PrivateAddress.ModifiedBy = currentUserName;
            mappedPerson.PrivateAddress.Modified = DateTime.UtcNow;
        }

        var newEntry = await _personRepository.Create(mappedPerson);
        _logger.LogInformation("Person created with id {PersonId}", newEntry.Id);

        var savedPerson = await _personRepository.GetById(newEntry.Id);

        return ToPersonDetailDto(savedPerson);
    }

    public async Task<Person> CreatePersonInGeneralElection(MembershipCandidate membershipCandidate)
    {
        _logger.LogInformation("Create person for membership candidate {MembershipCandidateId}", membershipCandidate.Id);

        var mappedPerson = PersonMapper.FromMembershipCandidate(membershipCandidate);
        var currentUserName = _authorizationService.GetCurrentUserName();

        mappedPerson.SalutationText = await _salutationGeneratorService.CreateSalutationTextForPerson(mappedPerson.GenderId, mappedPerson.CorrespondenceLanguageId, mappedPerson.Surname, mappedPerson.Title);

        mappedPerson.CreatedBy = currentUserName;
        mappedPerson.Created = DateTime.UtcNow;
        mappedPerson.ModifiedBy = currentUserName;
        mappedPerson.Modified = DateTime.UtcNow;

        var newEntry = await _personRepository.Create(mappedPerson);
        _logger.LogInformation("Created person {PersonId} from membership candidate {MembershipCandidateId}", newEntry.Id, membershipCandidate.Id);

        return await _personRepository.GetById(newEntry.Id);
    }

    public async Task<PersonUpdateDto> UpdatePerson(Guid id, PersonUpdateDto updateDto)
    {
        _logger.LogInformation("Update person {PersonId}", id);

        var existingEntry = await _personRepository.GetByIdForUpdate(id, updateDto.RowVersion);
        var currentUserName = _authorizationService.GetCurrentUserName();

        var mappedOccupations = await _masterDataRepository.GetOccupationsByIds(updateDto.Occupations.Select(o => o.Id).ToList());

        var isGeneralElectionRunning = await _termOfOfficeDateService.CheckForRunningGeneralElection();

        // if a person has no active membership or membership is for general election (state to be defined), it can be modified by any permitted roles. After that, only
        // Admin users are permitted to do so.
        if (existingEntry.Memberships.Any(m => m.IsActive) &&
            (existingEntry.GenderId != updateDto.GenderId || existingEntry.LanguageId != updateDto.LanguageId) && !_authorizationService.IsAdmin)
        {
            throw new AuthorizationException("Not permitted to update Gender or Language with this role");
        }

        existingEntry.GenderId = updateDto.GenderId;
        existingEntry.LanguageId = updateDto.LanguageId;

        existingEntry.GivenName = updateDto.GivenName;
        existingEntry.Surname = updateDto.Surname;
        existingEntry.BirthYear = updateDto.BirthYear;
        existingEntry.FederalAssembly = updateDto.FederalAssembly;
        existingEntry.FederalDuty = updateDto.FederalDuty;
        existingEntry.Occupation = updateDto.Occupation;
        existingEntry.Employer = updateDto.Employer;
        existingEntry.NoEmployment = updateDto.NoEmployment;
        existingEntry.Title = updateDto.Title;
        existingEntry.NoInterest = updateDto.NoInterest;
        existingEntry.SalutationText = updateDto.SalutationText;

        existingEntry.CorrespondenceLanguageId = updateDto.CorrespondenceLanguageId;
        existingEntry.SalutationId = updateDto.SalutationId;
        existingEntry.CouncilId = updateDto.CouncilId;
        existingEntry.OfficeId = updateDto.OfficeId;

        existingEntry.LegislaturePeriods = updateDto.FederalAssembly ? (await _masterDataRepository.GetLegislaturePeriodsByIds(updateDto.LegislaturePeriodIds)).ToList() : [];
        existingEntry.Occupations = mappedOccupations;

        existingEntry.ModifiedBy = currentUserName;
        existingEntry.Modified = DateTime.UtcNow;

        if (!ShouldMaskAddress(existingEntry))
        {
            CreateOrUpdateAddresses(existingEntry, updateDto, currentUserName);
        }

        await _personRepository.CommitChanges();

        _logger.LogInformation("Updated person {PersonId}", existingEntry.Id);

        if (isGeneralElectionRunning)
        {
            // load all candidates with this person id
            await _generalElectionService.UpdateCandidatesFromPerson(existingEntry);

            await CheckAndCompletePersonTasks(existingEntry);
        }

        var canDelete = await CanCurrentUserDeletePerson(existingEntry);

        return PersonMapper.ToPersonUpdateDto(existingEntry, ShouldMaskAddress(existingEntry), canDelete);
    }

    private async Task CheckAndCompletePersonTasks(Person person)
    {
        var personTasks = await _worklistTaskRepository.GetAllByPersonId(person.Id);

        var baseDataTask = personTasks.FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonBaseData);
        if (baseDataTask is not null && baseDataTask.WorklistTaskStateId != WorklistTaskState.Completed && person is { NeedsAttentionBasicData: false, NeedsAttentionOccupation: false })
        {
            baseDataTask.WorklistTaskStateId = WorklistTaskState.Completed;
            baseDataTask.ModifiedBy = "System";
            baseDataTask.Modified = DateTime.UtcNow;
        }

        var interestsTask = personTasks.FirstOrDefault(x => x.WorklistTaskTypeId == WorklistTaskType.GeneralElectionPersonInterests);
        if (interestsTask is not null && interestsTask.WorklistTaskStateId != WorklistTaskState.Completed && person is { NeedsAttentionInterests: false })
        {
            interestsTask.WorklistTaskStateId = WorklistTaskState.Completed;
            interestsTask.ModifiedBy = "System";
            interestsTask.Modified = DateTime.UtcNow;
        }

        await _worklistTaskRepository.CommitChanges();
    }

    public async Task DeletePerson(Guid id)
    {
        _logger.LogDebug("Delete person {PersonId}", id);

        var person = await _personRepository.GetByIdForUpdate(id);
        var membershipCandidates = (await _membershipCandidateRepository.GetByPersonIdForUpdate(id)).ToList();

        if (!await CanCurrentUserDeletePerson(person, membershipCandidates))
        {
            throw new AuthorizationException($"Not permitted to delete person id {id} with this role");
        }

        var memberships = person.Memberships.ToList();
        var membershipIds = memberships.Select(m => m.Id).ToList();
        var membershipCandidateIds = membershipCandidates.Select(mc => mc.Id).ToList();

        var worklistTasks = await _worklistTaskRepository.GetByPersonOrMemberships(id, membershipIds, membershipCandidateIds);
        _worklistTaskRepository.DeleteRange(worklistTasks);

        var logMessages = await _membershipCandidateLogMessageRepository.GetByPersonId(id);
        _membershipCandidateLogMessageRepository.DeleteRange(logMessages);

        _membershipCandidateRepository.DeleteRange(membershipCandidates);
        _membershipRepository.DeleteRange(memberships);

        var interests = await _interestRepository.GetAllByPersonIdForUpdate(id);
        _interestRepository.DeleteRange(interests);

        _personRepository.Delete(person);

        await _personRepository.CommitChanges();

        _logger.LogInformation("Deleted person {PersonId}", id);
    }

    private void CreateOrUpdateAddresses(Person existingEntry, PersonUpdateDto update, string currentUserName)
    {
        if (update.OfficeAddress is not null)
        {
            if (existingEntry.OfficeAddress is null)
            {
                existingEntry.OfficeAddress = CreateAddress(update.OfficeAddress, currentUserName);

                _logger.LogInformation("Created office address");
            }
            else
            {
                UpdateAddress(existingEntry.OfficeAddress, update.OfficeAddress, currentUserName);

                _logger.LogInformation("Updated office address {AddressId}'", existingEntry.OfficeAddress.Id);
            }

            if (update.OfficeAddress.ActiveAddress && existingEntry.CorrespondenceAddressId != existingEntry.OfficeAddressId)
            {
                existingEntry.CorrespondenceAddress = existingEntry.OfficeAddress;
            }
        }
        else
        {
            existingEntry.OfficeAddress = null;
        }

        if (update.PrivateAddress is not null)
        {
            if (existingEntry.PrivateAddress is null)
            {
                existingEntry.PrivateAddress = CreateAddress(update.PrivateAddress, currentUserName);

                _logger.LogInformation("Created private address");
            }
            else
            {
                UpdateAddress(existingEntry.PrivateAddress, update.PrivateAddress, currentUserName);

                _logger.LogInformation("Updated private address {AddressId}", existingEntry.PrivateAddress.Id);
            }

            if (update.PrivateAddress.ActiveAddress && existingEntry.CorrespondenceAddressId != existingEntry.PrivateAddressId)
            {
                existingEntry.CorrespondenceAddress = existingEntry.PrivateAddress;
            }
        }
        else
        {
            existingEntry.PrivateAddress = null;
        }
    }

    private static Address CreateAddress(AddressUpdateDto update, string currentUserName)
    {
        return new Address
        {
            CompanyName = update.CompanyName,
            Street = update.Street,
            PoBox = update.PoBox,
            CountryId = update.CountryId,
            Zip = update.Zip,
            City = update.City,
            CantonId = update.CantonId,
            Phone = update.Phone,
            Mobile = update.Mobile,
            Email = update.Email,
            Created = DateTime.UtcNow,
            CreatedBy = currentUserName,
            Modified = DateTime.UtcNow,
            ModifiedBy = currentUserName
        };
    }

    private static void UpdateAddress(Address address, AddressUpdateDto update, string currentUserName)
    {
        address.CompanyName = update.CompanyName;
        address.Street = update.Street;
        address.PoBox = update.PoBox;
        address.CountryId = update.CountryId;
        address.Zip = update.Zip;
        address.City = update.City;
        address.CantonId = update.CantonId;
        address.Phone = update.Phone;
        address.Mobile = update.Mobile;
        address.Email = update.Email;
        address.Modified = DateTime.UtcNow;
        address.ModifiedBy = currentUserName;
    }

    public async Task<IEnumerable<PersonDetailDto>?> GetSimilarPersons(string surname, string givenName, int birthYear, int birthYearRange)
    {
        var personsInBirthYearRange = (await _personRepository.GetPersonsByBirthYear(birthYear, birthYearRange)).ToList();
        var surnameReduced = SimilaritySearchTransformer.Reduce(surname);
        var givenNameReduced = SimilaritySearchTransformer.Reduce(givenName);

        if (birthYear.ToString()[2] != birthYear.ToString()[3])
        {
            var birthYearWithLastDigitsExchanged = birthYear.ToString().Substring(0, 2) + birthYear.ToString()[3] + birthYear.ToString()[2];
            var personsWithSimilarBirthYear = await _personRepository.GetPersonsByBirthYear(Convert.ToInt32(birthYearWithLastDigitsExchanged), 0);

            personsInBirthYearRange.AddRange(personsWithSimilarBirthYear.Where(pNew => !personsInBirthYearRange.Select(p => p.Id).Contains(pNew.Id)));
        }

        var similarPersons = new List<PersonDetailDto>();

        foreach (var person in personsInBirthYearRange)
        {
            var personSurnameReduced = SimilaritySearchTransformer.Reduce(person.Surname);
            var personGivenNameReduced = SimilaritySearchTransformer.Reduce(person.GivenName);

            if ((personSurnameReduced == surnameReduced && personGivenNameReduced == givenNameReduced) ||
                (personGivenNameReduced == surnameReduced && personSurnameReduced == givenNameReduced))
            {
                similarPersons.Add(PersonMapper.ToPersonDetailDto(person, false));
            }
        }

        return similarPersons;
    }

    public async Task<CandidateListDuplicateCheckResultDto> GetDuplicatePersonForGeneralElection(MembershipCandidate membershipCandidate)
    {
        const int birthYearRange = 5;

        var similarPersons = await GetSimilarPersons(membershipCandidate.Surname, membershipCandidate.GivenName, membershipCandidate.BirthYear, birthYearRange);

        var givenNameReduced = SimilaritySearchTransformer.Reduce(membershipCandidate.GivenName);

        var dto = new CandidateListDuplicateCheckResultDto()
        {
            MembershipCandidateToCheck = MembershipCandidateMapper.ToMembershipCandidateDetailDto(membershipCandidate),
            DuplicateReason = DuplicateReason.NoDuplicateFound
        };

        if (similarPersons is null)
        {
            return dto;
        }

        foreach (var person in similarPersons)
        {
            if (NamesAreEqual(person.Surname, membershipCandidate.Surname) && NamesAreEqual(person.GivenName, membershipCandidate.GivenName) &&
                person.BirthYear == membershipCandidate.BirthYear && person.GenderId == membershipCandidate.GenderId &&
                person.LanguageId == membershipCandidate.LanguageId)
            {
                dto.DuplicateReason = DuplicateReason.FullMatch;
            }
            else if (NamesAreEqual(person.Surname, membershipCandidate.Surname) && NamesAreEqual(person.GivenName, membershipCandidate.GivenName) &&
                     person.BirthYear == membershipCandidate.BirthYear && person.GenderId == membershipCandidate.GenderId &&
                     person.LanguageId != membershipCandidate.LanguageId)
            {
                dto.DuplicateReason = DuplicateReason.LanguageMismatch;
            }
            else
            {
                if (person.BirthYear == Convert.ToInt32(membershipCandidate.BirthYear.ToString()[..2] + membershipCandidate.BirthYear.ToString()[3] + membershipCandidate.BirthYear.ToString()[2]) ||
                    (NamesAreEqual(person.Surname, membershipCandidate.Surname) && NamesAreEqual(person.GivenName, membershipCandidate.GivenName) &&
                        person.GenderId == membershipCandidate.GenderId && Math.Abs(person.BirthYear - membershipCandidate.BirthYear) <= birthYearRange))
                {
                    dto.DuplicateReason = DuplicateReason.InBirthYearRange;
                }
                else
                {
                    var personGivenNameReduced = SimilaritySearchTransformer.Reduce(person.GivenName);

                    if (NamesAreEqual(person.Surname, membershipCandidate.Surname) &&
                        person.GenderId == membershipCandidate.GenderId && person.BirthYear == membershipCandidate.BirthYear && personGivenNameReduced == givenNameReduced)
                    {
                        dto.DuplicateReason = DuplicateReason.GivenNameMismatch;
                    }
                }
            }

            if (dto.DuplicateReason != DuplicateReason.NoDuplicateFound)
            {
                dto.PersonFound = person;
                break;
            }
        }

        return dto;
    }

    public async Task<IEnumerable<PersonDetailDto>?> GetByName(string name)
    {
        return (await _personRepository.GetByName(name)).Select(ToPersonDetailDto);
    }

    private PersonDetailDto ToPersonDetailDto(Person person)
    {
        var maskAddress = ShouldMaskAddress(person);

        return PersonMapper.ToPersonDetailDto(person, maskAddress);
    }

    internal static bool NamesAreEqual(string a, string b)
    {
        return string.Equals(
            a?.Trim(),
            b?.Trim(),
            StringComparison.CurrentCultureIgnoreCase);
    }

    private async Task<bool> CanCurrentUserDeletePerson(Person person, IEnumerable<MembershipCandidate>? membershipCandidates = null)
    {
        if (_authorizationService.IsAdmin)
        {
            return true;
        }

        var membershipCandidatesList = membershipCandidates?.ToList() ?? (await _membershipCandidateRepository.GetByPersonIdForUpdate(person.Id)).ToList();
        var hasMemberships = person.Memberships.Count != 0;
        var hasMembershipCandidates = membershipCandidatesList.Count != 0;

        var hasDeleteRights = _authorizationService.IsDepartment || _authorizationService.IsOffice || _authorizationService.IsSecretariat;

        return hasDeleteRights && !hasMemberships && !hasMembershipCandidates;
    }

    public bool ShouldMaskAddress(Person person)
    {
        if (_authorizationService.IsAdmin)
        {
            return false;
        }

        // Empty addresses should be editable
        if (person.CorrespondenceAddress is null)
        {
            return false;
        }

        if (person.Memberships.Count == 0)
        {
            return false;
        }

        var lastMembership = person.Memberships.OrderByDescending(m => m.EndDate).First();
        return lastMembership.EndDate < DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
    }
}
