import {NgClass} from '@angular/common';
import {AfterViewInit, ChangeDetectorRef, Component, DestroyRef, EventEmitter, Input, Output, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {ReactiveFormsModule, UntypedFormGroup} from '@angular/forms';
import {MatAutocomplete, MatAutocompleteSelectedEvent, MatAutocompleteTrigger} from '@angular/material/autocomplete';
import {MatIconButton} from '@angular/material/button';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatIcon} from '@angular/material/icon';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatProgressBar} from '@angular/material/progress-bar';
import {MatOption, MatSelect, MatSuffix} from '@angular/material/select';
import {MatTooltip} from '@angular/material/tooltip';
import {UidResult} from '@api/UidResult';
import {TranslatePipe} from '@ngx-translate/core';
import {ObButtonDirective, ObErrorMessagesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {debounceTime, finalize, of, switchMap} from 'rxjs';
import {PersonInterestsService} from '../person-interests.service';

@Component({
    selector: 'apg-interests-edit-form-detail',
    templateUrl: './interests-edit-form-detail.component.html',
    styleUrl: './interests-edit-form-detail.component.scss',
    imports: [
        ReactiveFormsModule,
        MatFormField,
        MatInput,
        ObErrorMessagesDirective,
        MatLabel,
        MatAutocompleteTrigger,
        MatError,
        MatAutocomplete,
        MatOption,
        MatProgressBar,
        MatSelect,
        MatIconButton,
        ObButtonDirective,
        MatTooltip,
        MatIcon,
        MatDatepickerInput,
        MatDatepickerToggle,
        MatSuffix,
        MatDatepicker,
        TranslatePipe,
        NgClass,
    ],
})
export class InterestsEditFormDetailComponent implements AfterViewInit {
    @Input() formGroup!: UntypedFormGroup;
    @Output() readonly interestRemoved = new EventEmitter<void>();

    isInactiveRow = false;
    isUidRow = false;
    cleanOrganizations = signal<UidResult[]>([]);
    isLoadingSuggestions = signal(false);

    private static readonly RECOMMENDATION_ORGANIZATION_MIN_LENGTH = 3;

    private prohibitResetUidId: boolean = false;

    constructor(
        protected readonly personInterestsService: PersonInterestsService,
        protected readonly masterDataService: MasterDataService,
        protected readonly errorService: ErrorService,
        private readonly destroyRef: DestroyRef,
        private readonly cd: ChangeDetectorRef
    ) {}

    ngAfterViewInit(): void {
        const interest = this.formGroup.controls.interestText;
        interest.markAsTouched();
        interest.updateValueAndValidity();
        this.formGroup.controls.interestText.valueChanges
            .pipe(
                debounceTime(500),
                switchMap(value => {
                    if (interest.dirty) {
                        this.formGroup.markAllAsTouched();
                        this.formGroup.updateValueAndValidity({emitEvent: false});
                    }
                    if (!value || value.trim().length < InterestsEditFormDetailComponent.RECOMMENDATION_ORGANIZATION_MIN_LENGTH) {
                        return of([]);
                    }

                    if (!this.prohibitResetUidId) {
                        this.formGroup.controls.uidOrganisationId.setValue(null);
                    }
                    this.isLoadingSuggestions.set(true);
                    return this.personInterestsService.getUidOrganizations(value).pipe(finalize(() => this.isLoadingSuggestions.set(false)));
                }),
                takeUntilDestroyed(this.destroyRef)
            )
            .subscribe(items => {
                const allOrganizations = items.map(item => ({
                    organizationName: item.organizationName,
                    uidOrganisationId: item.uidOrganisationId,
                    city: item.city!,
                    zip: item.zip!,
                    matchQuality: item.matchQuality!,
                    legalFormText: item.legalFormText,
                    id: `${item.uidOrganisationId!};${item.organizationName!};${item.legalFormId!}`,
                }));

                this.cleanOrganizations.set(allOrganizations);
            });
        this.isInactiveRow = this.formGroup?.controls?.isInactiveRow?.value;
        this.isUidRow = this.formGroup?.controls?.isUidRow?.value;
        this.cd.detectChanges();
    }

    remove(): void {
        this.interestRemoved.emit();
    }

    setOrganization(event: MatAutocompleteSelectedEvent) {
        this.prohibitResetUidId = true;
        const key = event.option.value;
        const splitted = key.split(';');
        this.formGroup.controls.uidOrganisationId.setValue(splitted[0]);
        this.formGroup.controls.interestText.setValue(splitted[1]);
        this.formGroup.controls.legalFormId.setValue(splitted[2]);

        setTimeout(() => {
            this.prohibitResetUidId = false;
        }, 1000);
    }
}
