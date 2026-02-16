import {ChangeDetectionStrategy, Component, computed, EventEmitter, Output} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {Router} from '@angular/router';
import {PersonFilterForm} from '@api/PersonFilterForm';
import {PersonFilterParameters} from '@api/PersonFilterParameters';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObInputClearDirective} from '@oblique/oblique';
import {MasterDataService} from '@shared/master-data.service';
import {SearchStorageService} from '@shared/services/search-storage.service';
import {personSearchStorageKey} from '@shared/storage-keys';
import {debounceTime, merge} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {Role} from '../../auth/Role';

@Component({
    selector: 'apg-persons-filter',
    changeDetection: ChangeDetectionStrategy.OnPush,
    templateUrl: './persons-filter.component.html',
    styleUrl: './persons-filter.component.scss',
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
        MatTooltip,
        ObInputClearDirective,
    ],
})
export class PersonsFilterComponent {
    @Output() readonly filter = new EventEmitter<PersonFilterParameters>();

    form: FormGroup<PersonFilterForm>;
    formFilter?: PersonFilterParameters;

    canCreatePerson = computed(
        () =>
            this.roles().includes(Role.Admin) ||
            this.roles().includes(Role.Department) ||
            this.roles().includes(Role.Office) ||
            this.roles().includes(Role.Secretariat)
    );

    private readonly roles = toSignal(this.authService.roles$, {initialValue: []});

    constructor(
        private readonly router: Router,
        private readonly fb: FormBuilder,
        protected readonly masterDataService: MasterDataService,
        private readonly authService: AuthService,
        private readonly searchStorageService: SearchStorageService
    ) {
        this.form = this.setupPersonFilterForm();
        this.form.controls.freeText.valueChanges.pipe(debounceTime(300), takeUntilDestroyed()).subscribe(() => this.applyFilter());
        merge(this.form.controls.hasActiveMembership.valueChanges, this.form.controls.cantons.valueChanges, this.form.controls.languages.valueChanges)
            .pipe(takeUntilDestroyed())
            .subscribe(() => this.applyFilter());

        this.setInitialFilterValuesToEmit();
    }

    applyFilter() {
        const filterParameters = {
            freeText: this.form.controls.freeText.value,
            hasActiveMembership: this.form.controls.hasActiveMembership.value,
            cantons: this.form.controls.cantons.value,
            languages: this.form.controls.languages.value,
        } as PersonFilterParameters;

        this.searchStorageService.setParams(personSearchStorageKey, filterParameters);
        this.filter.emit(filterParameters);
    }

    reset(): void {
        this.formFilter = undefined;
        this.searchStorageService.removeParams(personSearchStorageKey);
        this.form.reset();
    }

    create(): void {
        void this.router.navigate(['persons/create']);
    }

    clearFreeText(): void {
        this.form.controls.freeText.patchValue(null);
    }

    setupPersonFilterForm(): FormGroup<PersonFilterForm> {
        const params = this.searchStorageService.getParams(personSearchStorageKey);

        const formGroup = this.fb.group<PersonFilterForm>({
            freeText: this.fb.control<string | null>(null),
            hasActiveMembership: this.fb.control<boolean[] | null>(null),
            cantons: this.fb.control<string[] | null>(null),
            languages: this.fb.control<string[] | null>(null),
        });

        if (params) {
            this.formFilter = params;
        }
        return formGroup;
    }

    private setInitialFilterValuesToEmit() {
        if (this.formFilter) {
            this.form.patchValue(this.formFilter);
        }
    }
}
