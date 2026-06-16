import {NgClass} from '@angular/common';
import {Component, computed, input} from '@angular/core';
import {AddressDetails} from '@api/AddressDetails';
import {TranslatePipe} from '@ngx-translate/core';

@Component({
    selector: 'apg-person-overview-address',
    templateUrl: './person-overview-address.component.html',
    styleUrl: './person-overview-address.component.scss',
    imports: [NgClass, TranslatePipe],
})
export class PersonOverviewAddressComponent {
    addressDetail = input.required<AddressDetails>();
    zipCity = computed(() => {
        const {country, zip, city, canton} = this.addressDetail();
        return [`${country?.trim() ? `${country}-` : ''}${zip?.trim() ?? ''}`, city, `${canton?.trim() ? `(${canton})` : ''}`].filter(x => !!x).join(' ');
    });
}
