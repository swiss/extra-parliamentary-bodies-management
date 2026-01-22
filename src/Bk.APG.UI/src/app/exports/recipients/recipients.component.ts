import {CommonModule, NgTemplateOutlet} from '@angular/common';
import {Component, computed, effect, Inject, signal, DOCUMENT} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton, MatButtonModule} from '@angular/material/button';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatFormField, MatInputModule} from '@angular/material/input';
import {MatLabel, MatSelectModule} from '@angular/material/select';
import {MatCell, MatCellDef, MatColumnDef, MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef, MatTableDataSource, MatTableModule} from '@angular/material/table';
import {ActivatedRoute} from '@angular/router';
import {GeneralElectionCommitteeList} from '@api/GeneralElectionCommitteeList';
import {RecipientsFilterForm} from '@api/RecipientsFilterForm';
import {RecipientsFilterParameters} from '@api/RecipientsFilterParameters';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService, WINDOW} from '@oblique/oblique';
import {today} from '@shared/date-util';
import {downloadFileFromHttpResponse} from '@shared/file-util';
import {MasterDataService} from '@shared/master-data.service';
import {combineLatest, defer, distinctUntilChanged, pairwise, startWith, Subject, switchMap} from 'rxjs';
import {GeneralElectionCommitteesService} from 'src/app/general-election/ge-committees/ge-committees.service';
import {AuthService} from '../../auth/auth.service';
import {RecipientsService} from './recipients.service';

@Component({
    selector: 'apg-recipients',
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
        MatButtonModule,
        MatButton,
        MatLabel,
        ObButtonDirective,
        MatCheckboxModule,
        NgTemplateOutlet,
        CommonModule,
    ],
    templateUrl: './recipients.component.html',
    styleUrl: './recipients.component.scss',
})
export class RecipientsComponent {
    readonly displayedCommitteeColumns: string[] = ['select', 'description'];

    readonly form = this.setupRequestsAndReportsForm();
    readonly departmentOffices = computed(() => {
        const offices = this.masterDataService.permittedOffices();
        const selectedDepartmentIds = this.selectedDepartmentIds();
        return selectedDepartmentIds?.length ? offices.filter(office => selectedDepartmentIds.includes(office.departmentId)) : offices;
    });
    readonly termDates = computed(() => this.masterDataService.termDates());
    dataSource = new MatTableDataSource<GeneralElectionCommitteeList>();
    selectedItems = new Set<GeneralElectionCommitteeList>();
    readonly totalCount = signal(0);

    readonly reload$ = new Subject<void>();

    filterValue: RecipientsFilterParameters = {};
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
        private readonly recipientsService: RecipientsService,
        private readonly generalElectionCommitteesService: GeneralElectionCommitteesService,
        private readonly translateService: TranslateService,
        private readonly authService: AuthService,

        @Inject(WINDOW) private readonly window: Window,
        @Inject(DOCUMENT) private readonly document: Document
    ) {
        const routeData = this.route.snapshot.data;
        if (routeData.isGeneralElection) {
            this.isGeneralElection = true;
        }
        // this.form.controls.documentType.valueChanges.pipe(takeUntilDestroyed()).subscribe(_ => this.updateAnalysisDateFields());

        // this.subscribeToFilterChanges();

        console.log(this.form.value);

        if (this.isGeneralElection) {
            this.form.valueChanges.pipe(startWith(this.form.value), pairwise(), takeUntilDestroyed()).subscribe(([prev, curr]) => {
                this.onFilter({...curr} as RecipientsFilterParameters);
            });

            combineLatest([
                this.reload$,
                this.translateService.onLangChange.pipe(
                    startWith({lang: this.translateService.currentLang}),
                    distinctUntilChanged((prev, curr) => prev.lang === curr.lang)
                ),
            ])
                .pipe(
                    // TODO GEW Committees

                    switchMap(() => this.generalElectionCommitteesService.getGeneralElectionCommitteeListForExport(this.filterValue)),
                    takeUntilDestroyed()
                )
                .subscribe(result => {
                    this.dataSource.data = result;
                    this.toggleAllCommittees(true);
                });
            this.reload$.next();
        }

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

            return this.recipientsService.generateReport({
                ...this.form.getRawValue(),
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

    onFilter(searchQuery: RecipientsFilterParameters) {
        this.filterValue = searchQuery;
        this.reload$.next();
    }

    toggleRow(row: GeneralElectionCommitteeList) {
        if (this.selectedItems.has(row)) {
            this.selectedItems.delete(row);
        } else {
            this.selectedItems.add(row);
        }
    }

    isSelected(row: GeneralElectionCommitteeList): boolean {
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

    private setupRequestsAndReportsForm(): FormGroup<RecipientsFilterForm> {
        return this.fb.group<RecipientsFilterForm>({
            departments: this.fb.control<string[] | null>(null),
            offices: this.fb.control<string[] | null>(null),
            committeeTypes: this.fb.control<string[] | null>(null),
            correspondenceLanguages: this.fb.control<string[] | null>(null),
            electionTypes: this.fb.control<string[] | null>(null),
        });
    }

    // private subscribeToFilterChanges() {
    //     this.form.valueChanges.pipe(startWith(this.form.value), pairwise(), takeUntilDestroyed()).subscribe(([_, value]) => {
    //         this.generalElectionCommitteesService.getGeneralElectionCommitteeListForExport(value);
    //     });
    // }
}
