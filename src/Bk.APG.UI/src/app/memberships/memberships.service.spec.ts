import {HttpClient} from '@angular/common/http';
import {of} from 'rxjs';
import {MembershipsService} from './memberships.service';

describe('MembershipsService', () => {
    let service: MembershipsService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        post: jest.fn(() => of()),
        put: jest.fn(() => of()),
        delete: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new MembershipsService(httpClientMock as unknown as HttpClient);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should update membership', () => {
        service.updateMembership({
            id: '1',
            personId: '100',
            committeeId: '100',
            maximumEmploymentLevel: 10,
            beginDate: new Date(2024, 1, 1),
            endDate: new Date(2024, 2, 1),
            electionTypeId: 'electionTypeId',
            functionId: 'functionId',
            electionOfficeId: 'electionOfficeId',
            functionName: '',
            electionOfficeName: '',
            inCorrelationWithFederalDuty: true,
            rowVersion: 123,
            canEdit: true,
            canEditBeginDate: true,
            canDelete: true,
        });

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/memberships/1', {
            id: '1',
            personId: '100',
            committeeId: '100',
            maximumEmploymentLevel: 10,
            beginDate: new Date(2024, 1, 1),
            endDate: new Date(2024, 2, 1),
            electionTypeId: 'electionTypeId',
            functionId: 'functionId',
            electionOfficeId: 'electionOfficeId',
            functionName: '',
            electionOfficeName: '',
            inCorrelationWithFederalDuty: true,
            rowVersion: 123,
            canEdit: true,
            canEditBeginDate: true,
            canDelete: true,
        });
    });

    it('should delete membership', () => {
        service.deleteMembership('1');

        expect(httpClientMock.delete).toHaveBeenCalledWith('/api/memberships/1');
    });
});
