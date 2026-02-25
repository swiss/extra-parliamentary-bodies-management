import {Component, computed, signal, viewChild} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {GeneralElectionCommitteeDetailsService} from '../../ge-committees/ge-committee-details/ge-committee-details.service';
import {MembershipCandidateService} from '../membership-candidate-service';
import {MembershipCandidateDataFormComponent} from '../shared/membership-candidate-data-form/membership-candidate-data-form.component';

@Component({
    selector: 'apg-membership-candidate-edit',
    templateUrl: './membership-candidate-edit.component.html',
    styleUrl: './membership-candidate-edit.component.scss',
    imports: [MatButton, ObButtonDirective, MembershipCandidateDataFormComponent, TranslatePipe],
})
export class MembershipCandidateEditComponent {
    membershipCandidateToUpdate = signal<MembershipCandidateUpdate | undefined>(undefined);
    generalElectionCommittee = signal<GeneralElectionCommitteeDetails | undefined>(undefined);
    formComponent = viewChild.required(MembershipCandidateDataFormComponent);
    form = computed(() => this.formComponent()?.membershipCandidateForm);

    private unmodifiedMembershipCandidate!: MembershipCandidateUpdate;

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly membershipCandidatesService: MembershipCandidateService,
        private readonly generalElectionCommitteeDetailsService: GeneralElectionCommitteeDetailsService,
        private readonly notificationService: ObNotificationService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        this.membershipCandidatesService.getMembershipCandidateForUpdate(this.route.snapshot.params.membershipCandidateId).subscribe(membershipCandidate => {
            this.membershipCandidateToUpdate.set(membershipCandidate);
            this.unmodifiedMembershipCandidate = membershipCandidate;
        });

        this.generalElectionCommitteeDetailsService
            .generalElectionCommitteeDetails(this.route.parent?.snapshot.params.id)
            .subscribe(generalElectionCommitteeDetails => this.generalElectionCommittee.set(generalElectionCommitteeDetails));
    }

    save() {
        if (!this.form()?.valid) {
            this.form()?.markAsTouched();
            return;
        }
        const membershipCandidateToUpdate = this.formComponent().buildMembershipModification();

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.membershipCandidatesService.updateMembershipCandidate(membershipCandidateToUpdate).subscribe({
            next: async () => {
                this.form()?.reset();
                await this.router.navigate(['general-election', 'committees', this.generalElectionCommittee()!.committeeId], {
                    replaceUrl: true,
                    queryParams: {tab: this.generalElectionCommittee()?.isCandidateListCompleted ? 'members' : 'candidateList'},
                });
                return this.notificationService.success('generalElection.membershipCandidate.saveChanges.success');
            },
            error: () => this.notificationService.error('generalElection.membershipCandidate.saveChanges.error'),
        });
    }

    reset() {
        this.form()?.reset();
        this.formComponent()?.resetForm(this.unmodifiedMembershipCandidate);
    }

    back() {
        void this.router.navigate(['general-election', 'committees', this.generalElectionCommittee()!.committeeId], {
            replaceUrl: true,
            queryParams: {tab: this.generalElectionCommittee()?.isCandidateListCompleted ? 'members' : 'candidateList'},
        });
    }
}
