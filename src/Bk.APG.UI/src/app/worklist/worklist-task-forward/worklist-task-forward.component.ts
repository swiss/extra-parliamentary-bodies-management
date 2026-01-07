import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, input} from '@angular/core';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatError, MatFormField} from '@angular/material/select';
import {Router} from '@angular/router';
import {WorklistTaskForward} from '@api/WorklistTaskForward';
import {TranslatePipe} from '@ngx-translate/core';
import {
    ObAlertComponent,
    ObButtonDirective,
    ObErrorMessagesDirective,
    ObHttpApiInterceptorEvents,
    ObMatErrorDirective,
    ObNotificationService,
    ObUnsavedChangesDirective,
} from '@oblique/oblique';
import {WorklistService} from '../worklist.service';

@Component({
    selector: 'apg-worklist-task-forward',
    imports: [
        MatButton,
        ObButtonDirective,
        ReactiveFormsModule,
        TranslatePipe,
        ObUnsavedChangesDirective,
        MatDatepicker,
        MatDatepickerInput,
        MatDatepickerToggle,
        MatError,
        MatFormField,
        MatInput,
        MatLabel,
        MatSuffix,
        ObErrorMessagesDirective,
        ObMatErrorDirective,
        ObAlertComponent,
        CdkTextareaAutosize,
    ],
    templateUrl: './worklist-task-forward.component.html',
    styleUrl: './worklist-task-forward.component.scss',
})
export class WorklistTaskForwardComponent {
    form = this.buildForm();
    worklistTaskId = input.required<string>();
    isBigDepartment = input.required<boolean>();

    constructor(
        private readonly formBuilder: FormBuilder,
        private readonly worklistService: WorklistService,
        private readonly notificationService: ObNotificationService,
        private readonly router: Router,
        private readonly apiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {}

    forward() {
        if (!this.form.valid) {
            this.form.markAllAsTouched();
            return;
        }

        this.apiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.worklistService.forward(this.worklistTaskId(), this.form.getRawValue() as WorklistTaskForward).subscribe({
            next: () => {
                this.form.markAsPristine();
                void this.router.navigate(['/worklist']);
                this.notificationService.success('worklist.task.forward.success');
            },
            error: () => this.notificationService.error('worklist.task.forward.error'),
        });
    }

    private buildForm() {
        return this.formBuilder.group({
            candidateListDueDate: new FormControl<Date | undefined>(undefined, {nonNullable: true, validators: [Validators.required]}),
            candidateListDescription: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
            committeeDueDate: new FormControl<Date | undefined>(undefined, {nonNullable: true, validators: [Validators.required]}),
            committeeDescription: new FormControl('', {nonNullable: true, validators: [Validators.required]}),
        });
    }
}
