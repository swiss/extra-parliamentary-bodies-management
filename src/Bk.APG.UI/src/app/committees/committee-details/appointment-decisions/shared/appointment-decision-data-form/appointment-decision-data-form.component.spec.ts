import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule, FormGroup, Validators} from '@angular/forms';
import {MatOption} from '@angular/material/core';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatFormFieldModule} from '@angular/material/form-field';
import {MatSelect, MatSelectChange} from '@angular/material/select';
import {TranslateModule, TranslatePipe} from '@ngx-translate/core';
import {ObEUploadEventType, ObFileUploadModule, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockDirective, MockModule, MockPipe} from 'ng-mocks';
import {ConfigsService} from '../../../../../configs.service';
import {AppointmentDecisionDataFormComponent} from './appointment-decision-data-form.component';

describe('AppointmentDecisionDataFormComponent', () => {
    let component: AppointmentDecisionDataFormComponent;
    let fixture: ComponentFixture<AppointmentDecisionDataFormComponent>;

    const appointmentDecisionUpdate = {
        id: '1',
        appointmentDecisionDate: new Date(2025, 1, 1),
        appointmentDecisionTypeId: '2',
        appointmentDecisionLinkTypeId: '3',
        text: 'text',
        link: 'link',
        documents: [
            {
                id: '11',
                displayName: 'dName',
                isOriginal: true,
                languageId: 'germanLanguageId',
                file: {} as File,
            },
        ],
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    const notificationServiceMock = {
        success: jest.fn(),
        error: jest.fn(),
    };

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                appointmentDecisionType: {
                    decisionFederalCouncilId: 'decisionFederalCouncilId',
                    institutionId: 'institutionId',
                    reportId: 'reportId',
                },
                appointmentDecisionLinkType: {
                    exeLinkTypeId: 'exeLinkTypeId',
                    standardLinkTypeId: 'standardLinkTypeId',
                },
                language: {
                    germanLanguageId: 'germanLanguageId',
                    frenchLanguageId: 'frenchLanguageId',
                    italianLanguageId: 'italianLanguageId',
                    romanshLanguageId: 'romanshLanguageId',
                },
            },
        },
    };

    const masterDataServiceMock = {
        appointmentDecisionTypes: signal([
            {id: 'id1', text: 'type1', description: 'desc1'},
            {id: 'id2', text: 'type2', description: 'desc2'},
        ]),
        appointmentDecisionLinkTypes: signal([
            {id: 'id1', text: 'office1', description: 'desc1'},
            {id: 'id2', text: 'office2', description: 'desc2'},
        ]),
        languages: signal([
            {id: 'id1', text: 'lang1', description: 'desc1'},
            {id: 'id2', text: 'lang2', description: 'desc2'},
        ]),
    } as unknown as Partial<MasterDataService>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                AppointmentDecisionDataFormComponent,
                MockModule(ReactiveFormsModule),
                MockModule(MatFormFieldModule),
                MockModule(MatDatepickerModule),
                MockModule(ObFileUploadModule),
                MockComponents(MatFormField, MatOption, MatSelect, MatDatepicker, MatDatepickerToggle),
                MockPipe(TranslatePipe),
                MockModule(TranslateModule),
                MockDirective(ObUnsavedChangesDirective),
            ],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(AppointmentDecisionDataFormComponent);
        component = fixture.componentInstance;

        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should return appointmentDecisionTypes correctly', () => {
        expect(component.appointmentDecisionTypes().length).toBe(2);
    });

    it('should return appointmentDecisionLinkTypes correctly', () => {
        expect(component.appointmentDecisionLinkTypes().length).toBe(2);
    });

    it.each([
        ['decisionFederalCouncilId', true],
        ['institutionId', false],
    ])('should return isTypeDecisionFederalCouncilSelected correctly', (selectedValue, expected) => {
        component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue(selectedValue);
        expect(component.isTypeDecisionFederalCouncilSelected()).toBe(expected);
    });

    it.each([
        ['decisionFederalCouncilId', false],
        ['institutionId', true],
    ])('should return institutionId correctly', (selectedValue, expected) => {
        component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue(selectedValue);
        expect(component.isInstitutionSelected()).toBe(expected);
    });

    it.each([
        ['decisionFederalCouncilId', false],
        ['reportId', true],
    ])('should return isReportSelected correctly', (selectedValue, expected) => {
        component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue(selectedValue);
        expect(component.isReportSelected()).toBe(expected);
    });

    it('should handle too many files during upload', () => {
        const file = {} as File;
        component.uploadEvent({files: [file, file, file, file, file], type: ObEUploadEventType.CHOSEN});
        expect(notificationServiceMock.error).toHaveBeenCalledWith('appointmentDecision.files.count.error');
    });

    it('should handle upload files', () => {
        const file1 = {name: 'filename1.pdf'} as File;
        const file2 = {name: 'filename2.pdf'} as File;
        component.files = [];
        component.uploadEvent({files: [file1, file2], type: ObEUploadEventType.CHOSEN});
        expect(component.files).toHaveLength(2);
    });

    it('should remove uploaded document', () => {
        const file1 = {name: 'filename1.pdf'} as File;
        const file2 = {name: 'filename2.pdf'} as File;
        component.files = [file1, file2];
        component.removeUploadedDocument(1);
        expect(component.files).toHaveLength(1);
    });

    describe('updateMode', () => {
        beforeEach(() => {
            component.appointmentDecisionModification.set(appointmentDecisionUpdate);
        });
        it('should disable decisionType', () => {
            component.isUpdateMode = true;
            fixture.detectChanges();
            expect(component.appointmentDecisionForm.controls.appointmentDecisionTypeId.enabled).toBe(false);
        });

        it('should disable decisionLinkType for report', () => {
            component.isUpdateMode = true;
            fixture.detectChanges();
            component.setDecisionType({value: configsServiceMock.frontendConfig.entityIds.appointmentDecisionType.reportId} as MatSelectChange);
            expect(component.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.enabled).toBe(false);
            expect(component.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.value).toBe(
                configsServiceMock.frontendConfig.entityIds.appointmentDecisionLinkType.standardLinkTypeId
            );
        });

        it.each([
            configsServiceMock.frontendConfig.entityIds.appointmentDecisionType.institutionId,
            configsServiceMock.frontendConfig.entityIds.appointmentDecisionType.decisionFederalCouncilId,
        ])('should disable decisionLinkType for decision or institution', type => {
            component.isUpdateMode = true;
            fixture.detectChanges();
            component.setDecisionType({value: type} as MatSelectChange);
            expect(component.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.enabled).toBe(false);
            expect(component.appointmentDecisionForm.controls.appointmentDecisionLinkTypeId.value).toBe(
                configsServiceMock.frontendConfig.entityIds.appointmentDecisionLinkType.exeLinkTypeId
            );
        });

        it('rebuild existing document list properly', () => {
            component.isUpdateMode = true;
            fixture.detectChanges();
            expect(component.documentsForm.length).toBe(1);
            const formGroup = component.documentsForm.at(0);
            expect(formGroup.get('displayName')!.value).toBe('dName');
            expect(formGroup.get('languageId')!.value).toBe(configsServiceMock.frontendConfig.entityIds.language.germanLanguageId);
            expect(formGroup.get('id')!.value).toBe('11');
        });

        it('adds a new document to the list properly', () => {
            component.isUpdateMode = true;
            fixture.detectChanges();
            component.addNewToDocumentForm({name: 'file1_FR.docx'} as File);
            expect(component.documentsForm.length).toBe(2);
            const formGroup = component.documentsForm.at(1);
            expect(formGroup.get('displayName')!.value).toBe('file1_FR.docx');
            expect(formGroup.get('languageId')!.value).toBe(configsServiceMock.frontendConfig.entityIds.language.frenchLanguageId);
            expect(formGroup.get('id')!.value).toBe('');
        });
    });

    describe('isOriginal control effect', () => {
        beforeEach(() => {
            const file = {name: 'test.pdf'} as File;
            component.addNewToDocumentForm(file);
        });

        it('should add isOriginal control when institution is selected', () => {
            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('institutionId');

            fixture.detectChanges();

            const firstDocumentGroup = component.documentsForm.controls[0] as FormGroup;
            expect(firstDocumentGroup.contains('isOriginal')).toBe(true);
            expect(firstDocumentGroup.get('isOriginal')?.hasValidator(Validators.required)).toBe(true);
        });

        it('should remove isOriginal control when non-institution is selected', () => {
            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('institutionId');
            fixture.detectChanges();

            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('someOtherType');
            fixture.detectChanges();

            const firstDocumentGroup = component.documentsForm.controls[0] as FormGroup;
            expect(firstDocumentGroup.contains('isOriginal')).toBe(false);
        });

        it('should not add isOriginal control when appointment decision type is empty', () => {
            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('');
            fixture.detectChanges();

            const firstDocumentGroup = component.documentsForm.controls[0] as FormGroup;
            expect(firstDocumentGroup.contains('isOriginal')).toBe(false);
        });

        it('should handle multiple documents when adding isOriginal control', () => {
            const file1 = {name: 'test1.pdf'} as File;
            const file2 = {name: 'test2.pdf'} as File;
            component.addNewToDocumentForm(file1);
            component.addNewToDocumentForm(file2);

            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('institutionId');
            fixture.detectChanges();

            for (let i = 0; i < component.documentsForm.controls.length; i++) {
                const documentGroup = component.documentsForm.controls[i] as FormGroup;
                expect(documentGroup.contains('isOriginal')).toBe(true);
                expect(documentGroup.get('isOriginal')?.hasValidator(Validators.required)).toBe(true);
            }
        });

        it('should handle multiple documents when removing isOriginal control', () => {
            const file1 = {name: 'test1.pdf'} as File;
            const file2 = {name: 'test2.pdf'} as File;
            component.addNewToDocumentForm(file1);
            component.addNewToDocumentForm(file2);

            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('institutionId');
            fixture.detectChanges();

            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('someOtherType');
            fixture.detectChanges();

            for (let i = 0; i < component.documentsForm.controls.length; i++) {
                const documentGroup = component.documentsForm.controls[i] as FormGroup;
                expect(documentGroup.contains('isOriginal')).toBe(false);
            }
        });

        it('should not add isOriginal control if it already exists', () => {
            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('institutionId');
            fixture.detectChanges();

            const firstDocumentGroup = component.documentsForm.controls[0] as FormGroup;
            const originalControl = firstDocumentGroup.get('isOriginal');

            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('institutionId');
            fixture.detectChanges();

            expect(firstDocumentGroup.get('isOriginal')).toBe(originalControl);
        });

        it('should not remove isOriginal control if it does not exist', () => {
            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('someOtherType');
            fixture.detectChanges();

            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('someOtherType');
            fixture.detectChanges();

            const firstDocumentGroup = component.documentsForm.controls[0] as FormGroup;
            expect(firstDocumentGroup.contains('isOriginal')).toBe(false);
        });

        it('should add isOriginal control when document is added after institution is selected', () => {
            component.appointmentDecisionForm.controls.appointmentDecisionTypeId.setValue('institutionId');
            fixture.detectChanges();

            component.documentsForm.clear();

            const file = {name: 'new-test.pdf'} as File;
            component.addNewToDocumentForm(file);
            fixture.detectChanges();

            const firstDocumentGroup = component.documentsForm.controls[0] as FormGroup;
            expect(firstDocumentGroup.contains('isOriginal')).toBe(true);
            expect(firstDocumentGroup.get('isOriginal')?.hasValidator(Validators.required)).toBe(true);
        });
    });
});
