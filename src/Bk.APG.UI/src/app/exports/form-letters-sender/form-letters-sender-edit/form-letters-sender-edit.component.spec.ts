import {ComponentFixture, TestBed} from '@angular/core/testing';
import {FormGroup} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {FormLettersSenderUpdate} from '@api/FormLettersSenderUpdate';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {MockComponents, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, throwError} from 'rxjs';
import {FormLettersSenderService} from '../form-letters-sender.service';
import {FormLettersSenderDataFormComponent} from '../shared/form-letters-sender-data-form/form-letters-sender-data-form.component';
import {FormLettersSenderEditComponent} from './form-letters-sender-edit.component';

describe('FormLettersSenderEditComponent', () => {
    let component: FormLettersSenderEditComponent;
    let fixture: ComponentFixture<FormLettersSenderEditComponent>;
    let formLettersSenderServiceMock: Partial<FormLettersSenderService>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let interceptorEventsMock: Partial<ObHttpApiInterceptorEvents>;
    let activatedRouteMock: Partial<ActivatedRoute>;
    let reloadSubject: BehaviorSubject<void>;
    let markAsTouchedSpy: jest.Mock;
    let markAsPristineSpy: jest.Mock;

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    let mockFormComponentData: any;

    const mockSenderUpdate: FormLettersSenderUpdate = {
        id: '123',
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
        signatureFileName: 'signature.png',
        canEditDepartment: true,
    };

    beforeEach(async () => {
        reloadSubject = new BehaviorSubject<void>(undefined);
        markAsTouchedSpy = jest.fn();
        markAsPristineSpy = jest.fn();

        mockFormComponentData = {
            senderForm: {
                valid: true,
                getRawValue: () => ({
                    description: 'Test Sender Updated',
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
            } as unknown as FormGroup,
            signature: undefined,
            signatureFileName: 'signature.png',
        };

        formLettersSenderServiceMock = {
            updateFormLettersSender: jest.fn().mockReturnValue(of(mockSenderUpdate)),
            getFormLettersSenderForUpdate: jest.fn().mockReturnValue(of(mockSenderUpdate)),
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

        // @ts-ignore - ActivatedRoute mock only needs snapshot.params for this test
        activatedRouteMock = {
            snapshot: {
                params: {id: '123'},
            } as unknown as ActivatedRoute['snapshot'],
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), MockComponents(FormLettersSenderDataFormComponent), FormLettersSenderEditComponent],
            providers: [
                {provide: FormLettersSenderService, useValue: formLettersSenderServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: interceptorEventsMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: TranslateService, useValue: {instant: jest.fn((key: string) => key)}},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(FormLettersSenderEditComponent);
        component = fixture.componentInstance;

        // @ts-ignore
        component.formComponent = () => mockFormComponentData;

        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should load sender data on init', () => {
        expect(formLettersSenderServiceMock.getFormLettersSenderForUpdate).toHaveBeenCalledWith('123');
        expect(component.senderId).toBe('123');
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
        expect(formLettersSenderServiceMock.updateFormLettersSender).not.toHaveBeenCalled();
    });

    it('save should call updateFormLettersSender and handle success', async () => {
        mockFormComponentData.senderForm.valid = true;
        mockFormComponentData.senderForm.markAsTouched = markAsTouchedSpy;

        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');

        component.save();

        expect(formLettersSenderServiceMock.updateFormLettersSender).toHaveBeenCalled();
        expect(reloadNextSpy).toHaveBeenCalled();
        await fixture.whenStable();
        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'exports', 'formLetters']);
        expect(notificationServiceMock.success).toHaveBeenCalledWith('formLetter.sender.update.success');
    });

    it('save should handle updateFormLettersSender error', async () => {
        mockFormComponentData.senderForm.valid = true;

        (formLettersSenderServiceMock.updateFormLettersSender as jest.Mock).mockReturnValue(throwError(() => new Error('Update failed')));

        component.save();

        expect(formLettersSenderServiceMock.updateFormLettersSender).toHaveBeenCalled();
        await fixture.whenStable();
        expect(notificationServiceMock.error).toHaveBeenCalledWith('formLetter.sender.update.error');
    });

    it('save should include form data with signature and signature filename', async () => {
        mockFormComponentData.senderForm.valid = true;

        const mockSignature = new File(['test'], 'newsig.png', {type: 'image/png'});
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
        mockFormComponentData.signatureFileName = 'signature.png';

        component.save();

        const callArgs = (formLettersSenderServiceMock.updateFormLettersSender as jest.Mock).mock.calls[0][0];
        expect(callArgs.id).toBe('123');
        expect(callArgs.signature).toBe(mockSignature);
        expect(callArgs.signatureFileName).toBe('signature.png');
        await fixture.whenStable();
    });

    it('save should deactivate API notification on interceptor', async () => {
        mockFormComponentData.senderForm.valid = true;

        component.save();

        expect(interceptorEventsMock.deactivateNotificationOnNextAPICalls).toHaveBeenCalled();
        await fixture.whenStable();
    });
});
