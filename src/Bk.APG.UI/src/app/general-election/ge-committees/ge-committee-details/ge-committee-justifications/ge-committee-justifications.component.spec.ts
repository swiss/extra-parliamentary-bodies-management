import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatOption} from '@angular/material/core';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatIconModule} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';
import {MatFormField, MatLabel, MatSelect} from '@angular/material/select';
import {MatTooltipModule} from '@angular/material/tooltip';
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {GeneralElectionCommitteeJustificationUpdate} from '@api/GeneralElectionCommitteeJustificationUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {ErrorService} from '@shared/error-service.service';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {MockComponent, MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {GeneralElectionCommitteesService} from '../../ge-committees.service';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';
import {GeneralElectionCommitteeJustificationsComponent} from './ge-committee-justifications.component';

describe('GeCommitteeJustificationsComponent', () => {
    let component: GeneralElectionCommitteeJustificationsComponent;
    let fixture: ComponentFixture<GeneralElectionCommitteeJustificationsComponent>;

    const activatedRouteMock = {snapshot: {params: {id: '123'}}};
    const reloadSubject = new Subject<void>();

    const routerMock = {
        navigate: jest.fn(),
    };

    const generalElectionCommitteesServiceMock = {
        getGeneralElectionCommitteeJustificationForUpdate: jest.fn(() => of()),
        updateGeneralElectionCommitteeJustification: jest.fn(),
    };

    const generalElectionCommitteeDetailsServiceMock = {
        committeeDetails: signal<GeneralElectionCommitteeDetails>({
            canEdit: true,
            isValidated: true,
            generalGenderMeasure: 'test',
            generalLanguageMeasure: 'test',
        } as GeneralElectionCommitteeDetails),
        reload$: reloadSubject,
    };

    const notificationServiceMock: Partial<ObNotificationService> = {
        success: jest.fn(),
        error: jest.fn(),
    };

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    const reloadEntityAuditLogSubject = new Subject<void>();
    const entityAuditLogServiceMock = {
        reload$: reloadEntityAuditLogSubject,
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [
                MockModule(ReactiveFormsModule),
                MockModule(MatIconModule),
                MockModule(ObInputClearModule),
                MockModule(ObErrorMessagesModule),
                MockModule(MatDatepickerModule),
                MockModule(MatAutocompleteModule),
                MockModule(MatRadioModule),
                MockModule(MatTooltipModule),
                MockComponents(MatFormField, MatCheckbox, MatOption, MatSelect, MatDatepicker, MatDatepickerToggle),
                MockDirectives(MatInput, MatLabel, ObUnsavedChangesDirective),
                MockPipe(TranslatePipe),
                GeneralElectionCommitteeJustificationsComponent,
                MockComponent(RichTextEditorComponent),
            ],
            providers: [
                {provide: GeneralElectionCommitteeDetailsService, useValue: generalElectionCommitteeDetailsServiceMock},
                {provide: GeneralElectionCommitteesService, useValue: generalElectionCommitteesServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: EntityAuditLogService, useValue: entityAuditLogServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
        })
            .overrideTemplateUsingTestingModule(GeneralElectionCommitteeJustificationsComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(GeneralElectionCommitteeJustificationsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('save', () => {
        it('should handle updateGeneralElectionCommitteeJustification error', () => {
            const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
            const reloadEntityAuditLogNextSpy = jest.spyOn(reloadEntityAuditLogSubject, 'next');
            generalElectionCommitteesServiceMock.updateGeneralElectionCommitteeJustification.mockReturnValue(throwError(() => new Error('Create failed')));

            component.save();

            expect(generalElectionCommitteesServiceMock.updateGeneralElectionCommitteeJustification).toHaveBeenCalled();
            expect(reloadNextSpy).not.toHaveBeenCalled();
            expect(reloadEntityAuditLogNextSpy).not.toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('committee.details.justification.error');
        });

        it('should call updateGeneralElectionCommitteeJustification and handle success', async () => {
            generalElectionCommitteesServiceMock.updateGeneralElectionCommitteeJustification.mockReturnValue(of({}));
            const justificationBeforeSave = {id: '123', justificationMembers: 'my justification'} as GeneralElectionCommitteeJustificationUpdate;

            component.committeeJustificationUpdate.set(justificationBeforeSave);
            // eslint-disable-next-line dot-notation
            component['unmodifiedCommitteeJustification'] = {
                id: '999',
                selectionProcedure: 'selectionProcedure',
                justificationGenders: 'justificationGenders',
                rowVersion: 123,
            } as GeneralElectionCommitteeJustificationUpdate;

            component.save();

            expect(generalElectionCommitteesServiceMock.updateGeneralElectionCommitteeJustification).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith([]);

            await fixture.whenStable();

            expect(notificationServiceMock.success).toHaveBeenCalledWith('committee.details.justification.success');
        });
    });
});
