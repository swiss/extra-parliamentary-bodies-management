import {Component, computed, inject, output, Signal} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {Router} from '@angular/router';
import {CommitteeFilterForm} from '@api/CommitteeFilterForm';
import {CommitteeFilterParameters} from '@api/CommitteeFilterParameters';
import {CommitteeType} from '@api/CommitteeType';
import {Department} from '@api/Department';
import {Level} from '@api/Level';
import {Term} from '@api/Term';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObInputClearDirective} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {committeeSearchStorageKey} from '@shared/storage-keys';
import {debounce, pairwise, startWith, timer} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {Role} from '../../auth/Role';

@Component({
    selector: 'apg-committees-filter',
    templateUrl: './committees-filter.component.html',
    styleUrl: './committees-filter.component.scss',
    imports: [
        ReactiveFormsModule,
        MatFormField,
        MatLabel,
        MatInput,
        MatSelect,
        MatOption,
        MatButton,
        ObButtonDirective,
        MatIcon,
        TranslatePipe,
        ObInputClearDirective,
        MatTooltip,
        HelpTooltipComponent,
    ],
})
export class CommitteesFilterComponent {
    readonly filter = output<CommitteeFilterParameters>();
    canCreate = false;
    readonly form: FormGroup<CommitteeFilterForm>;
    formFilter?: CommitteeFilterParameters;

    readonly departmentOffices = computed(() => {
        const offices = this.masterDataService.offices();
        const selectedDepartmentIds = this.selectedDepartmentIds();
        return selectedDepartmentIds?.length ? offices.filter(office => selectedDepartmentIds.includes(office.departmentId)) : offices;
    });

    protected readonly levels: Signal<Level[]>;
    protected readonly departments: Signal<Department[]>;
    protected readonly committeeTypes: Signal<CommitteeType[]>;
    protected readonly terms: Signal<Term[]>;

    private readonly selectedDepartmentIds: Signal<string[] | null>;

    private readonly fb = inject(FormBuilder);
    private readonly router = inject(Router);
    private readonly authService = inject(AuthService);
    private readonly masterDataService = inject(MasterDataService);
    private readonly searchStorageService = inject(SearchStorageService);

    constructor() {
        this.levels = this.masterDataService.levels;
        this.departments = this.masterDataService.departments;
        this.committeeTypes = this.masterDataService.committeeTypes;
        this.terms = this.masterDataService.terms;

        this.form = this.setupCommitteeFilterForm();

        this.selectedDepartmentIds = toSignal(this.form.controls.departments.valueChanges, {initialValue: [] as string[]});

        this.subscribeToFilterChanges();
        this.setInitialFilterValuesToEmit();

        this.authService.roles$.subscribe(roles => {
            if (roles.includes(Role.Admin) || roles.includes(Role.Department)) {
                this.canCreate = true;
            }
        });
    }

    reset(): void {
        this.formFilter = undefined;
        this.searchStorageService.removeParams(committeeSearchStorageKey);
        this.form.reset();
        this.setInitialFilterValuesToEmit();
    }

    create(): void {
        void this.router.navigate(['committees/create']);
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
                this.filter.emit({...value} as CommitteeFilterParameters);
                this.searchStorageService.setParams(committeeSearchStorageKey, value);
            });
    }

    private setInitialFilterValuesToEmit() {
        this.form.patchValue({isActive: [true]});

        if (this.formFilter) {
            this.form.patchValue(this.formFilter);
        }
    }

    private setupCommitteeFilterForm(): FormGroup<CommitteeFilterForm> {
        const params = this.searchStorageService.getParams(committeeSearchStorageKey);

        const formGroup = this.fb.group<CommitteeFilterForm>({
            freeText: this.fb.control<string | null>(null),
            levels: this.fb.control<string[] | null>(null),
            departments: this.fb.control<string[] | null>(null),
            offices: this.fb.control<string[] | null>(null),
            committeeTypes: this.fb.control<string[] | null>(null),
            terms: this.fb.control<string[] | null>(null),
            isActive: this.fb.control<boolean[] | null>(null),
            isMarketOrientated: this.fb.control<boolean[] | null>(null),
            hasSupervisionDuty: this.fb.control<boolean[] | null>(null),
        });

        if (params) {
            this.formFilter = params;
        }
        return formGroup;
    }
}
