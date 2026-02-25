import {ComponentFixture, TestBed} from '@angular/core/testing';
import {AddressDetails} from '@api/AddressDetails';
import {TranslatePipe} from '@ngx-translate/core';
import {MockPipe} from 'ng-mocks';
import {PersonOverviewAddressComponent} from './person-overview-address.component';

describe('PersonsAddressDetailComponent', () => {
    let component: PersonOverviewAddressComponent;
    let fixture: ComponentFixture<PersonOverviewAddressComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), PersonOverviewAddressComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonOverviewAddressComponent);
        fixture.componentRef.setInput('addressDetail', {});
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should compute zipCity correctly with all fields', () => {
        fixture.componentRef.setInput('addressDetail', {
            country: 'US',
            zip: '12345',
            city: 'New York',
            canton: 'NY',
        } as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('US-12345 New York (NY)');
    });

    it('should compute zipCity correctly without country', () => {
        fixture.componentRef.setInput('addressDetail', {
            zip: '12345',
            city: 'New York',
            canton: 'NY',
        } as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('12345 New York (NY)');
    });

    it('should compute zipCity correctly without canton', () => {
        fixture.componentRef.setInput('addressDetail', {
            country: 'US',
            zip: '12345',
            city: 'New York',
        } as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('US-12345 New York');
    });

    it('should compute zipCity correctly with minimal fields', () => {
        fixture.componentRef.setInput('addressDetail', {
            zip: '12345',
            city: 'New York',
        } as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('12345 New York');
    });

    it('should compute zipCity correctly with empty fields', () => {
        fixture.componentRef.setInput('addressDetail', {} as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('');
    });

    it('should update zipCity when addressDetail changes', () => {
        fixture.componentRef.setInput('addressDetail', {
            country: 'DE',
            zip: '10115',
            city: 'Berlin',
            canton: '',
        } as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('DE-10115 Berlin');

        fixture.componentRef.setInput('addressDetail', {
            country: 'DE',
            zip: '10115',
            city: 'Berlin',
            canton: 'BE',
        } as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('DE-10115 Berlin (BE)');
    });

    it('should compute zipCity correctly', () => {
        fixture.componentRef.setInput('addressDetail', {
            country: 'FR',
            zip: '75001',
            city: 'Paris',
            canton: 'IDF',
        } as AddressDetails);
        fixture.detectChanges();
        expect(component.zipCity()).toBe('FR-75001 Paris (IDF)');
    });
});
