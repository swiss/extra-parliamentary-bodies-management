import {Component, signal, model, EventEmitter, Output} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatAutocompleteSelectedEvent, MatAutocompleteTrigger, MatAutocomplete} from '@angular/material/autocomplete';
import {MatIcon} from '@angular/material/icon';
import {MatFormField, MatInput} from '@angular/material/input';
import {MatOption} from '@angular/material/select';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObErrorMessagesDirective} from '@oblique/oblique';
import {merge, switchMap, debounceTime, filter} from 'rxjs';
import {PersonsService} from '../../../persons/persons.service';

@Component({
    selector: 'apg-person-search',
    templateUrl: './person-search.component.html',
    styleUrl: './person-search.component.scss',
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
export class PersonSearchComponent {
    personSearchForm = this.createForm();
    personSelected = model<PersonDetails | null>();
    filteredOptions = signal<PersonDetails[]>([]);
    @Output() readonly updateValidity: EventEmitter<void> = new EventEmitter();

    constructor(
        protected readonly personService: PersonsService,
        protected readonly obNotificationService: ObNotificationService
    ) {
        this.subscribePersonChanges();
    }

    public markTextBoxAsTouched() {
        this.personSearchForm.controls.name.markAllAsTouched();
    }

    public setTextAndDisable(text: string) {
        this.personSearchForm.controls.name.setValue(text);
        this.personSearchForm.controls.name.disable();
    }

    public reset() {
        this.personSearchForm.controls.name.setValue('');
        this.personSearchForm.controls.name.enable();
        this.personSelected.set(null);
        this.filteredOptions.set([]);
        this.updateValidity.emit();
    }

    setPersonName(event: MatAutocompleteSelectedEvent) {
        const person = event.option.value as PersonDetails;
        this.personSearchForm.controls.name.setValue(`${person.givenName} ${person.surname} (${person.birthYear})`, {emitEvent: false});
        this.personSelected.set(person);
        this.updateValidity.emit();
        this.filteredOptions.set([]);
    }

    private subscribePersonChanges() {
        merge(
            this.personSearchForm.controls.name.valueChanges.pipe(
                filter(searchValue => !!searchValue),
                debounceTime(300),
                switchMap(searchValue => this.personService.getPersonsByName(searchValue!)),
                takeUntilDestroyed()
            )
        ).subscribe({
            next: suggestions => {
                this.filteredOptions.set(suggestions);
            },
            error: () => {
                this.obNotificationService.error('memberships.search.person.error');
            },
        });
    }

    private createForm() {
        return new FormGroup({
            name: new FormControl<string | undefined>(undefined, [Validators.required]),
        });
    }
}
