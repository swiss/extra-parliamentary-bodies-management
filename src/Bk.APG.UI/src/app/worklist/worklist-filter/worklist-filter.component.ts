import {Component, computed, output} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatOption} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatIconModule} from '@angular/material/icon';
import {MatInput, MatInputModule} from '@angular/material/input';
import {MatFormField, MatLabel, MatSelect} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {Router} from '@angular/router';
import {WorklistFilterForm} from '@api/WorklistFilterForm';
import {WorklistFilterParameters} from '@api/WorklistFilterParameters';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObInputClearDirective} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {worklistSearchStorageKey} from '@shared/storage-keys';
import {debounce, pairwise, startWith, Subject, timer} from 'rxjs';
import {AuthService} from '../../auth/auth.service';

@Component({
    selector: 'apg-worklist-filter',
    imports: [
        ReactiveFormsModule,
        MatLabel,
        MatFormField,
        TranslatePipe,
        MatIconModule,
        MatOption,
        MatInput,
        MatInputModule,
        MatSelect,
        MatButton,
        ObButtonDirective,
        TranslatePipe,
        ObInputClearDirective,
        MatDatepickerModule,
        MatTooltip,
        HelpTooltipComponent,
    ],
    templateUrl: './worklist-filter.component.html',
    styleUrl: './worklist-filter.component.scss',
})
export class WorklistFilterComponent {
    readonly reload$ = new Subject<void>();
    readonly filter = output<WorklistFilterParameters>();
    readonly form = this.setupWorklistFilterForm();
    readonly departmentOffices = computed(() => {
        const offices = this.masterDataService.permittedOffices();
        const selectedDepartmentIds = this.selectedDepartmentIds();
        return selectedDepartmentIds?.length ? offices.filter(office => selectedDepartmentIds.includes(office.departmentId)) : offices;
    });
    readonly worklistTaskTypes = computed(() => {
        return this.masterDataService.worklistTaskTypes();
    });
    readonly worklistTaskStates = computed(() => {
        return this.masterDataService.worklistTaskStates();
    });
    protected isAdmin = toSignal(this.authService.isAdmin$);
    protected isDepartmentUser = toSignal(this.authService.isDepartmentUser$);

    private readonly selectedDepartmentIds = toSignal(this.form.controls.departments.valueChanges, {initialValue: [] as string[]});

    constructor(
        private readonly fb: FormBuilder,
        private readonly router: Router,
        private readonly authService: AuthService,
        protected readonly masterDataService: MasterDataService,
        private readonly searchStorageService: SearchStorageService
    ) {
        this.subscribeToFilterChanges();
        this.setInitialFilterValuesToEmit();
    }

    reset(): void {
        this.searchStorageService.removeParams(worklistSearchStorageKey);
        this.form.reset();
    }

    createTask() {
        void this.router.navigate(['worklist/create'], {relativeTo: this.router.routerState.root}).then();
    }

    clearCommittee(): void {
        this.form.controls.committee.patchValue(null);
    }

    private subscribeToFilterChanges() {
        this.form.valueChanges
            .pipe(
                startWith(this.form.value),
                pairwise(),
                debounce(([prev, curr]) => (prev.committee !== curr.committee ? timer(300) : timer(0))),
                takeUntilDestroyed()
            )
            .subscribe(([_, value]) => {
                this.filter.emit({...value} as WorklistFilterParameters);
                this.searchStorageService.setParams(worklistSearchStorageKey, value);
            });
    }

    private setupWorklistFilterForm(): FormGroup<WorklistFilterForm> {
        return this.fb.group<WorklistFilterForm>({
            committee: this.fb.control<string | null>(null),
            departments: this.fb.control<string[] | null>(null),
            offices: this.fb.control<string[] | null>(null),
            worklistTaskTypes: this.fb.control<string[] | null>(null),
            worklistTaskStates: this.fb.control<string[] | null>(null),
            assignedBy: this.fb.control<string | null>(null),
            assignedTo: this.fb.control<string | null>(null),
            createdFrom: this.fb.control<Date | null>(null),
            createdTo: this.fb.control<Date | null>(null),
            dueDateFrom: this.fb.control<Date | null>(null),
            dueDateTo: this.fb.control<Date | null>(null),
        });
    }

    private setInitialFilterValuesToEmit() {
        const params = this.searchStorageService.getParams(worklistSearchStorageKey) ?? {};
        this.form.patchValue(params);
    }
}
