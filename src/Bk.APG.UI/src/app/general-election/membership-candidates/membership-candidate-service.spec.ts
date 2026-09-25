import {HttpClient} from '@angular/common/http';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {of} from 'rxjs';
import {MembershipCandidateService} from './membership-candidate-service';

describe('MembershipCandidateService', () => {
    let service: MembershipCandidateService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        put: jest.fn(() => of()),
        post: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new MembershipCandidateService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get membership candidate for update', () => {
        service.getMembershipCandidateForUpdate('1');

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/general-election/membership-candidates/1/update');
    });

    it('should update membership candidate', () => {
        const membershipCandidateUpdate = {id: '1'} as MembershipCandidateUpdate;

        service.updateMembershipCandidate(membershipCandidateUpdate);

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/general-election/membership-candidates/1', membershipCandidateUpdate);
    });

    it('should calculate membership candidate term', () => {
        service.calculateMembershipCandidateTerm('1', new Date(2023, 0, 1), new Date(2024, 11, 31));

        expect(httpClientMock.post).toHaveBeenCalledWith('/api/general-election/membership-candidates/1/calculate-term', {
            beginDate: '2023-01-01',
            endDate: '2024-12-31',
        });
    });
});
