import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, OnInit, signal} from '@angular/core';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatInput} from '@angular/material/input';
import {MatError, MatFormField, MatLabel, MatSuffix} from '@angular/material/select';
import {ActivatedRoute, Router} from '@angular/router';
import {WorklistTaskUpdate} from '@api/WorklistTaskUpdate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {
    ObButtonDirective,
    ObErrorMessagesDirective,
    ObHttpApiInterceptorEvents,
    ObMatErrorDirective,
    ObNotificationService,
    ObUnsavedChangesDirective,
} from '@oblique/oblique';
import {startWith, switchMap} from 'rxjs';
import {WorklistTaskForwardComponent} from '../worklist-task-forward/worklist-task-forward.component';
import {WorklistService} from '../worklist.service';

@Component({
    selector: 'apg-worklist-task-edit',
    imports: [
        ReactiveFormsModule,
        TranslatePipe,
        MatFormField,
        MatLabel,
        MatDatepickerToggle,
        MatDatepicker,
        MatSuffix,
        ObMatErrorDirective,
        ObErrorMessagesDirective,
        CdkTextareaAutosize,
        MatInput,
        MatDatepickerInput,
        MatButton,
        ObButtonDirective,
        ObUnsavedChangesDirective,
        MatError,
        WorklistTaskForwardComponent,
    ],
    templateUrl: './worklist-task-edit.component.html',
    styleUrl: './worklist-task-edit.component.scss',
})
export class WorklistTaskEditComponent implements OnInit {
    form = this.buildForm();
    protected worklistTask = signal<WorklistTaskUpdate | undefined>(undefined);
    protected readonly taskId: string;

    constructor(
        protected readonly worklistService: WorklistService,
        protected readonly notificationService: ObNotificationService,
        protected readonly router: Router,
        protected readonly route: ActivatedRoute,
        protected readonly formBuilder: FormBuilder,
        private readonly translateService: TranslateService,
        private readonly apiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        this.taskId = this.route.snapshot.params.id;
    }

    ngOnInit() {
        this.translateService.onLangChange
            .pipe(
                startWith('de'),
                switchMap(() => this.worklistService.getWorklistTaskForUpdate(this.taskId))
            )
            .subscribe(task => {
                this.worklistTask.set(task);
                if (!task.canEdit) {
                    this.form.disable();
                }
                this.form.patchValue(task);
            });
    }

    save() {
        if (!this.form.valid) {
            this.form.markAllAsTouched();
            return;
        }

        this.apiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.worklistService.update(this.taskId, this.form.getRawValue() as WorklistTaskUpdate).subscribe({
            next: () => {
                this.form.markAsPristine();
                void this.router.navigate(['/worklist']);
                this.notificationService.success('worklist.task.edit.success');
            },
            error: () => this.notificationService.error('worklist.task.edit.error'),
        });
    }

    close() {
        void this.router.navigate(['/worklist']);
    }

    private buildForm() {
        return this.formBuilder.group({
            id: new FormControl({value: '', disabled: true}),
            worklistTaskType: new FormControl({value: '', disabled: true}, {nonNullable: true}),
            worklistTaskState: new FormControl({value: '', disabled: true}, {nonNullable: true}),
            assignedBy: new FormControl({value: '', disabled: true}, {nonNullable: true}),
            assignedTo: new FormControl({value: '', disabled: true}, {nonNullable: true}),
            dueDate: new FormControl<Date | undefined>(undefined, {nonNullable: true, validators: [Validators.required]}),
            description: new FormControl(''),
        });
    }
}
