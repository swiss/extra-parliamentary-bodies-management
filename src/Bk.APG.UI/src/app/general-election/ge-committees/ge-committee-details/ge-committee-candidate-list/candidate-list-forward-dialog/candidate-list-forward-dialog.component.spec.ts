/* eslint-disable @typescript-eslint/no-explicit-any */
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObNotificationService} from '@oblique/oblique';
import {MockPipes} from 'ng-mocks';
import {of, Subject, throwError} from 'rxjs';
import {GeneralElectionCommitteeDetailsService} from '../../ge-committee-details.service';
import {GeneralElectionCommitteeCandidateListService} from '../ge-committee-candidate-list.service';
import {CandidateListForwardDialogComponent} from './candidate-list-forward-dialog.component';

describe('CandidateListForwardDialogComponent', () => {
    let component: CandidateListForwardDialogComponent;
    let fixture: ComponentFixture<CandidateListForwardDialogComponent>;
    let candidateListService: jest.Mocked<GeneralElectionCommitteeCandidateListService>;
    let detailsService: jest.Mocked<GeneralElectionCommitteeDetailsService>;
    let notificationService: jest.Mocked<ObNotificationService>;

    const mockDialogData = {
        committeeId: 'committee-123',
        candidateIds: ['candidate-1', 'candidate-2'],
    };

    beforeEach(async () => {
        candidateListService = {
            getAssignmentsForCandidateListForward: jest.fn(() => of([])),
            forwardCandidateList: jest.fn(() => of(undefined)),
        } as any;
        detailsService = {
            reload$: {next: jest.fn()},
        } as any;

        notificationService = {
            success: jest.fn(),
            error: jest.fn(),
        } as any;
        const translateServiceMock = {
            currentLang: 'en',
            onLangChange: new Subject(),
        };

        await TestBed.configureTestingModule({
            imports: [CandidateListForwardDialogComponent, MockPipes(TranslatePipe)],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: MAT_DIALOG_DATA, useValue: mockDialogData},
                {provide: GeneralElectionCommitteeCandidateListService, useValue: candidateListService},
                {provide: GeneralElectionCommitteeDetailsService, useValue: detailsService},
                {provide: ObNotificationService, useValue: notificationService},
            ],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();

        fixture = TestBed.createComponent(CandidateListForwardDialogComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => jest.clearAllMocks());

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('ngOnInit', () => {
        it('should load available assignments', () => {
            const mockAssignments = [{id: 'assignment-1', text: 'Assignment 1'}];
            candidateListService.getAssignmentsForCandidateListForward.mockReturnValue(of(mockAssignments as any));

            component.ngOnInit();

            expect(candidateListService.getAssignmentsForCandidateListForward).toHaveBeenCalledWith('committee-123');
            expect(component.availableAssignments()).toEqual(mockAssignments);
        });
    });

    describe('forward', () => {
        it('should forward candidate list when form is valid', () => {
            component.form.patchValue({
                forwardToId: 'assignment-1',
                description: 'Test description',
            });

            component.forward();

            expect(candidateListService.forwardCandidateList).toHaveBeenCalledWith('committee-123', {
                candidateIds: ['candidate-1', 'candidate-2'],
                forwardToId: 'assignment-1',
                description: 'Test description',
            });
            expect(detailsService.reload$.next).toHaveBeenCalled();
            expect(notificationService.success).toHaveBeenCalledWith('generalElection.committee.candidateList.forward.success');
        });

        it('should not forward candidate list when form is invalid', () => {
            component.form.patchValue({
                forwardToId: '',
                description: '',
            });

            component.forward();

            expect(candidateListService.forwardCandidateList).not.toHaveBeenCalled();
            expect(component.form.touched).toBe(true);
        });

        it('should show error notification on forward failure', () => {
            candidateListService.forwardCandidateList.mockReturnValue(throwError(() => new Error('Error')));
            component.form.patchValue({
                forwardToId: 'assignment-1',
                description: 'Test description',
            });

            component.forward();

            expect(notificationService.error).toHaveBeenCalledWith('generalElection.committee.candidateList.forward.error');
        });
    });
});
