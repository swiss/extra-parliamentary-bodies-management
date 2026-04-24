import {Component, computed, signal, viewChild, WritableSignal} from '@angular/core';
import {MatButton} from '@angular/material/button';
import {Router} from '@angular/router';
import {PersonCreate} from '@api/PersonCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService, ObButtonDirective} from '@oblique/oblique';
import {PersonsService} from '../persons.service';
import {PersonDataFormComponent} from '../shared/person-data-form/person-data-form.component';

@Component({
    selector: 'apg-person-create',
    templateUrl: './person-create.component.html',
    styleUrl: './person-create.component.scss',
    imports: [MatButton, ObButtonDirective, PersonDataFormComponent, TranslatePipe],
})
export class PersonCreateComponent {
    personToCreate!: WritableSignal<PersonCreate>;
    formComponent = viewChild.required(PersonDataFormComponent);
    form = computed(() => this.formComponent().personForm);

    constructor(
        private readonly router: Router,
        private readonly personsService: PersonsService,
        private readonly notificationService: ObNotificationService
    ) {
        this.personsService.getPersonForCreate().subscribe(personCreate => (this.personToCreate = signal(personCreate)));
    }

    save() {
        this.formComponent().validateAddresses();
        if (!this.form().valid) {
            this.form().markAsTouched();
            return;
        }

        const personToCreate = this.formComponent().buildPersonModification() as PersonCreate;

        this.personsService.createPerson(personToCreate).subscribe({
            next: async p => {
                this.form().reset(this.personToCreate());
                this.personsService.reload$.next();
                await this.router.navigate(['persons', p.id], {replaceUrl: true, queryParams: {tab: 'data'}});
                return this.notificationService.success('person.details.data.success');
            },
            error: () => this.notificationService.error('person.details.data.error'),
        });
    }

    close() {
        void this.router.navigate(['persons']);
    }
}
