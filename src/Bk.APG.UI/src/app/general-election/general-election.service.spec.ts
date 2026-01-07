/* eslint-disable @typescript-eslint/no-explicit-any */
import {provideHttpClient} from '@angular/common/http';
import {HttpTestingController, provideHttpClientTesting} from '@angular/common/http/testing';
import {TestBed} from '@angular/core/testing';
import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {of, Subject} from 'rxjs';
import {AuthService} from '../auth/auth.service';
import {GeneralElectionService} from './general-election.service';

interface MockRouter extends Omit<Router, 'url'> {
    url: string;
}

describe('GeneralElectionService', () => {
    let service: GeneralElectionService;
    let httpMock: HttpTestingController;
    let mockRouter: jest.Mocked<MockRouter>;
    let mockActivatedRoute: jest.Mocked<ActivatedRoute>;
    let mockAuthService: jest.Mocked<AuthService>;
    let routerEventsSubject: Subject<any>;

    beforeEach(() => {
        routerEventsSubject = new Subject();
        mockRouter = {
            url: '/some-path',
            navigateByUrl: jest.fn().mockResolvedValue(true),
            events: routerEventsSubject.asObservable(),
        } as any;
        mockActivatedRoute = {} as any;
        mockAuthService = {isAuthenticated$: of(true)} as any;

        TestBed.configureTestingModule({
            providers: [
                provideHttpClient(),
                provideHttpClientTesting(),
                {provide: Router, useValue: mockRouter},
                {provide: ActivatedRoute, useValue: mockActivatedRoute},
                {provide: AuthService, useValue: mockAuthService},
            ],
        });

        httpMock = TestBed.inject(HttpTestingController);
        service = TestBed.inject(GeneralElectionService);

        const req = httpMock.expectOne('/api/general-election/toggle-available');
        req.flush(false);
    });

    afterEach(() => {
        httpMock?.verify();
        routerEventsSubject?.complete();
    });

    const setRouterUrl = (url: string) => {
        mockRouter.url = url;
    };

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should initialize with default signal values', () => {
        expect(service.isGeneralElectionVisible()).toBe(false);
        expect(service.isGeneralElectionEnabled()).toBe(false);
    });

    it('should set isGeneralElectionEnabled to true when navigating to general-election route', () => {
        setRouterUrl('/general-election/some-path');

        routerEventsSubject.next(new NavigationEnd(1, '/general-election/some-path', '/general-election/some-path'));

        expect(service.isGeneralElectionEnabled()).toBe(true);
    });

    it('should set isGeneralElectionEnabled to false when navigating away from general-election route', () => {
        setRouterUrl('/other-path');

        routerEventsSubject.next(new NavigationEnd(1, '/other-path', '/other-path'));

        expect(service.isGeneralElectionEnabled()).toBe(false);
    });

    describe('toggleGeneralElection', () => {
        it('should navigate to general-election route when enabling', () => {
            service.toggleGeneralElection(true);

            expect(mockRouter.navigateByUrl).toHaveBeenCalledWith('general-election/some-path');
        });

        it('should not modify URL when already on general-election route and enabling', () => {
            setRouterUrl('/general-election/some-path');

            service.toggleGeneralElection(true);

            expect(mockRouter.navigateByUrl).toHaveBeenCalledWith('/general-election/some-path');
        });

        it('should navigate away from general-election route when disabling', () => {
            setRouterUrl('/general-election/some-path');

            service.toggleGeneralElection(false);

            expect(mockRouter.navigateByUrl).toHaveBeenCalledWith('/some-path');
        });

        it('should handle multiple general-election segments in URL when disabling', () => {
            setRouterUrl('/general-election/general-election/some-path');

            service.toggleGeneralElection(false);

            expect(mockRouter.navigateByUrl).toHaveBeenCalledWith('/general-election/some-path');
        });
    });
});
