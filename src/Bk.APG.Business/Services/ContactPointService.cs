using Bk.APG.Business.Dtos;
using Bk.APG.Business.Mapper;
using Bk.APG.Business.Models;
using Bk.APG.Business.Repositories;
using Bk.APG.CrossCutting.Exception;
using Microsoft.Extensions.Logging;

namespace Bk.APG.Business.Services;

public class ContactPointService : IContactPointService
{
    private readonly IContactPointRepository _contactPointRepository;
    private readonly ICommitteeRepository _committeeRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IMasterDataService _masterDataService;
    private readonly ILogger<ContactPointService> _logger;

    public ContactPointService(
        IContactPointRepository contactPointRepository, ICommitteeRepository committeeRepository, IAuthorizationService authorizationService, IMasterDataService masterDataService, ILogger<ContactPointService> logger)
    {
        _contactPointRepository = contactPointRepository;
        _committeeRepository = committeeRepository;
        _authorizationService = authorizationService;
        _masterDataService = masterDataService;
        _logger = logger;
    }

    public async Task<IEnumerable<ContactPointListDto>> GetContactPointListByCommitteeId(Guid committeeId)
    {
        var committee = await _committeeRepository.GetById(committeeId);

        var contactPoints = await _contactPointRepository.GetContactPointsByCommitteeId(committee.Id);

        var contactPointList = contactPoints.Select(cp => ContactPointMapper.ToContactPointListDto(cp)).ToList().OrderByDescending(cp => cp.BeginDate);

        return contactPointList;
    }

    public async Task<ContactPointDetailDto> GetContactPointDetail(Guid id)
    {
        var contactPoint = await _contactPointRepository.GetById(id);
        return ContactPointMapper.ToContactPointDetailDto(contactPoint);
    }

    public async Task<ContactPointUpdateDto> GetByIdForUpdate(Guid id)
    {
        var contactPoint = await _contactPointRepository.GetByIdForUpdate(id);
        return ContactPointMapper.ToContactPointUpdateDto(contactPoint);
    }

    public async Task<ContactPointCreateDto> GetEmpty(Guid committeeId)
    {
        var committee = await _committeeRepository.GetById(committeeId);

        return new ContactPointCreateDto
        {
            CommitteeId = committee.Id,
            ContactPointTypeId = Guid.Empty,
            ContactPointTypeUri = string.Empty,
            Zip = string.Empty,
            City = string.Empty,
            CommitteeBeginDate = committee.BeginDate
        };
    }

    public async Task<ContactPointDetailDto> Create(ContactPointCreateDto createDto)
    {
        createDto.ContactPointTypeId = await _masterDataService.GetContactPointGuidFromContactPointUri(createDto.ContactPointTypeUri);

        var mappedContactPoint = ContactPointMapper.FromContactPointCreateDto(createDto);

        if (await CheckForDuplicate(mappedContactPoint))
        {
            throw new BusinessValidationException("Record already exists");
        }

        var userName = _authorizationService.GetCurrentUserName();

        mappedContactPoint.CreatedBy = userName;
        mappedContactPoint.Created = DateTime.UtcNow;
        mappedContactPoint.ModifiedBy = userName;
        mappedContactPoint.Modified = DateTime.UtcNow;

        await _contactPointRepository.Create(mappedContactPoint);
        _logger.LogInformation("Created contact point {ContactPointId}", mappedContactPoint.Id);

        return await GetContactPointDetail(mappedContactPoint.Id);
    }

    public async Task Update(Guid id, ContactPointUpdateDto updateDto)
    {
        _logger.LogInformation("Update contact point {ContactPointId}", id);

        updateDto.ContactPointTypeId = await _masterDataService.GetContactPointGuidFromContactPointUri(updateDto.ContactPointTypeUri);

        if (updateDto.IsCopy)
        {
            var mappedContactPointCreate = ContactPointMapper.FromContactPointUpdateToCreateDto(updateDto);

            await Create(mappedContactPointCreate);
        }
        else
        {
            var contactPoint = ContactPointMapper.FromContactPointUpdateDto(updateDto);

            if (await CheckForDuplicate(contactPoint))
            {
                throw new BusinessValidationException("Record already exists");
            }

            var existingContactPoint = await _contactPointRepository.GetByIdForUpdate(id, updateDto.RowVersion);

            existingContactPoint.CommitteeId = updateDto.CommitteeId;
            existingContactPoint.ContactPointTypeId = updateDto.ContactPointTypeId;
            existingContactPoint.CompanyName = updateDto.CompanyName;
            existingContactPoint.Section = updateDto.Section;
            existingContactPoint.BeginDate = updateDto.BeginDate;
            existingContactPoint.EndDate = updateDto.EndDate;
            existingContactPoint.Street = updateDto.Street;
            existingContactPoint.PoBox = updateDto.PoBox;
            existingContactPoint.Zip = updateDto.Zip;
            existingContactPoint.City = updateDto.City;
            existingContactPoint.Phone = updateDto.Phone;
            existingContactPoint.Email = updateDto.Email;
            existingContactPoint.Surname = updateDto.Surname;
            existingContactPoint.GivenName = updateDto.GivenName;
            existingContactPoint.Title = updateDto.Title;
            existingContactPoint.LanguageId = updateDto.LanguageId;
            existingContactPoint.GenderId = updateDto.GenderId;
            existingContactPoint.PersonalPhone = updateDto.PersonalPhone;
            existingContactPoint.PersonalMobile = updateDto.PersonalMobile;
            existingContactPoint.PersonalEmail = updateDto.PersonalEmail;
            existingContactPoint.ReleasePersonData = updateDto.ReleasePersonData;
            existingContactPoint.OldId = updateDto.OldId;

            existingContactPoint.Modified = DateTime.UtcNow;
            existingContactPoint.ModifiedBy = _authorizationService.GetCurrentUserName();

            await _contactPointRepository.CommitChanges();

            _logger.LogInformation("Updated contact point {ContactPointId}", id);
        }
    }

    public async Task Delete(Guid id)
    {
        _logger.LogDebug("Delete contact point {ContactPointId}", id);

        var contactPoint = await _contactPointRepository.GetByIdForUpdate(id);

        if (!await _authorizationService.HasAccessToCommittee(contactPoint.Committee!))
        {
            _logger.LogError("User is not allowed to delete contact point {ContactPointId} for committee {CommitteeId}", id, contactPoint.CommitteeId);
            throw new AuthorizationException($"User is not allowed to delete contact point with id: {id}");
        }

        _contactPointRepository.Delete(contactPoint);

        await _contactPointRepository.CommitChanges();

        _logger.LogInformation("Deleted contact point {ContactPointId}", id);
    }

    private async Task<bool> CheckForDuplicate(ContactPoint contactPoint)
    {
        var contactPoints = await _contactPointRepository.GetContactPointsByCommitteeId(contactPoint.CommitteeId);

        // duplicates have to be not me, but the same type as mine.
        contactPoints = contactPoints.Where(cp => cp.Id != contactPoint.Id && cp.ContactPointTypeId == contactPoint.ContactPointTypeId);

        if (contactPoint.CompanyName != null)
        {
            contactPoints = contactPoints.Where(cp => cp.CompanyName == contactPoint.CompanyName);
        }

        if (!string.IsNullOrWhiteSpace(contactPoint.Surname) && !string.IsNullOrWhiteSpace(contactPoint.GivenName))
        {
            contactPoints = contactPoints.Where(cp => cp.Surname == contactPoint.Surname && cp.GivenName == contactPoint.GivenName);
        }

        return contactPoints.Any(cp => (cp.EndDate is null && contactPoint.EndDate is null)
                                       || (cp.EndDate is null && cp.BeginDate < contactPoint.EndDate)
                                       || (cp.BeginDate >= contactPoint.BeginDate && cp.BeginDate < contactPoint.EndDate && (cp.EndDate <= contactPoint.EndDate || cp.EndDate > contactPoint.EndDate))
                                       || (cp.BeginDate < contactPoint.BeginDate && cp.EndDate > contactPoint.BeginDate));
    }
}
