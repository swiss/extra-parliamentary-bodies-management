import {Component, computed, signal, viewChild, WritableSignal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeTypeUpdate} from '@api/CommitteeTypeUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService, ObButtonDirective} from '@oblique/oblique';
import {CommitteeTypeFormComponent} from '../committee-type-form/committee-type-form.component';
import {CommitteeTypeService} from '../committee-type.service';

@Component({
    selector: 'apg-committee-type-edit',
    templateUrl: './committee-type-edit.component.html',
    styleUrl: './committee-type-edit.component.scss',
    providers: [],
    imports: [MatButton, ObButtonDirective, CommitteeTypeFormComponent, TranslatePipe],
})
export class CommitteeTypeEditComponent {
    committeeTypeToUpdate!: WritableSignal<CommitteeTypeUpdate>;
    formComponent = viewChild.required(CommitteeTypeFormComponent);
    form = computed(() => this.formComponent().committeeTypeForm);

    isUpdateMode = true;
    isCopyMode = false;
    committeeTypeName = '';

    unmodifiedCommitteeType!: CommitteeTypeUpdate;

    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly notificationService: ObNotificationService,
        private readonly committeeTypeService: CommitteeTypeService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        if (this.route.snapshot.url[1]?.path === 'copy') {
            this.isCopyMode = true;
            this.isUpdateMode = false;
        }

        this.committeeTypeService.getCommitteeTypeForUpdate(this.route.snapshot.params.id).subscribe(committeeTypeToUpdate => {
            this.committeeTypeToUpdate = signal(committeeTypeToUpdate);
            this.unmodifiedCommitteeType = committeeTypeToUpdate;
            this.committeeTypeName = this.committeeTypeToUpdate().text;
        });
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();
        this.committeeTypeService.updateCommitteeType(this.committeeTypeToUpdate()).subscribe({
            next: async () => {
                this.form().reset(this.committeeTypeToUpdate(), {emitEvent: false});
                this.unmodifiedCommitteeType = this.committeeTypeToUpdate();
                await this.router.navigate(['administration/committeeTypes']);
                return this.notificationService.success('committeeType.save.success');
            },
            error: () => this.notificationService.error('committeeType.save.error'),
        });
    }

    reset() {
        this.form().reset(this.unmodifiedCommitteeType, {emitEvent: false});
    }

    async back() {
        await this.router.navigate(['administration/committeeTypes']);
    }
}
