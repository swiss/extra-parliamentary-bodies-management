import {Inject, Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Router, RouterStateSnapshot} from '@angular/router';
import {SESSION_STORAGE} from '@shared/injection.tokens';
import {OidcSecurityService} from 'angular-auth-oidc-client';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class AuthGuard {
    readonly URL_STORAGE_KEY = 'redirectUrl';

    constructor(
        private readonly router: Router,
        private readonly oidcSecurityService: OidcSecurityService,
        @Inject(SESSION_STORAGE) private readonly sessionStorage: Storage
    ) {}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.oidcSecurityService.checkAuth().pipe(
            map(auth => {
                if (!auth.isAuthenticated && !auth.accessToken) {
                    if (state.url) {
                        this.sessionStorage.setItem(this.URL_STORAGE_KEY, state.url);
                    }
                    this.oidcSecurityService.authorize();
                    return false;
                }

                if (!auth.isAuthenticated) {
                    this.oidcSecurityService
                        .logoffAndRevokeTokens()
                        .subscribe(() => this.router.navigate(['/logout'], {queryParams: {reason: 'SessionExpired'}}));
                    return false;
                }
                if (auth.isAuthenticated && auth.accessToken) {
                    const uri = this.sessionStorage.getItem(this.URL_STORAGE_KEY);
                    this.sessionStorage.removeItem(this.URL_STORAGE_KEY);
                    if (uri) {
                        void this.router.navigateByUrl(uri);
                    }
                }
                return true;
            })
        );
    }
}
