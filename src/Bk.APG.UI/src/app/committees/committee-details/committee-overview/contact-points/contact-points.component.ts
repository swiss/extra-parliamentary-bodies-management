import {Component, computed, input} from '@angular/core';
import {ContactPointDetail} from '@api/ContactPointDetail';
import {TranslatePipe} from '@ngx-translate/core';
import {ObAlertModule} from '@oblique/oblique';

export const SECRETARIAT = 'Sekretariat';
export const DPO = 'Datenschutzberater/in';

@Component({
    selector: 'apg-contact-points',
    templateUrl: './contact-points.component.html',
    styleUrl: './contact-points.component.scss',
    imports: [TranslatePipe, ObAlertModule],
})
export class ContactPointsComponent {
    contactPoints = input.required<ContactPointDetail[]>();
    contactPointsSecretariats = computed(() => this.contactPoints().filter(x => x.contactPointType === SECRETARIAT && x.isActive));
    contactPointsDataProtectionOfficers = computed(() => this.contactPoints().filter(x => x.contactPointType === DPO && x.isActive));
}
