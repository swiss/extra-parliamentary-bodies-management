import {CommonModule, NgTemplateOutlet} from '@angular/common';
import {Component, computed, effect, Inject, signal, DOCUMENT} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton, MatButtonModule} from '@angular/material/button';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatInputModule} from '@angular/material/input';
import {MatLabel, MatSelectModule} from '@angular/material/select';
import {MatCell, MatCellDef, MatColumnDef, MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef, MatTableDataSource, MatTableModule} from '@angular/material/table';
import {ActivatedRoute} from '@angular/router';
import {CommitteeList} from '@api/CommitteeList';
import {RequestsAndReportsFilterForm} from '@api/RequestsAndReportsFilterForm';
import {RequestsAndReportsFilterParameters} from '@api/RequestsAndReportsFilterParameters';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService, WINDOW} from '@oblique/oblique';
import {today} from '@shared/date-util';
import {downloadFileFromHttpResponse} from '@shared/file-util';
import {MasterDataService} from '@shared/master-data.service';
import {combineLatest, defer, distinctUntilChanged, pairwise, startWith, Subject, switchMap} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {CommitteesService} from '../../committees/committees.service';
import {ReportType} from '../ReportType';
import {RequestsAndReportsService} from './requests-and-reports.service';

@Component({
    selector: 'apg-requests-and-reports',
    imports: [
        MatFormField,
        MatSelectModule,
        MatTableModule,
        MatHeaderRowDef,
        MatHeaderRow,
        MatInputModule,
        ReactiveFormsModule,
        MatColumnDef,
        MatCellDef,
        MatRowDef,
        MatRow,
        MatCell,
        TranslatePipe,
        MatDatepickerInput,
        MatDatepicker,
        MatDatepickerToggle,
        MatButtonModule,
        MatButton,
        MatLabel,
        ObButtonDirective,
        MatCheckboxModule,
        NgTemplateOutlet,
        CommonModule,
    ],
    templateUrl: './requests-and-reports.component.html',
    styleUrl: './requests-and-reports.component.scss',
})
export class RequestsAndReportsComponent {
    readonly displayedCommitteeColumns: string[] = ['select', 'description'];
    ReportType = ReportType;

    readonly form = this.setupRequestsAndReportsForm();
    readonly departmentOffices = computed(() => {
        const offices = this.masterDataService.permittedOffices();
        const selectedDepartmentIds = this.selectedDepartmentIds();
        return selectedDepartmentIds?.length ? offices.filter(office => selectedDepartmentIds.includes(office.departmentId)) : offices;
    });
    readonly termDates = computed(() => this.masterDataService.termDates());
    dataSource = new MatTableDataSource<CommitteeList>();
    selectedItems = new Set<CommitteeList>();
    readonly totalCount = signal(0);

    readonly reload$ = new Subject<void>();

    filterValue: RequestsAndReportsFilterParameters = {};
    isGeneralElection = false;
    analysisDateDefaultValue = today();

    protected readonly isAdmin = toSignal(this.authService.isAdmin$, {initialValue: false});
    protected readonly isDepartment = toSignal(this.authService.isDepartmentUser$, {initialValue: false});
    protected readonly isSecretariat = toSignal(this.authService.isSecretariatUser$, {initialValue: false});
    protected readonly isOffice = toSignal(this.authService.isOfficeUser$, {initialValue: false});
    protected readonly isObserver = toSignal(this.authService.isObserver$, {initialValue: false});

    private readonly selectedDepartmentIds = toSignal(this.form.controls.departments.valueChanges, {initialValue: [] as string[]});

    constructor(
        private readonly route: ActivatedRoute,
        private readonly fb: FormBuilder,
        private readonly interceptorEvents: ObHttpApiInterceptorEvents,
        protected readonly masterDataService: MasterDataService,
        private readonly notificationService: ObNotificationService,
        private readonly requestsAndReportsService: RequestsAndReportsService,
        private readonly committeesService: CommitteesService,
        private readonly translateService: TranslateService,
        private readonly authService: AuthService,

        @Inject(WINDOW) private readonly window: Window,
        @Inject(DOCUMENT) private readonly document: Document
    ) {
        const routeData = this.route.snapshot.data;
        if (routeData.isGeneralElection) {
            this.isGeneralElection = true;
        }
        this.form.controls.documentType.valueChanges.pipe(takeUntilDestroyed()).subscribe(_ => this.updateAnalysisDateFields());

        if (!this.isGeneralElection) {
            this.form.valueChanges.pipe(startWith(this.form.value), pairwise(), takeUntilDestroyed()).subscribe(([_, curr]) => {
                this.onFilter({...curr});
            });

            combineLatest([
                this.reload$,
                this.translateService.onLangChange.pipe(
                    startWith({lang: this.translateService.getCurrentLang()}),
                    distinctUntilChanged((prev, curr) => prev.lang === curr.lang)
                ),
            ])
                .pipe(
                    switchMap(() => this.committeesService.getCommitteeListForExport(this.filterValue)),
                    takeUntilDestroyed()
                )
                .subscribe(result => {
                    this.dataSource.data = result;
                    this.toggleAllCommittees(true);
                });
            this.reload$.next();
        }
        effect(() => {
            const data = this.termDates();
            if (data.length > 0) {
                if (!this.isGeneralElection) {
                    this.analysisDateDefaultValue =
                        this.termDates().find(y => new Date(y.beginDate) <= today() && new Date(y.endDate!) >= today())?.beginDate ?? today();
                } else {
                    this.analysisDateDefaultValue = this.termDates().find(y => y.isGeneralElection)?.beginDate ?? today();
                    this.form.controls.analysisDate1.setValue(this.analysisDateDefaultValue);
                }
            }
        });

        effect(() => {
            this.form.controls.offices.disable();
            this.form.controls.departments.disable();

            if (this.isAdmin() || this.isDepartment()) {
                this.form.controls.offices.enable();

                if (this.isAdmin()) {
                    this.form.controls.departments.enable();
                }
            }
        });
    }
    generateReport(): void {
        const fallbackFilename = `export.xlsx`;

        defer(() => {
            this.interceptorEvents.deactivateSpinnerOnNextAPICalls(1);
            this.interceptorEvents.deactivateNotificationOnNextAPICalls(1);

            this.notificationService.info({
                title: 'requestsAndReports.download.title',
                message: 'requestsAndReports.download.message',
                timeout: 10000,
            });

            return this.requestsAndReportsService.generateReport({
                ...this.form.getRawValue(),
                isGeneralElection: this.isGeneralElection,
                committees: Array.from(this.selectedItems).map(y => y.id),
            });
        }).subscribe({
            next: response => {
                this.notificationService.success({
                    message: 'requestsAndReports.download.success',
                });

                downloadFileFromHttpResponse(response, fallbackFilename, this.window, this.document);
            },
            error: () => {
                this.notificationService.error({
                    message: 'requestsAndReports.download.error',
                });
            },
        });
    }

    onFilter(searchQuery: RequestsAndReportsFilterParameters) {
        this.filterValue = {...searchQuery};
        this.reload$.next();
    }

    toggleRow(row: CommitteeList) {
        if (this.selectedItems.has(row)) {
            this.selectedItems.delete(row);
        } else {
            this.selectedItems.add(row);
        }
    }

    isSelected(row: CommitteeList): boolean {
        return this.selectedItems.has(row);
    }

    isAllSelected(): boolean {
        return this.dataSource.data.length > 0 && this.selectedItems.size === this.dataSource.data.length;
    }

    isSomeSelected(): boolean {
        const selectedCount = this.selectedItems.size;
        return selectedCount > 0 && selectedCount < this.dataSource.data.length;
    }

    toggleAllCommittees(isRefresh: boolean) {
        if (!this.isAllSelected() || isRefresh) {
            this.selectedItems.clear();
            this.dataSource.data.forEach(row => this.selectedItems.add(row));
        } else {
            this.selectedItems.clear();
        }
    }

    resetGE() {
        this.form.controls.documentType.setValue(null);
        this.form.controls.analysisDate1.setValue(null);
        this.form.controls.committeesWithActiveMembership.setValue(true);
        this.form.controls.releasedCommittees.setValue(true);
        this.form.controls.committeeTypes.setValue(null);
    }

    reset() {
        this.form.controls.documentType.setValue(null);
        this.form.controls.analysisDate1.setValue(null);
        this.form.controls.departments.setValue(null);
        this.form.controls.offices.setValue(null);
        this.form.controls.committeeTypes.setValue(null);
        this.form.controls.analysisDate2.setValue(null);
    }

    private updateAnalysisDateFields() {
        if (!this.isGeneralElection) {
            if ([ReportType.AppendixFederalCouncilCheck, ReportType.Vacancies].includes(this.form.controls.documentType.value!)) {
                this.form.controls.analysisDate1.setValue(today());
            }

            if (this.form.controls.documentType.value! === ReportType.CompareListGeneralElection) {
                this.form.controls.analysisDate1.setValue(this.analysisDateDefaultValue);
                this.form.controls.analysisDate2.setValue(today());
                this.form.controls.analysisDate2.enable();
            } else {
                this.form.controls.analysisDate2.setValue(null);
                this.form.controls.analysisDate2.disable();
            }
        } else {
            if (this.form.controls.documentType.value === ReportType.InformationNoteGeneralElection) {
                this.form.controls.analysisDate1.disable();
                this.form.controls.releasedCommittees.setValue(true);
                this.form.controls.releasedCommittees.disable();
            } else {
                this.form.controls.analysisDate1.enable();
                this.form.controls.releasedCommittees.enable();
            }
        }
    }

    private setupRequestsAndReportsForm(): FormGroup<RequestsAndReportsFilterForm> {
        return this.fb.group<RequestsAndReportsFilterForm>({
            documentType: this.fb.control<ReportType | null>(null, {nonNullable: true, validators: [Validators.required]}),
            analysisDate1: this.fb.control<Date | null>(today(), {nonNullable: true, validators: [Validators.required]}),
            analysisDate2: this.fb.control<Date | null>(null),
            departments: this.fb.control<string[] | null>(null),
            offices: this.fb.control<string[] | null>(null),
            committeeTypes: this.fb.control<string[] | null>(null),
            committeesWithActiveMembership: this.fb.control<boolean>(true, {nonNullable: true}),
            releasedCommittees: this.fb.control<boolean>(true, {nonNullable: true}),
        });
    }
}
