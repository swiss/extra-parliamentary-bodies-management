import {ComponentFixture, fakeAsync, TestBed, tick} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatAutocompleteModule, MatAutocompleteSelectedEvent} from '@angular/material/autocomplete';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {CommitteeList} from '@api/CommitteeList';
import {TranslatePipe, TranslateModule} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {of, throwError} from 'rxjs';
import {CommitteesService} from '../../../committees/committees.service';
import {CommitteeSearchComponent} from './committee-search.component';

describe('CommitteeSearchComponent', () => {
    let component: CommitteeSearchComponent;
    let fixture: ComponentFixture<CommitteeSearchComponent>;

    const notificationServiceMock: Partial<ObNotificationService> = {
        success: jest.fn(),
        error: jest.fn(),
    };

    const committeesServiceMock: Partial<CommitteesService> = {
        getCommitteesByDescription: jest.fn(() => of([])),
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
                CommitteeSearchComponent,
            ],
            providers: [
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeSearchComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should set committee name', () => {
        const newValue = {description: 'committeeTest', id: 'id'} as CommitteeList;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;
        const spy = jest.spyOn(component.updateValidity, 'emit');
        component.setCommitteName(selectedEvent);

        expect(component.committeeSearchForm.controls.description.value).toBe('committeeTest');
        expect(component.committeeSelected()).not.toBeNull();
        expect(component.committeeSelected()?.id).toBe('id');
        expect(component.committeeSelected()?.description).toBe('committeeTest');
        expect(component.filteredOptions()).toHaveLength(0);
        expect(spy).toHaveBeenCalled();
    });

    it('should call service', fakeAsync(() => {
        component.committeeSearchForm.controls.description.setValue('test');

        tick(500);

        expect(committeesServiceMock.getCommitteesByDescription).toHaveBeenCalledWith('test');
    }));

    it('should handle error at search', fakeAsync(() => {
        (committeesServiceMock.getCommitteesByDescription as jest.Mock).mockReturnValue(throwError(() => new Error('Search failed')));

        component.committeeSearchForm.controls.description.setValue('test2');
        tick(500);

        expect(notificationServiceMock.error).toHaveBeenCalledWith('memberships.search.committee.error');
    }));

    it('should set values in control when setCommitteName is called with valid value', () => {
        const newValue = {description: 'committeeTest'} as CommitteeList;
        const selectedEvent: MatAutocompleteSelectedEvent = {
            option: {value: newValue},
        } as MatAutocompleteSelectedEvent;

        component.setCommitteName(selectedEvent);

        expect(component.committeeSearchForm.controls.description.value).toBe('committeeTest');
    });
});
