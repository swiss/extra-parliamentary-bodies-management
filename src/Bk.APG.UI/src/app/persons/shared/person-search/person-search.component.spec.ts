import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatAutocompleteModule, MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe, TranslateModule} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {of, throwError} from 'rxjs';
import {PersonsService} from '../../../persons/persons.service';
import {PersonSearchComponent} from './person-search.component';

describe('PersonSearchComponent', () => {
    let component: PersonSearchComponent;
    let fixture: ComponentFixture<PersonSearchComponent>;

    const notificationServiceMock: Partial<ObNotificationService> = {
        success: jest.fn(),
        error: jest.fn(),
    };

    const personsServiceMock: Partial<PersonsService> = {
        getPersonsByName: jest.fn(() => of()),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                MockPipe(TranslatePipe),
                MockModule(ReactiveFormsModule),
                MockModule(ObErrorMessagesModule),
                MockModule(ObInputClearModule),
                MockComponents(MatFormField),
                MockDirectives(MatInput, MatLabel),
                MockModule(MatAutocompleteModule),
                TranslateModule,
                PersonSearchComponent,
            ],
            providers: [
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonSearchComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should set person name', () => {
        const newValue = {surname: 'Clark', givenName: 'Jim', birthYear: 1936, id: 'id'} as PersonDetails;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;
        const spy = jest.spyOn(component.updateValidity, 'emit');
        component.setPersonName(selectedEvent);

        expect(component.personSearchForm.controls.name.value).toBe('Jim Clark (1936)');
        expect(component.personSelected()).not.toBeNull();
        expect(component.personSelected()?.id).toBe('id');
        expect(component.personSelected()?.givenName).toBe('Jim');
        expect(component.personSelected()?.surname).toBe('Clark');
        expect(component.personSelected()?.birthYear).toBe(1936);
        expect(component.filteredOptions()).toHaveLength(0);
        expect(spy).toHaveBeenCalled();
    });

    it('should call service', fakeAsync(() => {
        component.personSearchForm.controls.name.setValue('test');
        tick(500);

        expect(personsServiceMock.getPersonsByName).toHaveBeenCalledWith('test');
    }));

    it('should handle error at search', fakeAsync(() => {
        (personsServiceMock.getPersonsByName as jest.Mock).mockReturnValue(throwError(() => new Error('Search failed')));

        component.personSearchForm.controls.name.setValue('test2');
        tick(500);

        expect(notificationServiceMock.error).toHaveBeenCalledWith('memberships.search.person.error');
    }));

    it('should set values in control when setPersonName is called with valid value', () => {
        const newValue = {surname: 'Clark', givenName: 'Jim', birthYear: 1936} as PersonDetails;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.setPersonName(selectedEvent);

        expect(component.personSearchForm.controls.name.value).toBe('Jim Clark (1936)');
    });
});
