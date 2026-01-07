import {HttpClient} from '@angular/common/http';
import {Injectable, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {NavigationEnd, Router} from '@angular/router';
import {Observable, switchMap} from 'rxjs';
import {filter, map} from 'rxjs/operators';
import {AuthService} from '../auth/auth.service';

@Injectable({
    providedIn: 'root',
})
export class GeneralElectionService {
    isGeneralElectionVisible = signal(false);
    isGeneralElectionEnabled = signal(false);

    constructor(
        readonly authService: AuthService,
        private readonly http: HttpClient,
        private readonly router: Router
    ) {
        authService.isAuthenticated$
            .pipe(
                filter(isAuthenticated => isAuthenticated),
                switchMap(() => this.checkIfGeneralElectionIsAvailable()),
                takeUntilDestroyed()
            )
            .subscribe(isGeneralElectionAvailable => this.isGeneralElectionVisible.set(isGeneralElectionAvailable));

        this.router.events
            .pipe(
                filter(event => event instanceof NavigationEnd),
                map(() => this.router.url.includes('general-election')),
                takeUntilDestroyed()
            )
            .subscribe(isGeneralElectionRoute => this.isGeneralElectionEnabled.set(isGeneralElectionRoute));
    }

    toggleGeneralElection(enableGeneralElection: boolean): void {
        if (enableGeneralElection) {
            const currentUrl = this.router.url;
            const generalElectionUrl = currentUrl.includes('general-election') ? currentUrl : `general-election${currentUrl}`;
            void this.router.navigateByUrl(generalElectionUrl);
        } else {
            const previousUrl = this.router.url.replace('/general-election', '');
            void this.router.navigateByUrl(previousUrl);
        }
    }

    private checkIfGeneralElectionIsAvailable(): Observable<boolean> {
        return this.http.get<boolean>('/api/general-election/toggle-available');
    }
}
