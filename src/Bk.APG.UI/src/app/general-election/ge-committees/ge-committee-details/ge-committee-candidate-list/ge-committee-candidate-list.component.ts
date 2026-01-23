import {CommonModule, DatePipe} from '@angular/common';
import {AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef, Component, computed, DOCUMENT, effect, Inject, signal, viewChild} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatCard, MatCardContent} from '@angular/material/card';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatChipsModule} from '@angular/material/chips';
import {MatDialog} from '@angular/material/dialog';
import {MatIcon} from '@angular/material/icon';
import {MatInput, MatPrefix, MatSuffix} from '@angular/material/input';
import {MatPaginator} from '@angular/material/paginator';
import {MatFormField, MatHint, MatLabel, MatOption, MatSelect} from '@angular/material/select';
import {MatSort, MatSortHeader} from '@angular/material/sort';
import {
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatFooterCell,
    MatFooterCellDef,
    MatFooterRow,
    MatFooterRowDef,
    MatHeaderCell,
    MatHeaderCellDef,
    MatHeaderRow,
    MatHeaderRowDef,
    MatNoDataRow,
    MatRow,
    MatRowDef,
    MatTable,
    MatTableDataSource,
} from '@angular/material/table';
import {MatTooltip} from '@angular/material/tooltip';
import {ActivatedRoute, Router} from '@angular/router';
import {CandidateListDuplicateCheckResult} from '@api/CandidateListDuplicateCheckResult';
import {DuplicateReason} from '@api/DuplicateReason';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {MembershipCandidateCreate} from '@api/MembershipCandidateCreate';
import {MembershipCandidateDetail} from '@api/MembershipCandidateDetail';
import {MembershipCandidatePartialUpdate} from '@api/MembershipCandidatePartialUpdate';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertComponent, ObButtonDirective, ObHttpApiInterceptorEvents, ObNotificationService, WINDOW} from '@oblique/oblique';
import {downloadFileFromHttpResponse} from '@shared/file-util';
import {MasterDataService} from '@shared/master-data.service';
import {MembersQuotasComponent} from '@shared/members-quotas/members-quotas.component';
import {distinctUntilChanged, EMPTY, merge, switchMap} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {ConfigsService} from '../../../../configs.service';
import {PersonSearchComponent} from '../../../../persons/shared/person-search/person-search.component';
import {GeneralElectionCommitteesService} from '../../ge-committees.service';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';
import {CandidateListForwardDialogComponent} from './candidate-list-forward-dialog/candidate-list-forward-dialog.component';
import {GeneralElectionCommitteeCandidateListService} from './ge-committee-candidate-list.service';

export type MembershipCandidateColumns =
    | 'select'
    | 'surname'
    | 'givenName'
    | 'gender'
    | 'language'
    | 'birthYear'
    | 'function'
    | 'beginDate'
    | 'endDate'
    | 'electionType'
    | 'membershipAddition'
    | 'remarks'
    | 'remarksStatus'
    | 'actions';

@Component({
    selector: 'apg-ge-committee-candidate-list',
    templateUrl: './ge-committee-candidate-list.component.html',
    styleUrls: ['./ge-committee-candidate-list.component.scss'],
    imports: [
        MatTable,
        MatSort,
        MatSortHeader,
        MatSelect,
        MatIcon,
        MatIconButton,
        MatTooltip,
        MatFormField,
        MatLabel,
        MatInput,
        MatOption,
        MatHint,
        MatPaginator,
        MatColumnDef,
        MatCellDef,
        MatCell,
        MatFooterCellDef,
        MatFooterCell,
        MatHeaderCell,
        MatHeaderCellDef,
        MatFooterRowDef,
        MatFooterRow,
        MatNoDataRow,
        MatTooltip,
        MatCheckbox,
        ObButtonDirective,
        ReactiveFormsModule,
        TranslatePipe,
        DatePipe,
        PersonSearchComponent,
        MatButton,
        ObAlertComponent,
        MatHeaderRowDef,
        MatRowDef,
        MatHeaderRow,
        MatRow,
        MatSuffix,
        MatPrefix,
        MembersQuotasComponent,
        MatChipsModule,
        MatCardContent,
        MatCard,
        FormsModule,
        CommonModule,
    ],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GeneralElectionCommitteeCandidateListComponent implements AfterViewInit {
    generalElectionCommittee = signal<GeneralElectionCommitteeDetails | undefined>(undefined);
    validationErrors = signal<string[]>([]);
    areJustificationsMissing = signal(false);
    dataSource = new MatTableDataSource<MembershipCandidateDetail>();
    DuplicateReason = DuplicateReason;
    readonly pageSizeOptions = [25, 50, 100];

    readonly sort = viewChild(MatSort);
    readonly paginator = viewChild(MatPaginator);

    membershipCandidateForms = new Map<string, FormGroup>(); // membershipCandidateId -> FormGroup
    newMembershipCandidateForm: FormGroup;
    vacanciesForm: FormGroup;
    personDuplicates: CandidateListDuplicateCheckResult[] = [];
    createdPersons: PersonDetails[] = [];
    existingPersons: PersonDetails[] = [];
    validated = false;

    selectedPerson = signal<PersonDetails | undefined>(undefined);
    allSelected = false;
    selectedIds: string[] = [];
    vacancies = computed(() =>
        this.generalElectionCommitteeDetailsService.committeeDetails()?.vacanciesGeneralElection != null
            ? this.generalElectionCommitteeDetailsService.committeeDetails()?.vacanciesGeneralElection
            : this.generalElectionCommitteeDetailsService.committeeDetails()?.calculatedVacancies
    );
    duplicateCheckConfirmed = false;
    personSearchComponent = viewChild(PersonSearchComponent);
    highlightedCandidateId: string | null = null;

    protected readonly isAdmin = toSignal(this.authService.isAdmin$, {initialValue: false});
    protected readonly isDepartment = toSignal(this.authService.isDepartmentUser$, {initialValue: false});
    protected readonly isSecretariat = toSignal(this.authService.isSecretariatUser$, {initialValue: false});
    protected readonly isOffice = toSignal(this.authService.isOfficeUser$, {initialValue: false});
    protected readonly isObserver = toSignal(this.authService.isObserver$, {initialValue: false});

    protected readonly isAllowed = computed(() => this.isAdmin() || this.isSecretariat() || this.isDepartment() || this.isOffice());

    protected readonly canEdit = computed(
        () =>
            !!this.generalElectionCommittee()?.canValidateCandidateList ||
            !!this.generalElectionCommittee()?.canForwardCandidateList ||
            !!this.generalElectionCommittee()?.canSaveCandidateList
    );

    protected readonly displayedColumnsWithPermissions = computed(() => {
        const baseColumns: MembershipCandidateColumns[] = [
            'select',
            'surname',
            'givenName',
            'gender',
            'language',
            'birthYear',
            'function',
            'beginDate',
            'endDate',
            'electionType',
            'membershipAddition',
            'remarks',
            'remarksStatus',
        ];

        return this.isAllowed() ? [...baseColumns, 'actions'] : baseColumns;
    });
    constructor(
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly translateService: TranslateService,
        private readonly membershipCandidateListService: GeneralElectionCommitteeCandidateListService,
        private readonly generalElectionCommitteesService: GeneralElectionCommitteesService,
        private readonly formBuilder: FormBuilder,
        private readonly notificationService: ObNotificationService,
        private readonly authService: AuthService,
        private readonly httpApiInterceptorEvents: ObHttpApiInterceptorEvents,
        private readonly configsService: ConfigsService,
        protected readonly generalElectionCommitteeDetailsService: GeneralElectionCommitteeDetailsService,
        protected readonly masterDataService: MasterDataService,
        private readonly dialog: MatDialog,
        private readonly cdRef: ChangeDetectorRef,
        @Inject(WINDOW) private readonly window: Window,
        @Inject(DOCUMENT) private readonly document: Document
    ) {
        this.newMembershipCandidateForm = this.formBuilder.group({
            personId: [''],
            surname: ['', Validators.required],
            givenName: ['', Validators.required],
            genderId: ['', Validators.required],
            languageId: ['', Validators.required],
            birthYear: [0, Validators.required],
            functionId: ['', Validators.required],
            remarks: [''],
            remarksStatus: [''],
        });

        this.vacanciesForm = this.formBuilder.group({
            vacancies: [],
        });

        merge(
            this.generalElectionCommitteeDetailsService.reload$,
            this.membershipCandidateListService.reload$,
            this.translateService.onLangChange.pipe(distinctUntilChanged((prev, curr) => prev.lang === curr.lang))
        )
            .pipe(
                switchMap(() => this.generalElectionCommitteeDetailsService.generalElectionCommitteeDetails(this.route.snapshot.params.id)),
                takeUntilDestroyed()
            )
            .subscribe(generalElectionCommittee => {
                this.dataSource.data = generalElectionCommittee.candidates ?? [];
                this.initializeForms(generalElectionCommittee.candidates ?? []);
                this.selectedIds = (generalElectionCommittee.candidates ?? []).filter(x => x.isSelected).map(x => x.id);
                this.allSelected = false;
                this.validationErrors.set([]);
                this.generalElectionCommittee.set(generalElectionCommittee);
            });

        const effectRef = effect(() => {
            if (this.vacancies()) {
                this.vacanciesForm.controls.vacancies.setValue(this.vacancies());
                effectRef.destroy();
            }
        });

        effect(() => {
            const person = this.selectedPerson();
            if (person) {
                if (this.dataSource.data.some(membershipCandidate => membershipCandidate.personId === person.id)) {
                    this.notificationService.error('generalElection.membershipCandidate.create.duplicatePerson');
                    return;
                }

                this.newMembershipCandidateForm.patchValue({...person, personId: person.id});

                this.newMembershipCandidateForm.controls.surname.disable();
                this.newMembershipCandidateForm.controls.givenName.disable();
                this.newMembershipCandidateForm.controls.genderId.disable();
                this.newMembershipCandidateForm.controls.languageId.disable();
                this.newMembershipCandidateForm.controls.birthYear.disable();
            } else {
                this.newMembershipCandidateForm.reset();

                this.newMembershipCandidateForm.controls.surname.enable();
                this.newMembershipCandidateForm.controls.givenName.enable();
                this.newMembershipCandidateForm.controls.genderId.enable();
                this.newMembershipCandidateForm.controls.languageId.enable();
                this.newMembershipCandidateForm.controls.birthYear.enable();
            }
        });

        this.disableFormsIfCantEdit();
    }

    ngAfterViewInit(): void {
        this.dataSource.sort = this.sort() ?? null;
        this.dataSource.paginator = this.paginator() ?? null;
    }

    getMembershipCandidateForm(membershipCandidateId: string): FormGroup {
        if (!membershipCandidateId) {
            return this.newMembershipCandidateForm;
        }
        return this.membershipCandidateForms.get(membershipCandidateId)!;
    }

    hasUnsavedFormChanges(membershipCandidateId: string): boolean {
        const form = this.getMembershipCandidateForm(membershipCandidateId);
        return form ? form.dirty : false;
    }

    saveMembershipCandidate(element: MembershipCandidateDetail): void {
        const form = this.membershipCandidateForms.get(element.id);
        if (!form?.dirty) {
            return;
        }

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        const membershipCandidatePartialUpdate = {...form.value} as MembershipCandidatePartialUpdate;
        this.membershipCandidateListService.partialUpdateMembershipCandidate(element.id, membershipCandidatePartialUpdate).subscribe({
            next: () => {
                form.markAsPristine();
                this.notificationService.success('generalElection.membershipCandidate.saveChanges.success');
            },
            error: () => this.notificationService.error('generalElection.membershipCandidate.saveChanges.error'),
        });
    }

    editMembershipCandidate(membershipCandidateId: string) {
        void this.router.navigate(['general-election', 'committees', this.route.snapshot.params.id, 'membership-candidate', membershipCandidateId]);
    }

    deleteMembershipCandidate(membershipCandidateId: string): void {
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.membershipCandidateListService.deleteMembershipCandidate(membershipCandidateId).subscribe({
            next: () => {
                this.notificationService.success('generalElection.membershipCandidate.delete.success');

                const currentData = this.dataSource.data;
                this.dataSource.data = currentData.filter(item => item.id !== membershipCandidateId);
            },
            error: () => this.notificationService.error('generalElection.membershipCandidate.delete.error'),
        });
    }

    saveNewMembershipCandidate(): void {
        if (!this.newMembershipCandidateForm.dirty || !this.newMembershipCandidateForm.valid) {
            return;
        }

        const membershipCandidateCreate = {
            ...this.newMembershipCandidateForm.getRawValue(),
            committeeId: this.route.snapshot.params.id,
        } as MembershipCandidateCreate;

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();
        this.membershipCandidateListService
            .getDuplicateMembershipCandidate(membershipCandidateCreate)
            .pipe(
                switchMap(duplicateCandidate => {
                    if (duplicateCandidate) {
                        this.highlightedCandidateId = duplicateCandidate.id;
                        this.cdRef.detectChanges();
                        this.notificationService.error('generalElection.membershipCandidate.create.duplicatePerson');
                        return EMPTY;
                    }
                    this.highlightedCandidateId = null;
                    return this.membershipCandidateListService.createMembershipCandidate(membershipCandidateCreate);
                })
            )
            .subscribe({
                next: (newMembershipCandidate: MembershipCandidateDetail) => {
                    const currentData = this.dataSource.data;
                    this.dataSource.data = [...currentData, newMembershipCandidate];

                    this.initializeFormForCandidate(newMembershipCandidate);

                    this.selectedPerson.set(undefined);
                    this.personSearchComponent()?.reset();

                    this.notificationService.success('generalElection.membershipCandidate.create.success');
                },
                error: () => {
                    this.notificationService.error('generalElection.membershipCandidate.create.error');
                },
            });
    }

    isSelected(id: string): boolean {
        return this.selectedIds.includes(id);
    }

    toggleSelection(id: string): void {
        const index = this.selectedIds.indexOf(id);
        if (index > -1) {
            this.selectedIds.splice(index, 1);
        } else {
            this.selectedIds.push(id);
        }
        this.updateAllSelectedState();
    }

    toggleSelectAll(): void {
        if (this.allSelected) {
            this.selectedIds = [];
            this.allSelected = false;
        } else {
            this.selectedIds = [...this.dataSource.data.map(item => item.id)];
            this.allSelected = true;
        }
    }

    isPartiallySelected(): boolean {
        return this.selectedIds.length > 0 && this.selectedIds.length < this.dataSource.data.length;
    }

    openForwardDialog() {
        this.dialog.open(CandidateListForwardDialogComponent, {data: {committeeId: this.route.snapshot.params.id, candidateIds: this.selectedIds}});
    }

    downloadCandidateList() {
        const fallbackFilename = `export.xlsx`;

        this.httpApiInterceptorEvents.deactivateSpinnerOnNextAPICalls(1);
        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls(1);

        this.notificationService.info({
            title: 'generalElection.candidateList.export.title',
            message: 'generalElection.candidateList.export.message',
            timeout: 10000,
        });

        return this.membershipCandidateListService.generateExport(this.route.snapshot.params.id, this.selectedIds).subscribe({
            next: response => {
                this.notificationService.success({
                    message: 'generalElection.candidateList.export.success',
                });

                downloadFileFromHttpResponse(response, fallbackFilename, this.window, this.document);
            },
            error: () => {
                this.notificationService.error({
                    message: 'generalElection.candidateList.export.error',
                });
            },
        });
    }

    saveSelectedIds() {
        this.membershipCandidateListService.saveCandidateList(this.route.snapshot.params.id, this.selectedIds).subscribe({
            next: () => {
                this.membershipCandidateListService.reload$.next();
                this.notificationService.success('generalElection.candidateList.save.success');
            },
            error: () => this.notificationService.error('generalElection.candidateList.save.error'),
        });
    }

    validateCandidateList() {
        this.membershipCandidateListService.validateCandidateList(this.route.snapshot.params.id, this.selectedIds, this.duplicateCheckConfirmed).subscribe({
            next: validationResult => {
                this.validated = true;
                this.validationErrors.set(validationResult.errors);
                this.areJustificationsMissing.set(validationResult.areJustificationsMissing);
                this.personDuplicates = validationResult.duplicateCheckResults.filter(y => y.duplicateReason !== DuplicateReason.NoDuplicateFound);
                this.existingPersons = validationResult.existingPersons;
                if (validationResult.isValid && this.personDuplicates.length === 0) {
                    this.generalElectionCommitteeDetailsService.reload$.next();
                    this.createdPersons = validationResult.createdPersons;
                    this.notificationService.success({message: 'generalElection.candidateList.validate.success', timeout: 10000});
                } else {
                    if (!validationResult.isValid) {
                        this.notificationService.error('generalElection.candidateList.validate.error');
                    }
                }
            },
            error: () => this.notificationService.error('generalElection.candidateList.validate.error'),
        });
    }

    clearExistingPersonList() {
        this.existingPersons = [];
    }

    clearCreatedPersonList() {
        this.createdPersons = [];
    }

    saveVacancies() {
        const newVacancies = this.vacanciesForm.controls.vacancies.value;

        this.httpApiInterceptorEvents.deactivateNotificationOnNextAPICalls();

        this.generalElectionCommitteesService.updateGeneralElectionCommitteeVacancies(this.route.snapshot.params.id, newVacancies).subscribe({
            next: async () => {
                this.generalElectionCommitteeDetailsService.reload$.next();
                await this.router.navigate([]);
                return this.notificationService.success('committee.details.vacancies.success');
            },
            error: () => this.notificationService.error('committee.details.vacancies.error'),
        });
    }

    protected isNewElection(membershipCandidate: MembershipCandidateDetail) {
        return membershipCandidate.electionTypeId === this.configsService.frontendConfig.entityIds.electionType.newElectionId;
    }

    private updateAllSelectedState(): void {
        this.allSelected = this.selectedIds.length === this.dataSource.data.length && this.dataSource.data.length > 0;
    }

    private initializeFormForCandidate(membershipCandidate: MembershipCandidateDetail): void {
        const form = this.formBuilder.group({
            functionId: [membershipCandidate.functionId],
            remarks: [membershipCandidate.remarks],
            remarksStatus: [membershipCandidate.remarksStatus],
        });

        this.membershipCandidateForms.set(membershipCandidate.id, form);
    }

    private initializeForms(candidates: MembershipCandidateDetail[]): void {
        this.membershipCandidateForms.clear();
        candidates.forEach(membershipCandidate => this.initializeFormForCandidate(membershipCandidate));
    }

    private disableFormsIfCantEdit() {
        effect(() => {
            this.generalElectionCommittee();
            if (this.isAllowed() && this.canEdit()) {
                this.membershipCandidateForms.forEach(form => form.enable());
                this.newMembershipCandidateForm?.enable();
                this.personSearchComponent()?.personSearchForm.enable();
            } else {
                this.membershipCandidateForms.forEach(form => form.disable());
                this.newMembershipCandidateForm?.disable();
                this.personSearchComponent()?.personSearchForm.disable();
            }
        });
    }
}
