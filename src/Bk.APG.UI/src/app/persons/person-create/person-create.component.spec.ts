import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {Router} from '@angular/router';
import {PersonCreate} from '@api/PersonCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockPipe} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {PersonsService} from '../persons.service';
import {PersonDataFormComponent} from '../shared/person-data-form/person-data-form.component';
import {PersonCreateComponent} from './person-create.component';

describe('PersonCreateComponent', () => {
    let component: PersonCreateComponent;
    let fixture: ComponentFixture<PersonCreateComponent>;
    let personsServiceMock: Partial<PersonsService>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let reloadSubject: Subject<void>;

    beforeEach(async () => {
        reloadSubject = new Subject<void>();
        personsServiceMock = {
            getPersonForCreate: jest.fn(() => of()),
            createPerson: jest.fn(),
            reload$: reloadSubject,
        };

        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };
        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), MockComponents(PersonDataFormComponent), PersonCreateComponent],
            providers: [
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonCreateComponent);
        component = fixture.componentInstance;
        // @ts-ignore
        component.form = () => ({
            pristine: true,
        });
        component.personToCreate = signal({} as unknown as PersonCreate);

        (personsServiceMock.getPersonForCreate as jest.Mock).mockReturnValue(of({name: 'John Doe'}));
        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('save should mark form as touched if invalid', () => {
        // @ts-ignore
        component.form = signal({
            valid: false,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });
        component.save();
        expect(component.form().markAsTouched).toHaveBeenCalled();
        expect(personsServiceMock.createPerson).not.toHaveBeenCalled();
    });

    it('save should call createPerson and handle success', async () => {
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });
        (personsServiceMock.createPerson as jest.Mock).mockReturnValue(of({id: '1'}));

        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        component.save();

        expect(personsServiceMock.createPerson).toHaveBeenCalled();
        expect(reloadNextSpy).toHaveBeenCalled();
        expect(routerMock.navigate).toHaveBeenCalledWith(['persons', '1'], {queryParams: {tab: 'data'}, replaceUrl: true});
        await fixture.whenStable();
        expect(notificationServiceMock.success).toHaveBeenCalledWith('person.details.data.success');
    });

    it('save should handle createPerson error', () => {
        // @ts-ignore
        component.form = () => ({
            valid: true,
            reset: jest.fn(),
        });
        (personsServiceMock.createPerson as jest.Mock).mockReturnValue(throwError(() => new Error('Create failed')));

        component.save();

        expect(personsServiceMock.createPerson).toHaveBeenCalled();
        expect(notificationServiceMock.error).toHaveBeenCalledWith('person.details.data.error');
    });
});
