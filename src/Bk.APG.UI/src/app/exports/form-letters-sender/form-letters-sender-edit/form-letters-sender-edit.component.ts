import {Component, computed, signal, viewChild, WritableSignal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {ActivatedRoute, Router} from '@angular/router';
import {FormLettersSenderUpdate} from '@api/FormLettersSenderUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObButtonDirective, ObHttpApiInterceptorEvents} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {FormLettersSenderService} from '../form-letters-sender.service';
import {FormLettersSenderDataFormComponent} from '../shared/form-letters-sender-data-form/form-letters-sender-data-form.component';

@Component({
    selector: 'apg-form-letters-sender-edit',
    templateUrl: './form-letters-sender-edit.component.html',
    styleUrl: './form-letters-sender-edit.component.scss',
    imports: [MatButton, ObButtonDirective, FormLettersSenderDataFormComponent, TranslatePipe],
})
export class FormLettersSenderEditComponent {
    senderId = '';
    senderToUpdate!: WritableSignal<FormLettersSenderUpdate>;

    formComponent = viewChild.required(FormLettersSenderDataFormComponent);
    form = computed(() => this.formComponent().senderForm);
    signature = computed(() => this.formComponent().signature);
    signatureFileName = computed(() => this.formComponent().signatureFileName);

    constructor(
        protected readonly errorService: ErrorService,
        protected readonly formLettersSenderService: FormLettersSenderService,
        private readonly notificationService: ObNotificationService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        this.senderId = this.route.snapshot.params.id;
        this.formLettersSenderService.getFormLettersSenderForUpdate(this.senderId).subscribe(sender => (this.senderToUpdate = signal(sender)));
    }

    close() {
        void this.router.navigate(['general-election', 'exports', 'formLetters']);
    }

    reset() {
        this.form().reset(this.senderToUpdate());
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        const senderData: FormLettersSenderUpdate = {
            ...this.form().getRawValue(),
            signature: this.signature(),
            signatureFileName: this.signatureFileName(),
            id: this.senderId,
            canEditDepartment: false, // is not part of the form and there has no use
        };

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.formLettersSenderService.updateFormLettersSender(senderData).subscribe({
            next: async () => {
                this.formLettersSenderService.reload$.next();
                this.form().markAsPristine();
                await this.router.navigate([]);
                this.notificationService.success('formLetter.sender.update.success');
            },
            error: () => this.notificationService.error('formLetter.sender.update.error'),
        });
    }
}
