import {HttpClient, HttpHeaders} from '@angular/common/http';
import {CandidateListForward} from '@api/CandidateListForward';
import {MembershipCandidateCreate} from '@api/MembershipCandidateCreate';
import {of} from 'rxjs';
import {GeneralElectionCommitteeCandidateListService} from './ge-committee-candidate-list.service';

describe('GeneralElectionCommitteeCandidateListService', () => {
    let service: GeneralElectionCommitteeCandidateListService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        put: jest.fn(() => of()),
        patch: jest.fn(() => of()),
        post: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new GeneralElectionCommitteeCandidateListService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should partial update membership candidate', async () => {
        service.partialUpdateMembershipCandidate('Foo', {functionId: 'Bar'});

        expect(httpClientMock.patch).toHaveBeenCalledWith('/api/general-election/membership-candidates/Foo', {functionId: 'Bar'});
    });
    describe('getAssignmentsForCandidateListForward', () => {
        it('should get assignments for candidate list forward', () => {
            service.getAssignmentsForCandidateListForward('Foo');

            expect(httpClientMock.get).toHaveBeenCalledWith('/api/general-election/committees/Foo/candidate-list/forward');
        });
    });

    describe('forwardCandidateList', () => {
        it('should forward candidate list', () => {
            const forwardData = {forwardToId: 'test'} as CandidateListForward;
            service.forwardCandidateList('Foo', forwardData);

            expect(httpClientMock.post).toHaveBeenCalledWith('/api/general-election/committees/Foo/candidate-list/forward', forwardData);
        });
    });

    describe('validateCandidateList', () => {
        it('should validate candidate list', () => {
            const selectedIds = ['id1', 'id2'];
            service.validateCandidateList('Foo', selectedIds, true);

            expect(httpClientMock.post).toHaveBeenCalledWith('/api/general-election/committees/Foo/candidate-list/validate', {
                selectedCandidateIds: ['id1', 'id2'],
                duplicateCheckConfirmed: true,
            });
        });
    });

    describe('saveCandidateList', () => {
        it('should save candidate list', () => {
            const selectedIds = ['id1', 'id2'];
            service.saveCandidateList('Foo', selectedIds);

            expect(httpClientMock.post).toHaveBeenCalledWith('/api/general-election/committees/Foo/candidate-list/save', ['id1', 'id2']);
        });
    });

    describe('getDuplicateMembershipCandidates', () => {
        it('should get similar membership candidate', () => {
            const createData = {
                committeeId: 'id',
                surname: 'surname',
                givenName: 'givenName',
                birthYear: 2000,
                genderId: 'genderId',
                languageId: 'languageId',
            } as MembershipCandidateCreate;
            service.getDuplicateMembershipCandidate(createData);

            expect(httpClientMock.post).toHaveBeenCalledWith('/api/general-election/committees/duplicate-membership-candidates', createData);
        });
    });

    describe('generateExport', () => {
        it('should call HttpClient.post with correct URL, body, headers, and options', () => {
            const committeeId = '1';
            const entryIds = ['11111111-1111-1111-1111-111111111111'];
            const body = {membershipCandidateIds: entryIds};
            const expectedUrl = `/api/general-election/committees/${committeeId}/download`;

            const expectedHeaders = new HttpHeaders().set('Accept', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');

            const expectedOptions = {
                headers: expectedHeaders,
                observe: 'response' as const,
                responseType: 'blob' as const,
            };

            const response = {body: new Blob()} as unknown;
            httpClientMock.post.mockReturnValue(of(response));

            service.generateExport(committeeId, entryIds).subscribe(res => {
                expect(res).toBe(response);
            });

            expect(httpClientMock.post).toHaveBeenCalledWith(expectedUrl, body, expectedOptions);
        });

        it('should set Accept header to Excel MIME type', () => {
            const committeeId = '1';
            const entryIds: string[] = [];
            httpClientMock.post.mockReturnValue(of({} as unknown));

            service.generateExport(committeeId, entryIds).subscribe();

            const callArgs = httpClientMock.post.mock.calls[0];
            const options = callArgs[2];
            const acceptHeader = (options!.headers as HttpHeaders).get('Accept');
            expect(acceptHeader).toBe('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
        });

        it('should set responseType to blob and observe to response', () => {
            const committeeId = '1';
            const entryIds: string[] = [];
            httpClientMock.post.mockReturnValue(of({} as unknown));

            service.generateExport(committeeId, entryIds).subscribe();

            const callArgs = httpClientMock.post.mock.calls[0];
            const options = callArgs[2];
            expect(options!.responseType).toBe('blob');
            expect(options!.observe).toBe('response');
        });
    });
});
