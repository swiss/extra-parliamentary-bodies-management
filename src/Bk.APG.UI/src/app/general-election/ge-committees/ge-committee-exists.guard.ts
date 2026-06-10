import {HttpErrorResponse} from '@angular/common/http';
import {inject} from '@angular/core';
import {ActivatedRouteSnapshot, Router} from '@angular/router';
import {catchError, map, of, throwError} from 'rxjs';
import {GeneralElectionCommitteeDetailsService} from './ge-committee-details/ge-committee-details.service';

export const GeneralElectionCommitteeExistsGuard = (route: ActivatedRouteSnapshot) => {
    const router = inject(Router);
    const service = inject(GeneralElectionCommitteeDetailsService);
    const committeeId = route.paramMap.get('id');

    if (!committeeId) {
        return router.createUrlTree(['/general-election/committees']);
    }

    return service.generalElectionCommitteeDetails(committeeId).pipe(
        map(committeeDetails => {
            service.committeeDetails.set(committeeDetails);
            return true;
        }),
        catchError((error: HttpErrorResponse) => {
            if (error.status === 404) {
                void router.navigate(['/general-election/committees']);
                return of(false);
            }
            return throwError(() => error);
        })
    );
};
