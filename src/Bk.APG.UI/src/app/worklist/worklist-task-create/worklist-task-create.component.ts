import {CdkTextareaAutosize} from '@angular/cdk/text-field';
import {Component, computed, effect, OnInit, signal} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormControl, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatInput, MatInputModule, MatLabel, MatSuffix} from '@angular/material/input';
import {MatError, MatOption, MatSelect} from '@angular/material/select';
import {MatCell, MatCellDef, MatColumnDef, MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef, MatTableDataSource, MatTableModule} from '@angular/material/table';
import {Router} from '@angular/router';
import {EiamAssignment} from '@api/EiamAssignment';
import {GeneralElectionCommitteeList} from '@api/GeneralElectionCommitteeList';
import {WorklistTaskCreate} from '@api/WorklistTaskCreate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
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
import {AuthService} from '../../auth/auth.service';
import {ConfigsService} from '../../configs.service';
import {GeneralElectionCommitteesService} from '../../general-election/ge-committees/ge-committees.service';
import {GeneralElectionService} from '../../general-election/general-election.service';
import {WorklistService} from '../worklist.service';
import {debounceTime} from 'rxjs';
import {MatDialog} from '@angular/material/dialog';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';

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
        MatTableModule,
        MatHeaderRowDef,
        MatHeaderRow,
        MatInputModule,
        MatColumnDef,
        MatCellDef,
        MatRowDef,
        MatRow,
        MatCell,
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
    dataSource = new MatTableDataSource<GeneralElectionCommitteeList>();
    readonly displayedCommitteeColumns: string[] = ['description'];

    baseLinkCommittee = 'general-election/committees';
    baseLinkCommitteeSuffix = '?tab=overview';
    isEndGeneralElection = false;

    readonly worklistTaskTypes = computed(() => {
        return this.masterDataService.worklistTaskTypes().filter(y => y.canBeCreatedManually);
    });
    protected readonly isAdmin = toSignal(this.authService.isAdmin$, {initialValue: false});

    constructor(
        protected readonly masterDataService: MasterDataService,
        protected readonly worklistService: WorklistService,
        protected readonly notificationService: ObNotificationService,
        protected readonly router: Router,
        protected readonly formBuilder: FormBuilder,
        protected readonly configsService: ConfigsService,
        private readonly generalElectionService: GeneralElectionService,
        private readonly generalElectionCommitteeService: GeneralElectionCommitteesService,
        private readonly translateService: TranslateService,
        private readonly dialog: MatDialog,

        protected readonly eiamAssignmentService: EiamAssignmentService,
        private readonly authService: AuthService,
        private readonly apiInterceptorEvents: ObHttpApiInterceptorEvents
    ) {
        this.form.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(() => {
            const formValuesWithNull = {...this.form.getRawValue()};

            if (formValuesWithNull.worklistTaskTypeId === configsService.frontendConfig.entityIds.worklistTaskType.generalElectionEndId) {
                this.isEndGeneralElection = true;
            }
        });

        effect(() => {
            const isAdmin = this.isAdmin();
            if (isAdmin) {
                this.generalElectionCommitteeService.getUnfinishedGeneralElectionCommitteeList().subscribe(result => {
                    this.dataSource.data = result;
                });
            }
        });
    }

    ngOnInit() {
        this.eiamAssignmentService.getAvailableEiamAssignments().subscribe(assignments => this.availableAssignments.set(assignments));
    }

    save() {
        if (!this.form.valid) {
            this.form.markAllAsTouched();
            return;
        }

        if (this.isEndGeneralElection && this.dataSource.data.length > 0) {
            const dialogRef = this.dialog.open(ConfirmDialogComponent, {
                width: '400px',
                data: {
                    title: this.translateService.instant('worklist.task.endGeneralElection.unfinished.dialogTitle'),
                    message: this.translateService.instant('worklist.task.endGeneralElection.unfinished.dialogText'),
                },
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result === true) {
                    this.worklistService.create(this.form.getRawValue() as WorklistTaskCreate).subscribe({
                        next: () => {
                            this.form.markAsPristine();
                            void this.router.navigate(['/worklist']);
                            this.notificationService.success('worklist.task.create.success');

                            if (this.form.value.worklistTaskTypeId === this.configsService.frontendConfig.entityIds.worklistTaskType.generalElectionEndId) {
                                this.generalElectionService.isGeneralElectionVisible.set(false);
                            }
                        },
                        error: () => this.notificationService.error('worklist.task.create.error'),
                    });
                }
            });
        } else {
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
