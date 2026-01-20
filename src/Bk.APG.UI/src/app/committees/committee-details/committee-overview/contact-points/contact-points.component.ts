import {Component, computed, input} from '@angular/core';
import {ContactPointDetail} from '@api/ContactPointDetail';
import {TranslatePipe} from '@ngx-translate/core';
import {ObAlertModule} from '@oblique/oblique';
import {ConfigsService} from '../../../../../app/configs.service';

@Component({
    selector: 'apg-contact-points',
    templateUrl: './contact-points.component.html',
    styleUrl: './contact-points.component.scss',
    imports: [TranslatePipe, ObAlertModule],
})
export class ContactPointsComponent {
    contactPoints = input.required<ContactPointDetail[]>();
    contactPointsSecretariats = computed(() =>
        this.contactPoints().filter(x => x.contactPointTypeId === this.configsService.frontendConfig.entityIds.contactPoint.secretariatId && x.isActive)
    );
    contactPointsDataProtectionOfficers = computed(() =>
        this.contactPoints().filter(x => x.contactPointTypeId === this.configsService.frontendConfig.entityIds.contactPoint.dpoId && x.isActive)
    );
    constructor(protected readonly configsService: ConfigsService) {}
}
