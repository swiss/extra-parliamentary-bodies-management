import {TestBed} from '@angular/core/testing';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {BehaviorSubject, of} from 'rxjs';
import {ConfigsService} from '../configs.service';
import {AuthService} from './auth.service';
import {Role} from './Role';

describe('AuthService', () => {
    let service: AuthService;
    const userDataResult = {
        userData: {
            given_name: 'peter',
            family_name: 'lustig',
            email: 'peter@lustig.de',
            role: ['allow', 'admin'],
        },
    };
    const userData$ = new BehaviorSubject(userDataResult);
    const authResult$ = new BehaviorSubject({isAuthenticated: false});
    const oidcSecurityServiceMock = {
        userData$: userData$.asObservable(),
        isAuthenticated$: authResult$.asObservable(),
        logoff: jest.fn(),
        authorize: jest.fn(),
    };

    const configsServiceMock = {
        frontendConfig: {
            keyCloakRoleAllow: 'Allow',
            keyCloakRoleAdmin: 'Admin',
            keyCloakRoleDepartment: 'Department',
            keyCloakRoleOffice: 'Office',
            keyCloakRoleSecretariat: 'Secretariat',
            keyCloakRoleObserver: 'Observer',
        },
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                {provide: OidcSecurityService, useValue: oidcSecurityServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
            ],
        });
        service = TestBed.inject(AuthService);
        oidcSecurityServiceMock.logoff.mockReturnValue(of({}));
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should set userinfo correctly from userdata', done => {
        const localUserDataResult = {
            userData: {given_name: 'Boogie', family_name: 'Man', email: 'boogie@man.org', role: ['boogie', 'man']},
        };
        userData$.next(localUserDataResult);

        service.userInfo$.subscribe(result => {
            expect(result.roles).toBe(localUserDataResult.userData.role);
            expect(result.email).toBe(localUserDataResult.userData.email);
            expect(result.firstName).toBe(localUserDataResult.userData.given_name);
            expect(result.lastName).toBe(localUserDataResult.userData.family_name);
            done();
        });
    });

    it.each([
        ['Admin', Role.Admin],
        ['Department', Role.Department],
        ['Office', Role.Office],
        ['Secretariat', Role.Secretariat],
        ['Spectator', Role.Observer],
    ])('should set roles correctly', (role: string, expectedRole: number) => {
        const localUserDataResult = {
            userData: {given_name: 'Boogie', family_name: 'Man', email: 'boogie@man.org', role: ['Allow', role]},
        };
        userData$.next(localUserDataResult);

        expect(
            service.roles$.subscribe(result => {
                expect(result).toEqual([Role.Allow, expectedRole]);
            })
        );
    });

    it(`should call logoff on logout`, () => {
        service.logout();

        expect(oidcSecurityServiceMock.logoff).toHaveBeenCalledTimes(1);
    });

    it('should call authorize on login', () => {
        service.login();

        expect(oidcSecurityServiceMock.authorize).toHaveBeenCalledTimes(1);
    });
});
