import {computed, Injectable, signal} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {AuthService} from '../../auth/auth.service';

@Injectable()
export class CommitteeDetailsService {
    committeeDetails = signal<CommitteeDetails | undefined>(undefined);

    isDataFormDirty = signal(false);
    isMembersFormDirty = signal(false);
    isContactsFormDirty = signal(false);
    isJustificationsFormDirty = signal(false);
    isDecisionsFormDirty = signal(false);

    isObserver = toSignal(this.authService.isObserver$, {initialValue: false});

    isDataTabDisabled = computed(
        () => this.isMembersFormDirty() || this.isContactsFormDirty() || this.isJustificationsFormDirty() || this.isDecisionsFormDirty()
    );
    isMembersTabDisabled = computed(
        () => this.isDataFormDirty() || this.isContactsFormDirty() || this.isJustificationsFormDirty() || this.isDecisionsFormDirty()
    );
    isContactsTabDisabled = computed(
        () => this.isDataFormDirty() || this.isMembersFormDirty() || this.isJustificationsFormDirty() || this.isDecisionsFormDirty()
    );
    isJustificationsTabDisabled = computed(
        () => this.isDataFormDirty() || this.isContactsFormDirty() || this.isMembersFormDirty() || this.isDecisionsFormDirty()
    );
    isDecisionsTabDisabled = computed(
        () => this.isDataFormDirty() || this.isContactsFormDirty() || this.isMembersFormDirty() || this.isJustificationsFormDirty()
    );

    constructor(private readonly authService: AuthService) {}
}
