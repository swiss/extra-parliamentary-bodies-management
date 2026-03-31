import {Component, computed, signal, viewChild} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {Router} from '@angular/router';
import {FormLettersSenderCreate} from '@api/FormLettersSenderCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObButtonDirective, ObHttpApiInterceptorEvents} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {FormLettersSenderService} from '../form-letters-sender.service';
import {FormLettersSenderDataFormComponent} from '../shared/form-letters-sender-data-form/form-letters-sender-data-form.component';

@Component({
    selector: 'apg-form-letters-sender-create',
    templateUrl: './form-letters-sender-create.component.html',
    styleUrl: './form-letters-sender-create.component.scss',
    imports: [MatButton, ObButtonDirective, FormLettersSenderDataFormComponent, TranslatePipe],
})
export class FormLettersSenderCreateComponent {
    emptyFormLettersSender = signal({} as FormLettersSenderCreate);
    formComponent = viewChild.required(FormLettersSenderDataFormComponent);
    form = computed(() => this.formComponent().senderForm);
    signature = computed(() => this.formComponent().signature);

    constructor(
        protected readonly errorService: ErrorService,
        protected readonly formLettersSenderService: FormLettersSenderService,
        private readonly notificationService: ObNotificationService,
        private readonly router: Router,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        this.formLettersSenderService.getEmptyFormLettersSender().subscribe(emptyFormLettersSender => this.emptyFormLettersSender.set(emptyFormLettersSender));
    }

    close() {
        void this.router.navigate(['general-election', 'exports', 'formLetters']);
    }

    save() {
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        const senderData: FormLettersSenderCreate = {...this.form().getRawValue(), signature: this.signature()};

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.formLettersSenderService.createFormLettersSender(senderData).subscribe({
            next: async formLetterSender => {
                this.form().reset(senderData);
                this.form().markAsPristine();
                this.formLettersSenderService.reload$.next();
                await this.router.navigate(['general-election', 'exports', 'formLettersSenders', formLetterSender.id]);
                this.notificationService.success('formLetter.sender.create.success');
            },
            error: () => this.notificationService.error('formLetter.sender.create.error'),
        });
    }
}
