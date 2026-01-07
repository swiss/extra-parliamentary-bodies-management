import {HttpClient} from '@angular/common/http';
import {Injectable, signal} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';

@Injectable({providedIn: 'root'})
export class GeneralElectionCommitteeDetailsService {
    committeeDetails = signal<GeneralElectionCommitteeDetails | undefined>(undefined);
    isDataFormDirty = signal(false);
    reload$ = new BehaviorSubject<void>(undefined);

    isObserver = toSignal(this.authService.isObserver$, {initialValue: false});

    constructor(
        private readonly http: HttpClient,
        private readonly authService: AuthService
    ) {}

    generalElectionCommitteeDetails = (committeeId: string) =>
        this.http.get<GeneralElectionCommitteeDetails>(`/api/general-election/committees/${committeeId}`);
}
