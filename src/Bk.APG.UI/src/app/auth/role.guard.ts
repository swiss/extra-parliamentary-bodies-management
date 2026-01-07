import {Injectable, OnDestroy} from '@angular/core';
import {ActivatedRouteSnapshot, Router, UrlTree} from '@angular/router';
import {Observable, Subject, takeUntil} from 'rxjs';
import {map} from 'rxjs/operators';
import {AuthService} from './auth.service';
import {Role} from './Role';

@Injectable({
    providedIn: 'root',
})
export class RoleGuard implements OnDestroy {
    private readonly unsubscribe = new Subject();

    constructor(
        private readonly router: Router,
        private readonly authService: AuthService
    ) {}

    canActivate(route: ActivatedRouteSnapshot): Observable<boolean | UrlTree> {
        const allowedRoles = route.data?.allowedRoles as Role[] | undefined;

        return this.authService.roles$.pipe(
            takeUntil(this.unsubscribe),
            map(roles => {
                const hasAccess = roles.some(r => allowedRoles?.includes(r));

                if (hasAccess) {
                    return true;
                }

                return this.router.createUrlTree(['/unauthorized']);
            })
        );
    }

    ngOnDestroy(): void {
        this.unsubscribe.next(null);
        this.unsubscribe.complete();
    }
}
