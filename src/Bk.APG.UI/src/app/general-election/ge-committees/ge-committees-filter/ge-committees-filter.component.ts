import {Component, computed, output} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {GeneralElectionCommitteeFilterForm} from '@api/GeneralElectionCommitteeFilterForm';
import {GeneralElectionCommitteeFilterParameters} from '@api/GeneralElectionCommitteeFilterParameters';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObInputClearDirective} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {EiamAssignmentService} from '@shared/services/eiam-assignment.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {geCommitteeSearchStorageKey} from '@shared/storage-keys';
import {debounce, pairwise, startWith, timer} from 'rxjs';
import {AuthService} from '../../../auth/auth.service';

@Component({
    selector: 'apg-ge-committees-filter',
    templateUrl: './ge-committees-filter.component.html',
    styleUrl: './ge-committees-filter.component.scss',
    imports: [
        ReactiveFormsModule,
        MatFormField,
        MatLabel,
        MatInput,
        MatSelect,
        MatOption,
        MatButton,
        ObButtonDirective,
        TranslatePipe,
        ObInputClearDirective,
        MatIcon,
        HelpTooltipComponent,
    ],
})
export class GeneralElectionCommitteesFilterComponent {
    readonly filter = output<GeneralElectionCommitteeFilterParameters>();
    readonly form = this.setupCommitteeFilterForm();
    formFilter?: GeneralElectionCommitteeFilterParameters;

    readonly departmentOffices = computed(() => {
        const offices = this.masterDataService.offices();
        const selectedDepartmentIds = this.selectedDepartmentIds();
        const isDepartmentUser = this.isDepartmentUser();

        if (isDepartmentUser) {
            const departmentId = this.userAssignment()?.departmentId;
            return departmentId ? offices.filter(office => office.departmentId === departmentId) : [];
        }

        return selectedDepartmentIds?.length ? offices.filter(office => selectedDepartmentIds.includes(office.departmentId)) : offices;
    });
    protected isAdmin = toSignal(this.authService.isAdmin$);
    protected isDepartmentUser = toSignal(this.authService.isDepartmentUser$);

    private readonly userAssignment = toSignal(this.eiamAssignmentService.getCurrentEiamAssignment());

    private readonly selectedDepartmentIds = toSignal(this.form.controls.departments.valueChanges, {initialValue: [] as string[]});

    constructor(
        private readonly fb: FormBuilder,
        private readonly authService: AuthService,
        protected readonly masterDataService: MasterDataService,
        private readonly eiamAssignmentService: EiamAssignmentService,
        private readonly searchStorageService: SearchStorageService
    ) {
        this.subscribeToFilterChanges();
        this.setInitialFilterValuesToEmit();
    }

    reset(): void {
        this.formFilter = undefined;
        this.searchStorageService.removeParams(geCommitteeSearchStorageKey);
        this.form.reset();
        this.setInitialFilterValuesToEmit();
    }

    clearFreeText(): void {
        this.form.controls.freeText.patchValue(null);
    }

    private subscribeToFilterChanges() {
        this.form.valueChanges
            .pipe(
                startWith(this.form.value),
                pairwise(),
                debounce(([prev, curr]) => (prev.freeText !== curr.freeText ? timer(300) : timer(0))),
                takeUntilDestroyed()
            )
            .subscribe(([_, value]) => {
                this.filter.emit({...value} as GeneralElectionCommitteeFilterParameters);
                this.searchStorageService.setParams(geCommitteeSearchStorageKey, value);
            });
    }

    private setInitialFilterValuesToEmit() {
        if (this.formFilter) {
            this.form.patchValue(this.formFilter);
        }
    }

    private setupCommitteeFilterForm(): FormGroup<GeneralElectionCommitteeFilterForm> {
        const params = this.searchStorageService.getParams(geCommitteeSearchStorageKey);

        const formGroup = this.fb.group<GeneralElectionCommitteeFilterForm>({
            freeText: this.fb.control<string | null>(null),
            departments: this.fb.control<string[] | null>(null),
            offices: this.fb.control<string[] | null>(null),
            committeeTypes: this.fb.control<string[] | null>(null),
            isMarketOrientated: this.fb.control<boolean[] | null>(null),
            hasSupervisionDuty: this.fb.control<boolean[] | null>(null),
            status: this.fb.control<string | null>(null),
            vacancies: this.fb.control<string | null>(null),
            statusProposal: this.fb.control<string | null>(null),
        });

        if (params) {
            this.formFilter = params;
        }
        return formGroup;
    }
}
