import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormGroup} from '@angular/forms';
import {Router} from '@angular/router';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, throwError} from 'rxjs';
import {FormLettersSenderService} from '../form-letters-sender.service';
import {FormLettersSenderDataFormComponent} from '../shared/form-letters-sender-data-form/form-letters-sender-data-form.component';
import {FormLettersSenderCreateComponent} from './form-letters-sender-create.component';

describe('FormLettersSenderCreateComponent', () => {
    let component: FormLettersSenderCreateComponent;
    let fixture: ComponentFixture<FormLettersSenderCreateComponent>;
    let formLettersSenderServiceMock: Partial<FormLettersSenderService>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let interceptorEventsMock: Partial<ObHttpApiInterceptorEvents>;
    let reloadSubject: BehaviorSubject<void>;
    let markAsTouchedSpy: jest.Mock;
    let markAsPristineSpy: jest.Mock;

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    let mockFormComponentData: any;

    beforeEach(async () => {
        reloadSubject = new BehaviorSubject<void>(undefined);
        markAsTouchedSpy = jest.fn();
        markAsPristineSpy = jest.fn();

        mockFormComponentData = {
            senderForm: {
                valid: true,
                getRawValue: () => ({
                    description: 'Test Sender',
                    surname: 'Doe',
                    givenName: 'John',
                    senderFunctionId: 'f1',
                    departmentId: 'd1',
                    officeId: 'o1',
                    streetGerman: 'Main St',
                    streetFrench: 'Rue Principale',
                    streetItalian: 'Via Principale',
                    streetRomansh: 'Via Principala',
                    zip: '5400',
                    cityGerman: 'Baden',
                    cityFrench: 'Bâle',
                    cityItalian: 'Basilea',
                    cityRomansh: 'Basilea',
                    phone: '0041123456789',
                    email: 'john@example.com',
                    website: 'https://example.com',
                }),
                markAsTouched: markAsTouchedSpy,
                markAsPristine: markAsPristineSpy,
                reset: jest.fn(),
            } as unknown as FormGroup,
            signature: undefined,
        };

        formLettersSenderServiceMock = {
            createFormLettersSender: jest.fn().mockReturnValue(of({id: '123', signatureFileName: 'sig.png'})),
            getEmptyFormLettersSender: jest.fn().mockReturnValue(of({})),
            reload$: reloadSubject,
        };

        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };

        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        interceptorEventsMock = {
            deactivateNotificationOnNextAPICalls: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), MockComponents(FormLettersSenderDataFormComponent), FormLettersSenderCreateComponent],
            providers: [
                {provide: FormLettersSenderService, useValue: formLettersSenderServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: interceptorEventsMock},
                {provide: TranslateService, useValue: {instant: jest.fn((key: string) => key)}},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(FormLettersSenderCreateComponent);
        component = fixture.componentInstance;

        // @ts-ignore
        component.formComponent = () => mockFormComponentData;

        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should navigate to sender list on close', async () => {
        component.close();

        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'exports', 'formLetters']);
        await fixture.whenStable();
    });

    it('save should mark form as touched if invalid', () => {
        const invalidMarkAsTouchedSpy = jest.fn();
        mockFormComponentData.senderForm.valid = false;
        mockFormComponentData.senderForm.markAsTouched = invalidMarkAsTouchedSpy;

        component.save();

        expect(invalidMarkAsTouchedSpy).toHaveBeenCalled();
        expect(formLettersSenderServiceMock.createFormLettersSender).not.toHaveBeenCalled();
    });

    it('save should call createFormLettersSender and handle success', async () => {
        mockFormComponentData.senderForm.valid = true;
        mockFormComponentData.senderForm.markAsTouched = markAsTouchedSpy;

        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');

        component.save();

        expect(formLettersSenderServiceMock.createFormLettersSender).toHaveBeenCalled();
        expect(reloadNextSpy).toHaveBeenCalled();
        await fixture.whenStable();
        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'exports', 'formLetters']);
        expect(notificationServiceMock.success).toHaveBeenCalledWith('formLetter.sender.create.success');
    });

    it('save should handle createFormLettersSender error', async () => {
        mockFormComponentData.senderForm.valid = true;

        (formLettersSenderServiceMock.createFormLettersSender as jest.Mock).mockReturnValue(throwError(() => new Error('Create failed')));

        component.save();

        expect(formLettersSenderServiceMock.createFormLettersSender).toHaveBeenCalled();
        await fixture.whenStable();
        expect(notificationServiceMock.error).toHaveBeenCalledWith('formLetter.sender.create.error');
    });

    it('save should include signature in request payload', async () => {
        mockFormComponentData.senderForm.valid = true;

        const mockSignature = new File(['test'], 'signature.png', {type: 'image/png'});
        mockFormComponentData.senderForm.getRawValue = () => ({
            description: 'Test',
            surname: 'Doe',
            givenName: 'John',
            senderFunctionId: 'f1',
            departmentId: 'd1',
            officeId: 'o1',
            streetGerman: 'Main St',
            streetFrench: 'Rue Principale',
            streetItalian: 'Via Principale',
            streetRomansh: 'Via Principala',
            zip: '5400',
            cityGerman: 'Baden',
            cityFrench: 'Bâle',
            cityItalian: 'Basilea',
            cityRomansh: 'Basilea',
            phone: '0041123456789',
            email: 'john@example.com',
            website: undefined,
        });
        mockFormComponentData.signature = mockSignature;

        component.save();

        const callArgs = (formLettersSenderServiceMock.createFormLettersSender as jest.Mock).mock.calls[0][0];
        expect(callArgs.signature).toBe(mockSignature);
        await fixture.whenStable();
    });

    it('save should deactivate API notification on interceptor', async () => {
        mockFormComponentData.senderForm.valid = true;

        component.save();

        expect(interceptorEventsMock.deactivateNotificationOnNextAPICalls).toHaveBeenCalled();
        await fixture.whenStable();
    });
});
