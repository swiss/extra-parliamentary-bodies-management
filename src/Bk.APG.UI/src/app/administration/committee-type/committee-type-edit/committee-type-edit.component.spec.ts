import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {WritableSignal, signal, NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ControlEvent, FormGroup} from '@angular/forms';
import {ActivatedRoute, Router, ActivatedRouteSnapshot} from '@angular/router';
import {CommitteeTypeUpdate} from '@api/CommitteeTypeUpdate';
import {TranslateModule, TranslatePipe} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {MockPipe, MockComponents, MockModule} from 'ng-mocks';
import {Subject, of, throwError} from 'rxjs';
import {CommitteeTypeFormComponent} from '../committee-type-form/committee-type-form.component';
import {CommitteeTypeService} from '../committee-type.service';
import {CommitteeTypeEditComponent} from './committee-type-edit.component';

describe('CommitteeTypeEditComponent', () => {
    let component: CommitteeTypeEditComponent;
    let fixture: ComponentFixture<CommitteeTypeEditComponent>;
    let committeeTypeServiceMock: Partial<CommitteeTypeService>;
    let activatedRouteMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let eventsSubject: Subject<ControlEvent>;
    let form: WritableSignal<FormGroup>;

    beforeEach(async () => {
        eventsSubject = new Subject<ControlEvent>();

        committeeTypeServiceMock = {
            getCommitteeTypeForUpdate: jest.fn(),
            updateCommitteeType: jest.fn(),
        };

        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        activatedRouteMock = {
            snapshot: {
                params: {id: '123'},
                url: ['route', 'copy'],
            } as unknown as ActivatedRouteSnapshot,
        };
        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), CommitteeTypeEditComponent, MockComponents(CommitteeTypeFormComponent), MockModule(TranslateModule)],
            providers: [
                {provide: CommitteeTypeService, useValue: committeeTypeServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();

        (committeeTypeServiceMock.getCommitteeTypeForUpdate as jest.Mock).mockReturnValue(of({id: '123', text: 'Gremiumtyp a'}));

        fixture = TestBed.createComponent(CommitteeTypeEditComponent);
        component = fixture.componentInstance;

        // @ts-ignore
        form = signal({
            pristine: true,
            events: eventsSubject.asObservable(),
            valid: true,
            reset: jest.fn(),
            markAsTouched: jest.fn(),
        });
        component.form = form;

        component.committeeTypeToUpdate = signal({} as unknown as CommitteeTypeUpdate);

        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('save', () => {
        it('should mark form as touched if invalid', () => {
            form.update(form => ({...form, valid: false}) as unknown as FormGroup);

            component.save();

            expect(component.form().markAsTouched).toHaveBeenCalled();
            expect(committeeTypeServiceMock.updateCommitteeType).not.toHaveBeenCalled();
        });

        it('should call updateCommitteeType and handle success', async () => {
            (committeeTypeServiceMock.updateCommitteeType as jest.Mock).mockReturnValue(of({}));
            const committeeTypeBeforeSave = {
                id: '1',
                text: 'text',
                femaleThreshold: 15,
                maleThreshold: 15,
                germanMinimalThreshold: undefined,
                frenchMinimalThreshold: undefined,
                italianMinimalThreshold: undefined,
                romanshMinimalThreshold: undefined,
                germanThresholdPercentage: 25,
                frenchThresholdPercentage: 25,
                italianThresholdPercentage: 25,
                romanshThresholdPercentage: 10,
            } as CommitteeTypeUpdate;

            component.committeeTypeToUpdate.set(committeeTypeBeforeSave);
            component.unmodifiedCommitteeType = {
                id: '1',
                text: 'text2',
                femaleThreshold: 25,
                maleThreshold: 25,
                germanMinimalThreshold: undefined,
                frenchMinimalThreshold: undefined,
                italianMinimalThreshold: undefined,
                romanshMinimalThreshold: undefined,
                germanThresholdPercentage: 30,
                frenchThresholdPercentage: 30,
                italianThresholdPercentage: 30,
                romanshThresholdPercentage: 5,
            } as CommitteeTypeUpdate;

            component.save();

            expect(committeeTypeServiceMock.updateCommitteeType).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith(['administration/committeeTypes']);

            await fixture.whenStable();

            expect(notificationServiceMock.success).toHaveBeenCalledWith('committeeType.save.success');
            expect(component.unmodifiedCommitteeType).toEqual(committeeTypeBeforeSave);
        });

        it('should handle updateCommitteeType error', () => {
            (committeeTypeServiceMock.updateCommitteeType as jest.Mock).mockReturnValue(throwError(() => new Error('Update failed')));

            component.save();

            expect(committeeTypeServiceMock.updateCommitteeType).toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('committeeType.save.error');
        });
    });

    describe('reset', () => {
        it('should reset form', () => {
            component.reset();

            expect(component.form().reset).toHaveBeenCalled();
        });
    });

    describe('back', () => {
        it('should navigate back from form', () => {
            component.back();

            expect(routerMock.navigate).toHaveBeenCalledWith(['administration/committeeTypes']);
        });
    });
});
