import {HttpErrorResponse} from '@angular/common/http';
import {TestBed} from '@angular/core/testing';
import {ActivatedRouteSnapshot, Router, UrlTree} from '@angular/router';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {of, throwError} from 'rxjs';
import {GeneralElectionCommitteeDetailsService} from './ge-committee-details/ge-committee-details.service';
import {GeneralElectionCommitteeExistsGuard} from './ge-committee-exists.guard';

describe('GeneralElectionCommitteeExistsGuard', () => {
    let service: jest.Mocked<GeneralElectionCommitteeDetailsService>;
    let router: jest.Mocked<Router>;
    let route: ActivatedRouteSnapshot;

    beforeEach(() => {
        const serviceMock = {
            generalElectionCommitteeDetails: jest.fn(),
            committeeDetails: {
                set: jest.fn(),
            },
        };

        const routerMock = {
            navigate: jest.fn(),
            createUrlTree: jest.fn(),
        };

        TestBed.configureTestingModule({
            providers: [
                {provide: GeneralElectionCommitteeDetailsService, useValue: serviceMock},
                {provide: Router, useValue: routerMock},
            ],
        });

        service = TestBed.inject(GeneralElectionCommitteeDetailsService) as jest.Mocked<GeneralElectionCommitteeDetailsService>;
        router = TestBed.inject(Router) as jest.Mocked<Router>;
        route = {
            paramMap: {
                get: jest.fn(),
            },
        } as unknown as ActivatedRouteSnapshot;
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should return true when committee exists', done => {
        const mockCommittee = {id: '123'} as GeneralElectionCommitteeDetails;
        (route.paramMap.get as jest.Mock).mockReturnValue('123');
        service.generalElectionCommitteeDetails.mockReturnValue(of(mockCommittee));

        const result = TestBed.runInInjectionContext(() => GeneralElectionCommitteeExistsGuard(route));

        if (typeof result === 'object' && 'subscribe' in result) {
            result.subscribe(value => {
                expect(value).toBe(true);
                expect(service.generalElectionCommitteeDetails).toHaveBeenCalledWith('123');
                expect(service.committeeDetails.set).toHaveBeenCalledWith(mockCommittee);
                expect(router.navigate).not.toHaveBeenCalled();
                done();
            });
        }
    });

    it('should set committeeDetails in service when committee exists', done => {
        const mockCommittee = {id: '123', description: 'Test Committee'} as GeneralElectionCommitteeDetails;
        (route.paramMap.get as jest.Mock).mockReturnValue('123');
        service.generalElectionCommitteeDetails.mockReturnValue(of(mockCommittee));

        const result = TestBed.runInInjectionContext(() => GeneralElectionCommitteeExistsGuard(route));

        if (typeof result === 'object' && 'subscribe' in result) {
            result.subscribe(() => {
                expect(service.committeeDetails.set).toHaveBeenCalledWith(mockCommittee);
                done();
            });
        }
    });

    it('should redirect to committees list when committee does not exist (404)', done => {
        const error = new HttpErrorResponse({status: 404});
        (route.paramMap.get as jest.Mock).mockReturnValue('123');
        service.generalElectionCommitteeDetails.mockReturnValue(throwError(() => error));

        const result = TestBed.runInInjectionContext(() => GeneralElectionCommitteeExistsGuard(route));

        if (typeof result === 'object' && 'subscribe' in result) {
            result.subscribe(value => {
                expect(value).toBe(false);
                expect(service.generalElectionCommitteeDetails).toHaveBeenCalledWith('123');
                expect(service.committeeDetails.set).not.toHaveBeenCalled();
                expect(router.navigate).toHaveBeenCalledWith(['/general-election/committees']);
                done();
            });
        }
    });

    it('should rethrow error when HTTP error is not 404', done => {
        const error = new HttpErrorResponse({status: 500});
        (route.paramMap.get as jest.Mock).mockReturnValue('123');
        service.generalElectionCommitteeDetails.mockReturnValue(throwError(() => error));

        const result = TestBed.runInInjectionContext(() => GeneralElectionCommitteeExistsGuard(route));

        if (typeof result === 'object' && 'subscribe' in result) {
            result.subscribe({
                error: err => {
                    expect(err.status).toBe(500);
                    expect(service.generalElectionCommitteeDetails).toHaveBeenCalledWith('123');
                    expect(service.committeeDetails.set).not.toHaveBeenCalled();
                    expect(router.navigate).not.toHaveBeenCalled();
                    done();
                },
            });
        }
    });

    it('should redirect to committees list when committeeId is missing', () => {
        (route.paramMap.get as jest.Mock).mockReturnValue(null);
        const mockUrlTree = {} as UrlTree;
        router.createUrlTree.mockReturnValue(mockUrlTree);

        const result = TestBed.runInInjectionContext(() => GeneralElectionCommitteeExistsGuard(route));

        expect(result).toBe(mockUrlTree);
        expect(router.createUrlTree).toHaveBeenCalledWith(['/general-election/committees']);
        expect(service.generalElectionCommitteeDetails).not.toHaveBeenCalled();
        expect(service.committeeDetails.set).not.toHaveBeenCalled();
    });
});
