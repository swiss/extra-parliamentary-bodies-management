import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import {MatCheckbox} from '@angular/material/checkbox';
import {MatOption} from '@angular/material/core';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatIconModule} from '@angular/material/icon';
import {MatInput} from '@angular/material/input';
import {MatRadioModule} from '@angular/material/radio';
import {MatSelect} from '@angular/material/select';
import {MatTooltipModule} from '@angular/material/tooltip';
import {ActivatedRoute, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {CommitteeJustificationUpdate} from '@api/CommitteeJustificationUpdate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObErrorMessagesModule, ObInputClearModule, ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {EntityAuditLogService} from '@shared/entity-audit-log/entity-audit-log.service';
import {RichTextEditorComponent} from '@shared/rich-text-editor/rich-text-editor.component';
import {MockComponent, MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {CommitteesService} from '../../committees.service';
import {CommitteeDetailsService} from '../committee-details.service';
import {CommitteeJustificationsComponent} from './committee-justifications.component';

describe('CommitteeJustificationsComponent', () => {
    let component: CommitteeJustificationsComponent;
    let fixture: ComponentFixture<CommitteeJustificationsComponent>;

    const activatedRouteMock = {snapshot: {params: {id: '123'}}};
    const reloadSubject = new Subject<void>();

    const routerMock = {
        navigate: jest.fn(),
    };

    const committeesServiceMock = {
        getCommitteeJustificationForUpdate: jest.fn(() => of()),
        updateCommitteeJustification: jest.fn(),
        reload$: reloadSubject,
    };

    const committeeDetailsServiceMock = {
        committeeDetails: signal<CommitteeDetails>({canEdit: true} as CommitteeDetails),
    };

    const notificationServiceMock: Partial<ObNotificationService> = {
        success: jest.fn(),
        error: jest.fn(),
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
                CommitteeJustificationsComponent,
                MockComponent(RichTextEditorComponent),
            ],
            providers: [
                {provide: CommitteeDetailsService, useValue: committeeDetailsServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: EntityAuditLogService, useValue: entityAuditLogServiceMock},
                provideHttpClient(),
                provideHttpClientTesting(),
            ],
        })
            .overrideTemplateUsingTestingModule(CommitteeJustificationsComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(CommitteeJustificationsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should set canEdit', () => {
        expect(component.canEdit()).toBe(true);
    });

    describe('save', () => {
        it('should handle updateCommitteeJustification error', () => {
            const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
            const reloadEntityAuditLogNextSpy = jest.spyOn(reloadEntityAuditLogSubject, 'next');
            committeesServiceMock.updateCommitteeJustification.mockReturnValue(throwError(() => new Error('Create failed')));

            component.save();

            expect(committeesServiceMock.updateCommitteeJustification).toHaveBeenCalled();
            expect(reloadNextSpy).not.toHaveBeenCalled();
            expect(reloadEntityAuditLogNextSpy).not.toHaveBeenCalled();
            expect(notificationServiceMock.error).toHaveBeenCalledWith('committee.details.justification.error');
        });

        it('should call updateCommitteeJustification and handle success', async () => {
            committeesServiceMock.updateCommitteeJustification.mockReturnValue(of({}));
            const justificationBeforeSave = {id: '123', justificationMembers: 'my justification'} as CommitteeJustificationUpdate;

            component.committeeJustificationUpdate.set(justificationBeforeSave);
            // eslint-disable-next-line dot-notation
            component['unmodifiedCommitteeJustification'] = {
                id: '999',
                justificationGenders: 'justificationGenders',
                rowVersion: 123,
            } as CommitteeJustificationUpdate;

            component.save();

            expect(committeesServiceMock.updateCommitteeJustification).toHaveBeenCalled();
            expect(routerMock.navigate).toHaveBeenCalledWith([]);

            await fixture.whenStable();

            expect(notificationServiceMock.success).toHaveBeenCalledWith('committee.details.justification.success');
        });
    });
});
