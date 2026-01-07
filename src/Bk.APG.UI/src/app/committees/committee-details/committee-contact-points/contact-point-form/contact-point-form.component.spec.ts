import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatAutocompleteModule, MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatOption} from '@angular/material/core';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';
import {MatSelect} from '@angular/material/select';
import {MatTooltipModule} from '@angular/material/tooltip';
import {Router} from '@angular/router';
import {Address} from '@api/Address';
import {Office} from '@api/Office';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {ContactPointFormComponent} from './contact-point-form.component';

describe('ContactPointFormComponent', () => {
    let component: ContactPointFormComponent;
    let fixture: ComponentFixture<ContactPointFormComponent>;

    const masterDataServiceMock = {
        getOfficeByName: jest.fn(),
        languages: jest.fn(),
        genders: jest.fn(),
    } as unknown as Partial<MasterDataService>;

    const routerMock = {
        navigate: jest.fn(),
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                MockModule(ReactiveFormsModule),
                MockModule(ObErrorMessagesModule),
                MockModule(MatIconModule),
                MockModule(ObInputClearModule),
                MockModule(MatDatepickerModule),
                MockModule(MatAutocompleteModule),
                MockModule(MatRadioModule),
                MockModule(MatTooltipModule),
                MockComponents(MatFormField, MatCheckbox, MatOption, MatSelect, MatDatepicker, MatDatepickerToggle, HelpTooltipComponent),
                MockDirectives(MatInput, MatLabel, ObUnsavedChangesDirective),
                MockPipe(TranslatePipe),
                ContactPointFormComponent,
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ErrorService, useValue: errorServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(ContactPointFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should set values in controls when setStreet is called with valid values', () => {
        const newValue = {city: 'Baden', zip: '5400', street: 'Bahnhofstrasse'} as Address;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.setStreet(selectedEvent);

        expect(component.contactPointForm.controls.street.value).toBe('Bahnhofstrasse');
        expect(component.contactPointForm.controls.zip.value).toBe('5400');
        expect(component.contactPointForm.controls.city.value).toBe('Baden');
    });

    it('should set values in controls when setZipOrCity is called with valid values', () => {
        const newValue = {city: 'Baden', zip: '5400'} as Address;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.setZipOrCity(selectedEvent);

        expect(component.contactPointForm.controls.zip.value).toBe('5400');
        expect(component.contactPointForm.controls.city.value).toBe('Baden');
    });

    it('should clean address fields when clear is called', () => {
        component.contactPointForm.controls.street.setValue('ab');
        component.contactPointForm.controls.zip.setValue('cd');
        component.contactPointForm.controls.city.setValue('ef');

        component.clear();

        expect(component.contactPointForm.controls.street.value).toBe(null);
        expect(component.contactPointForm.controls.zip.value).toBe(null);
        expect(component.contactPointForm.controls.city.value).toBe(null);
    });

    it('should set isSecretariat correctly', () => {
        let currentView = 'https://politics.ld.admin.ch/fch/apg/vocabulary/contact-point-type/2';
        component.changeContactPointType(currentView);
        expect(component.isSecretariat).toBe(false);

        currentView = 'https://politics.ld.admin.ch/fch/apg/vocabulary/contact-point-type/1';
        component.changeContactPointType(currentView);
        expect(component.isSecretariat).toBe(true);
    });

    it('should set values in control when setCompanyName is called with valid value', () => {
        const newValue = {description: 'Bundesamt für vieles'} as Office;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.setCompanyName(selectedEvent);

        expect(component.contactPointForm.controls.companyName.value).toBe('Bundesamt für vieles');
    });
});
