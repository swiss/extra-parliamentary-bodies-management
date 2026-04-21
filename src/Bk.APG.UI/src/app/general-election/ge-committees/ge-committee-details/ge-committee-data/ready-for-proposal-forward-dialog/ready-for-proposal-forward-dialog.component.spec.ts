import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {EiamAssignment} from '@api/EiamAssignment';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {MockPipes} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {GeneralElectionCommitteeDetailsService} from '../../ge-committee-details.service';
import {GeneralElectionCommitteeDataService} from '../ge-committee-data.service';
import {ReadyForProposalForwardDialogComponent} from './ready-for-proposal-forward-dialog.component';

describe('ReadyForProposalForwardDialogComponent', () => {
    let component: ReadyForProposalForwardDialogComponent;
    let fixture: ComponentFixture<ReadyForProposalForwardDialogComponent>;
    let dataService: jest.Mocked<GeneralElectionCommitteeDataService>;
    let detailsService: jest.Mocked<GeneralElectionCommitteeDetailsService>;
    let notificationService: jest.Mocked<ObNotificationService>;
    let dialogRef: jest.Mocked<MatDialogRef<ReadyForProposalForwardDialogComponent>>;

    const mockDialogData = {
        committeeId: 'committee-123',
    };

    beforeEach(async () => {
        dataService = {
            getAssignmentsForReadyForProposalForward: jest.fn(() => of([])),
            forwardReadyForProposal: jest.fn(() => of(undefined)),
        } as unknown as jest.Mocked<GeneralElectionCommitteeDataService>;
        detailsService = {
            reload$: {next: jest.fn()},
        } as unknown as jest.Mocked<GeneralElectionCommitteeDetailsService>;

        notificationService = {
            success: jest.fn(),
            error: jest.fn(),
        } as unknown as jest.Mocked<ObNotificationService>;
        dialogRef = {close: jest.fn()} as unknown as jest.Mocked<MatDialogRef<ReadyForProposalForwardDialogComponent>>;
        const translateServiceMock = {
            getCurrentLang: jest.fn(() => 'en'),
            onLangChange: new Subject(),
        };

        await TestBed.configureTestingModule({
            imports: [ReadyForProposalForwardDialogComponent, MockPipes(TranslatePipe)],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: MAT_DIALOG_DATA, useValue: mockDialogData},
                {provide: MatDialogRef, useValue: dialogRef},
                {provide: GeneralElectionCommitteeDetailsService, useValue: detailsService},
                {provide: ObNotificationService, useValue: notificationService},
            ],
        })
            .overrideComponent(ReadyForProposalForwardDialogComponent, {
                set: {
                    providers: [{provide: GeneralElectionCommitteeDataService, useValue: dataService}],
                },
            })
            .compileComponents();

        fixture = TestBed.createComponent(ReadyForProposalForwardDialogComponent);
        component = fixture.componentInstance;
    });

    afterEach(() => jest.clearAllMocks());

    it('should create', () => {
        fixture.detectChanges();
        expect(component).toBeTruthy();
    });

    it('should load available assignments', () => {
        const mockAssignments: EiamAssignment[] = [{id: 'assignment-1', text: 'Assignment 1'}];
        dataService.getAssignmentsForReadyForProposalForward.mockReturnValue(of(mockAssignments));

        fixture.detectChanges();

        expect(dataService.getAssignmentsForReadyForProposalForward).toHaveBeenCalledWith('committee-123');
        expect(component.availableAssignments()).toEqual(mockAssignments);
    });

    it('should forward ready for proposal when form is valid', () => {
        component.form.patchValue({
            forwardToId: 'assignment-1',
            description: 'Test description',
        });

        component.forward();

        expect(dialogRef.close).toHaveBeenCalled();
        expect(dataService.forwardReadyForProposal).toHaveBeenCalledWith('committee-123', {
            forwardToId: 'assignment-1',
            description: 'Test description',
        });
        expect(detailsService.reload$.next).toHaveBeenCalled();
        expect(notificationService.success).toHaveBeenCalledWith('generalElection.committee.data.readyForProposal.forward.success');
    });

    it('should not forward when form is invalid', () => {
        component.form.patchValue({
            forwardToId: '',
            description: '',
        });

        component.forward();

        expect(dialogRef.close).not.toHaveBeenCalled();
        expect(dataService.forwardReadyForProposal).not.toHaveBeenCalled();
        expect(component.form.touched).toBe(true);
    });

    it('should show error notification on forward failure', () => {
        dataService.forwardReadyForProposal.mockReturnValue(throwError(() => new Error('Error')));
        component.form.patchValue({
            forwardToId: 'assignment-1',
            description: 'Test description',
        });

        component.forward();

        expect(notificationService.error).toHaveBeenCalledWith('generalElection.committee.data.readyForProposal.forward.error');
    });
});
