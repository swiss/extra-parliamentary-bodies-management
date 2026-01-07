import {Component, EventEmitter, Output, model, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatAutocompleteSelectedEvent, MatAutocompleteTrigger, MatAutocomplete} from '@angular/material/autocomplete';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatInput} from '@angular/material/input';
import {MatOption} from '@angular/material/select';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObErrorMessagesDirective} from '@oblique/oblique';
import {merge, switchMap, debounceTime, filter} from 'rxjs';
import {CommitteesService} from '../../../committees/committees.service';

@Component({
    selector: 'apg-committee-search',
    templateUrl: './committee-search.component.html',
    styleUrl: './committee-search.component.scss',
    imports: [
        ReactiveFormsModule,
        MatFormField,
        ObErrorMessagesDirective,
        MatInput,
        MatAutocompleteTrigger,
        MatAutocomplete,
        MatOption,
        MatIcon,
        TranslatePipe,
    ],
})
export class CommitteeSearchComponent {
    committeeSearchForm = this.createForm();
    committeeSelected = model<CommitteeDetails | null>();
    filteredOptions = signal<CommitteeDetails[]>([]);
    @Output() readonly updateValidity: EventEmitter<void> = new EventEmitter();

    constructor(
        protected readonly committeeService: CommitteesService,
        protected readonly obNotificationService: ObNotificationService
    ) {
        this.subscribeCommitteeChanges();
    }

    setCommitteName(event: MatAutocompleteSelectedEvent) {
        const committee = event.option.value as CommitteeDetails;
        this.committeeSearchForm.controls.description.setValue(committee.description, {emitEvent: false});
        this.committeeSelected.set(committee);
        this.updateValidity.emit();
        this.filteredOptions.set([]);
    }

    public setTextAndDisable(text: string) {
        this.committeeSearchForm.controls.description.setValue(text);
        this.committeeSearchForm.controls.description.disable();
    }

    public markTextBoxAsTouched() {
        this.committeeSearchForm.controls.description.markAllAsTouched();
    }

    private subscribeCommitteeChanges() {
        merge(
            this.committeeSearchForm.controls.description.valueChanges.pipe(
                filter(searchValue => !!searchValue),
                debounceTime(300),
                switchMap(searchValue => this.committeeService.getCommitteesByDescription(searchValue!)),
                takeUntilDestroyed()
            )
        ).subscribe({
            next: suggestions => {
                this.filteredOptions.set(suggestions);
            },
            error: () => {
                this.obNotificationService.error('memberships.search.committee.error');
            },
        });
    }

    private createForm() {
        return new FormGroup({
            description: new FormControl<string | undefined>(undefined, [Validators.required]),
        });
    }
}
