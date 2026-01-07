import {TestBed} from '@angular/core/testing';
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree} from '@angular/router';
import {SESSION_STORAGE} from '@shared/injection.tokens';
import {LoginResponse, OidcSecurityService} from 'angular-auth-oidc-client';
import {first, Observable, of} from 'rxjs';
import {AuthGuard} from './auth.guard';

describe('AuthGuard', () => {
    let guard: AuthGuard;

    const oidcSecurityServiceMock = {
        checkAuth: jest.fn(),
        logoffAndRevokeTokens: jest.fn(),
        authorize: jest.fn(),
    };

    const routerMock = {
        navigate: jest.fn(),
        navigateByUrl: jest.fn(),
    };

    const sessionStorageMock = {
        getItem: jest.fn(),
        setItem: jest.fn(),
        removeItem: jest.fn().mockImplementation(() => {}),
    };

    const routeMock = {} as ActivatedRouteSnapshot;
    const stateMock = {} as RouterStateSnapshot;

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                {provide: OidcSecurityService, useValue: oidcSecurityServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: SESSION_STORAGE, useValue: sessionStorageMock},
            ],
        });

        guard = TestBed.inject(AuthGuard);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(guard).toBeTruthy();
    });

    it('should return true when logged in', () => {
        const loginRespone: LoginResponse = {
            accessToken: 'token',
            isAuthenticated: true,
            userData: '',
            configId: '1',
            idToken: 'idToken',
        };
        oidcSecurityServiceMock.checkAuth.mockReturnValue(of(loginRespone));
        sessionStorageMock.getItem.mockReturnValue('');

        const obs = guard.canActivate(routeMock, stateMock) as Observable<boolean | UrlTree>;
        let obsResult = null;
        obs.pipe(first()).subscribe(result => {
            obsResult = result;
        });

        expect(obsResult).toEqual(true);
        expect(oidcSecurityServiceMock.authorize).not.toHaveBeenCalled();
        expect(oidcSecurityServiceMock.logoffAndRevokeTokens).not.toHaveBeenCalled();
    });

    it('should redirect after login and remove url from the session storage', () => {
        const loginRespone: LoginResponse = {
            accessToken: 'token',
            isAuthenticated: true,
            userData: '',
            configId: '1',
            idToken: 'idToken',
        };
        oidcSecurityServiceMock.checkAuth.mockReturnValue(of(loginRespone));
        sessionStorageMock.getItem.mockReturnValue('testUrl');

        guard.canActivate(routeMock, stateMock).subscribe();

        expect(oidcSecurityServiceMock.authorize).not.toHaveBeenCalled();
        expect(oidcSecurityServiceMock.logoffAndRevokeTokens).not.toHaveBeenCalled();
        expect(sessionStorageMock.getItem).toHaveBeenCalledTimes(1);
        expect(sessionStorageMock.removeItem).toHaveBeenCalledTimes(1);
        expect(routerMock.navigateByUrl).toHaveBeenCalledWith('testUrl');
    });

    it('should login when not authenticated and no token', () => {
        const loginRespone: LoginResponse = {
            accessToken: '',
            isAuthenticated: false,
            userData: '',
            configId: '1',
            idToken: 'idToken',
        };
        oidcSecurityServiceMock.checkAuth.mockReturnValue(of(loginRespone));

        const obs = guard.canActivate(routeMock, stateMock) as Observable<boolean | UrlTree>;
        let obsResult = null;
        obs.pipe(first()).subscribe(result => {
            obsResult = result;
        });

        expect(obsResult).toEqual(false);
        expect(oidcSecurityServiceMock.authorize).toHaveBeenCalledTimes(1);
    });

    it('should save redirect url into session storage when has to be authenticated', () => {
        const loginRespone: LoginResponse = {
            accessToken: '',
            isAuthenticated: false,
            userData: '',
            configId: '1',
            idToken: 'idToken',
        };
        oidcSecurityServiceMock.checkAuth.mockReturnValue(of(loginRespone));

        const obs = guard.canActivate(routeMock, {url: 'redirectUrl'} as RouterStateSnapshot) as Observable<boolean | UrlTree>;
        let obsResult = null;
        obs.pipe(first()).subscribe(result => {
            obsResult = result;
        });

        expect(obsResult).toEqual(false);
        expect(sessionStorageMock.setItem).toHaveBeenCalledTimes(1);
        expect(oidcSecurityServiceMock.authorize).toHaveBeenCalledTimes(1);
    });

    it('should log off when not authenticated but has stale token', () => {
        const loginRespone: LoginResponse = {
            accessToken: 'token',
            isAuthenticated: false,
            userData: '',
            configId: '1',
            idToken: 'idToken',
        };
        oidcSecurityServiceMock.checkAuth.mockReturnValue(of(loginRespone));

        const obs = guard.canActivate(routeMock, stateMock) as Observable<boolean | UrlTree>;
        let obsResult = null;
        obs.pipe(first()).subscribe(result => {
            obsResult = result;
            expect(obsResult).toEqual(false);
        });

        expect(oidcSecurityServiceMock.authorize).not.toHaveBeenCalled();
        expect(oidcSecurityServiceMock.logoffAndRevokeTokens).toHaveBeenCalledTimes(1);
    });
});
