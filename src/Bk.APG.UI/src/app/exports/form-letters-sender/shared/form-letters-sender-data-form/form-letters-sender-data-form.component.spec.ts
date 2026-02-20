import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObDropZoneComponent, ObErrorMessagesDirective, ObEUploadEventType, ObIUploadEvent, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockDirectives, MockPipe} from 'ng-mocks';
import {FormLettersSenderDataFormComponent} from './form-letters-sender-data-form.component';

describe('FormLettersSenderDataFormComponent', () => {
    let component: FormLettersSenderDataFormComponent;
    let fixture: ComponentFixture<FormLettersSenderDataFormComponent>;

    const masterDataServiceMock = {
        formLetterSenderFunctions: jest.fn().mockReturnValue([{id: 'f1', description: 'Function 1'}]),
        departments: jest.fn().mockReturnValue([{id: 'd1', description: 'Department 1'}]),
        offices: jest.fn().mockReturnValue([
            {id: 'o1', departmentId: 'dep-a', name: 'Office A'},
            {id: 'o2', departmentId: 'dep-b', name: 'Office B'},
            {id: 'o3', departmentId: 'dep-a', name: 'Office C'},
        ]),
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                FormLettersSenderDataFormComponent,
                ReactiveFormsModule,
                MockComponents(ObDropZoneComponent),
                MockDirectives(ObUnsavedChangesDirective, ObErrorMessagesDirective),
                MockPipe(TranslatePipe),
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: TranslateService, useValue: {instant: jest.fn((key: string) => key)}},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(FormLettersSenderDataFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should initialize static signature config', () => {
        expect(component.acceptedSignatureFormats).toContain('.png');
        expect(component.maxSignatureFileSizeInMB).toBe(5);
        expect(component.signature).toBeUndefined();
        expect(component.signatureFileName).toBe('');
    });

    it('should build form with required controls and validators', () => {
        expect(component.senderForm.controls.description).toBeTruthy();
        expect(component.senderForm.controls.email).toBeTruthy();
        expect(component.senderForm.controls.website).toBeTruthy();

        expect(component.senderForm.controls.description.hasError('required')).toBe(true);
        expect(component.senderForm.controls.surname.hasError('required')).toBe(true);

        component.senderForm.controls.email.setValue('not-an-email');
        expect(component.senderForm.controls.email.hasError('email')).toBe(true);

        component.senderForm.controls.phone.setValue('123456789012345678901');
        expect(component.senderForm.controls.phone.hasError('maxlength')).toBe(true);
    });

    it('should expose sender functions and departments from master data', () => {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const senderFunctions = (component as any).senderFunctions();
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const departments = (component as any).departments();

        expect(senderFunctions).toEqual([{id: 'f1', description: 'Function 1'}]);
        expect(departments).toEqual([{id: 'd1', description: 'Department 1'}]);
    });

    it('should filter department offices by selected department id', () => {
        component.senderForm.controls.departmentId.setValue('dep-a');
        fixture.detectChanges();

        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const departmentOffices = (component as any).departmentOffices();
        expect(departmentOffices).toEqual([
            {id: 'o1', departmentId: 'dep-a', name: 'Office A'},
            {id: 'o3', departmentId: 'dep-a', name: 'Office C'},
        ]);
    });

    it('should set signature data and mark form dirty on upload CHOSEN event with file', () => {
        component.senderForm.markAsPristine();
        const file = new File(['signature-content'], 'signature.png', {type: 'image/png'});
        const event = {
            type: ObEUploadEventType.CHOSEN,
            files: [file],
        } as ObIUploadEvent;

        component.uploadEvent(event);

        expect(component.signature).toBe(file);
        expect(component.signatureFileName).toBe('signature.png');
        expect(component.senderForm.dirty).toBe(true);
    });

    it('should ignore upload CHOSEN event without files', () => {
        const previousSignature = new File(['existing'], 'existing.png', {type: 'image/png'});
        component.signature = previousSignature;
        component.signatureFileName = previousSignature.name;
        component.senderForm.markAsPristine();

        const event = {
            type: ObEUploadEventType.CHOSEN,
            files: [],
        } as ObIUploadEvent;

        component.uploadEvent(event);

        expect(component.signature).toBe(previousSignature);
        expect(component.signatureFileName).toBe('existing.png');
        expect(component.senderForm.dirty).toBe(false);
    });

    it('should clear signature and mark form dirty', () => {
        component.signature = new File(['existing'], 'existing.png', {type: 'image/png'});
        component.signatureFileName = 'existing.png';
        component.senderForm.markAsPristine();

        component.clearSignature();

        expect(component.signature).toBeUndefined();
        expect(component.signatureFileName).toBe('');
        expect(component.senderForm.dirty).toBe(true);
    });
});
