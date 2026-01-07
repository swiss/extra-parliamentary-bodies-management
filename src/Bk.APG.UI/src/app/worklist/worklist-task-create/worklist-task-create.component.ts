import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, computed, OnInit, signal} from '@angular/core';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatError, MatOption, MatSelect} from '@angular/material/select';
import {Router} from '@angular/router';
import {EiamAssignment} from '@api/EiamAssignment';
import {WorklistTaskCreate} from '@api/WorklistTaskCreate';
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
import {conditionalValidator} from '@shared/form-validators/conditional.validator';
import {MasterDataService} from '@shared/master-data.service';
import {EiamAssignmentService} from '@shared/services/eiam-assignment.service';
import {ConfigsService} from '../../configs.service';
import {GeneralElectionService} from '../../general-election/general-election.service';
import {WorklistService} from '../worklist.service';

@Component({
    selector: 'apg-worklist-task-create',
    imports: [
        TranslatePipe,
        MatButton,
        ObButtonDirective,
        ReactiveFormsModule,
        ObUnsavedChangesDirective,
        MatFormField,
        MatLabel,
        MatSelect,
        MatOption,
        ObMatErrorDirective,
        MatDatepickerToggle,
        MatDatepicker,
        MatDatepickerInput,
        MatInput,
        ObErrorMessagesDirective,
        MatSuffix,
        MatError,
        CdkTextareaAutosize,
        ObAlertComponent,
    ],
    templateUrl: './worklist-task-create.component.html',
    styleUrl: './worklist-task-create.component.scss',
})
export class WorklistTaskCreateComponent implements OnInit {
    form = this.buildForm();
    availableAssignments = signal<EiamAssignment[]>([]);
    readonly worklistTaskTypes = computed(() => {
        return this.masterDataService.worklistTaskTypes().filter(y => y.canBeCreatedManually);
    });
    constructor(
        protected readonly masterDataService: MasterDataService,
        protected readonly worklistService: WorklistService,
        protected readonly notificationService: ObNotificationService,
        protected readonly router: Router,
        protected readonly formBuilder: FormBuilder,
        protected readonly configsService: ConfigsService,
        private readonly generalElectionService: GeneralElectionService,
        protected readonly eiamAssignmentService: EiamAssignmentService,
        private readonly apiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {}

    ngOnInit() {
        this.eiamAssignmentService.getAvailableEiamAssignments().subscribe(assignments => this.availableAssignments.set(assignments));
    }

    save() {
        if (!this.form.valid) {
            this.form.markAllAsTouched();
            return;
        }

        this.apiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.worklistService.create(this.form.getRawValue() as WorklistTaskCreate).subscribe({
            next: () => {
                this.form.markAsPristine();
                void this.router.navigate(['/worklist']);
                this.notificationService.success('worklist.task.create.success');

                if (this.form.value.worklistTaskTypeId === this.configsService.frontendConfig.entityIds.worklistTaskType.generalElectionStartId) {
                    this.generalElectionService.isGeneralElectionVisible.set(true);
                }
            },
            error: () => this.notificationService.error('worklist.task.create.error'),
        });
    }

    close() {
        void this.router.navigate(['/worklist']);
    }

    private buildForm() {
        const form = this.formBuilder.group({
            worklistTaskTypeId: new FormControl('', Validators.required),
            description: new FormControl(''),
            dueDate: new FormControl<Date | undefined>(undefined, Validators.required),
            assignedToId: new FormControl<string | undefined>(undefined),
        });

        form.controls.assignedToId.setValidators(
            conditionalValidator(
                () => form.controls.worklistTaskTypeId.value === this.configsService.frontendConfig.entityIds.worklistTaskType.generalElectionStartId,
                Validators.required
            )
        );

        return form;
    }
}
