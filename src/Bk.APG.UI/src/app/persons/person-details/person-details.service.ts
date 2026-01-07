import {computed, Injectable, signal} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {PersonDetails} from '@api/PersonDetails';
import {AuthService} from '../../auth/auth.service';

@Injectable()
export class PersonDetailsService {
    personDetails = signal<PersonDetails>({} as PersonDetails);
    personName = computed(() => `${this.personDetails().surname ?? ''} ${this.personDetails().givenName ?? ''}`);
    needsAttentionOccupation = computed(() => this.personDetails().needsAttentionOccupation);
    needsAttentionBasicData = computed(() => this.personDetails().needsAttentionBasicData);
    needsAttentionInterests = computed(() => this.personDetails().needsAttentionInterests);
    needsAttentionFederalDuty = computed(() => this.personDetails().needsAttentionFederalDuty);
    needsAttentionFederalAssembly = computed(() => this.personDetails().needsAttentionFederalAssembly);
    needsAttentionLongerDuty = computed(() => this.personDetails().needsAttentionLongerDuty);
    needsAttentionShorterDuty = computed(() => this.personDetails().needsAttentionShorterDuty);
    needsAttentionMembershipExpired = computed(() => this.personDetails().needsAttentionMembershipExpired);

    isObserver = toSignal(this.authService.isObserver$, {initialValue: false});
    isDataFormDirty = signal(false);
    isInterestsFormDirty = signal(false);

    isDataTabDisabled = computed(() => this.isInterestsFormDirty() || this.isObserver());
    isInterestsTabDisabled = computed(() => this.isDataFormDirty() || this.isObserver());
    isMembershipsTabDisabled = computed(() => this.isDataFormDirty() || this.isInterestsFormDirty() || this.isObserver());

    constructor(private readonly authService: AuthService) {}
}
