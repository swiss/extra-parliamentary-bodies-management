import {Injectable, OnDestroy} from '@angular/core';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {distinctUntilChanged, map, Observable, Subject, takeUntil} from 'rxjs';
import {ConfigsService} from '../configs.service';
import {Role} from './Role';
import {UserInfo} from './UserInfo';

@Injectable({
    providedIn: 'root',
})
export class AuthService implements OnDestroy {
    readonly userInfo$: Observable<UserInfo>;
    readonly roles$: Observable<Role[]>;
    readonly isAuthenticated$: Observable<boolean>;
    readonly userName$: Observable<string>;

    readonly isAdmin$: Observable<boolean>;
    readonly isObserver$: Observable<boolean>;
    readonly isDepartmentUser$: Observable<boolean>;
    readonly isOfficeUser$: Observable<boolean>;
    readonly isSecretariatUser$: Observable<boolean>;

    private static readonly CLAIM_NAME_GIVEN_NAME = 'given_name';
    private static readonly CLAIM_NAME_FAMILY_NAME = 'family_name';
    private static readonly CLAIM_NAME_EMAIL = 'email';
    private static readonly CLAIM_NAME_ROLE = 'role';

    private readonly unsubscribe = new Subject<void>();

    constructor(
        private readonly oidcSecurityService: OidcSecurityService,
        private readonly configsService: ConfigsService
    ) {
        this.userInfo$ = this.oidcSecurityService.userData$.pipe(
            map(data =>
                data?.userData
                    ? {
                          firstName: data?.userData[AuthService.CLAIM_NAME_GIVEN_NAME] ?? '',
                          lastName: data?.userData[AuthService.CLAIM_NAME_FAMILY_NAME] ?? '',
                          email: data.userData[AuthService.CLAIM_NAME_EMAIL] ?? '',
                          roles: data.userData[AuthService.CLAIM_NAME_ROLE] ?? [],
                      }
                    : {}
            ),
            takeUntil(this.unsubscribe)
        );

        this.roles$ = this.observeRoles();

        this.isAuthenticated$ = this.oidcSecurityService.isAuthenticated$.pipe(
            takeUntil(this.unsubscribe),
            map(authResult => authResult.isAuthenticated),
            distinctUntilChanged()
        );

        this.userName$ = this.userInfo$.pipe(
            map(userInfo => `${userInfo.firstName ?? ''} ${userInfo.lastName ?? ''}`),
            takeUntil(this.unsubscribe)
        );

        this.isObserver$ = this.roles$.pipe(map(roles => roles.includes(Role.Observer)));
        this.isDepartmentUser$ = this.roles$.pipe(map(roles => roles.includes(Role.Department)));
        this.isOfficeUser$ = this.roles$.pipe(map(roles => roles.includes(Role.Office)));
        this.isAdmin$ = this.roles$.pipe(map(roles => roles.includes(Role.Admin)));
        this.isSecretariatUser$ = this.roles$.pipe(map(roles => roles.includes(Role.Secretariat)));
    }

    ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
    }

    logout(): void {
        this.oidcSecurityService.logoff().subscribe();
    }

    login(): void {
        this.oidcSecurityService.authorize();
    }

    private observeRoles(): Observable<Role[]> {
        const roleMap = {
            [this.configsService.frontendConfig.keyCloakRoleAllow]: Role.Allow,
            [this.configsService.frontendConfig.keyCloakRoleAdmin]: Role.Admin,
            [this.configsService.frontendConfig.keyCloakRoleDepartment]: Role.Department,
            [this.configsService.frontendConfig.keyCloakRoleOffice]: Role.Office,
            [this.configsService.frontendConfig.keyCloakRoleSecretariat]: Role.Secretariat,
            [this.configsService.frontendConfig.keyCloakRoleObserver]: Role.Observer,
        };
        return this.userInfo$.pipe(
            map(userInfo =>
                userInfo.roles?.includes(this.configsService.frontendConfig.keyCloakRoleAllow)
                    ? userInfo.roles?.map(role => roleMap[role]).filter(role => role != null)
                    : []
            ),
            takeUntil(this.unsubscribe)
        );
    }
}
