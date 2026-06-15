import {provideHttpClient} from '@angular/common/http';
import {HttpTestingController, provideHttpClientTesting} from '@angular/common/http/testing';
import {TestBed} from '@angular/core/testing';
import {InterestUpdate} from '@api/InterestUpdate';
import {ObHttpApiInterceptorEvents} from '@oblique/oblique';
import {PersonInterestsService} from './person-interests.service';

describe('PersonInterestsService', () => {
    let service: PersonInterestsService;
    let httpMock: HttpTestingController;
    const interceptorMock = {
        deactivateSpinnerOnNextAPICalls: jest.fn(),
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [provideHttpClient(), provideHttpClientTesting(), {provide: ObHttpApiInterceptorEvents, useValue: interceptorMock}],
        });
        service = TestBed.inject(PersonInterestsService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        jest.clearAllMocks();
        httpMock.verify();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get interests by personId', () => {
        let response;
        const id = 'myId';
        service.getInterestsByPersonId(id).subscribe(value => (response = value));

        const req = httpMock.expectOne(`/api/persons/myId/interests`);
        expect(req.request.method).toEqual('GET');
        req.flush([]);

        expect(response).toBeTruthy();
    });

    it('should update interests', () => {
        const personId = 'myId';

        const updates = [{id: 'abc', personId, text: 'myText'}] as InterestUpdate[];

        let updatedInterests: InterestUpdate[];

        service.saveInterestForPerson(personId, updates).subscribe(value => (updatedInterests = value));

        const req = httpMock.expectOne(`/api/persons/${personId}/interests`);
        expect(req.request.method).toEqual('PUT');
        expect(req.request.body).toBeInstanceOf(Array);
        const body = Object.fromEntries(req.request.body.entries());
        expect(body).toEqual({
            '0': {
                id: 'abc',
                personId: 'myId',
                text: 'myText',
            },
        });

        const result = [{id: 'abc', personId: 'myId', text: 'text'}] as InterestUpdate[];
        req.flush(result);
        expect(updatedInterests!).toEqual(result);

        httpMock.verify();
    });

    it('should get uid suggestions', () => {
        let response;

        service.getUidOrganizations('mySearchText').subscribe(value => (response = value));

        expect(interceptorMock.deactivateSpinnerOnNextAPICalls).toHaveBeenCalledWith(1);
        const req = httpMock.expectOne(`/api/uid/search?organizationName=mySearchText`);
        expect(req.request.method).toEqual('GET');
        req.flush([]);

        expect(response).toBeTruthy();
    });
});
