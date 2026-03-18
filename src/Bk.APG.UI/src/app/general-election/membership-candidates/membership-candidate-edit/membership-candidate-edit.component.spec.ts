import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {MembershipCandidateUpdate} from '@api/MembershipCandidateUpdate';
import {ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {of} from 'rxjs';
import {GeneralElectionCommitteeDetailsService} from '../../ge-committees/ge-committee-details/ge-committee-details.service';
import {MembershipCandidateService} from '../membership-candidate-service';
import {MembershipCandidateEditComponent} from './membership-candidate-edit.component';

describe('MembershipCandidateEditComponent', () => {
    let component: MembershipCandidateEditComponent;
    let fixture: ComponentFixture<MembershipCandidateEditComponent>;

    const routerMock = {
        navigate: jest.fn(),
    };

    const activatedRouteMock = {
        snapshot: {params: {membershipCandidateId: 'candidate-1'}},
        parent: {snapshot: {params: {id: 'ge-committee-1'}}},
    };

    const membershipCandidateServiceMock = {
        getMembershipCandidateForUpdate: jest.fn(),
        updateMembershipCandidate: jest.fn(),
    };

    const generalElectionCommitteeDetailsServiceMock = {
        generalElectionCommitteeDetails: jest.fn(),
    };

    const notificationServiceMock = {
        success: jest.fn(),
        error: jest.fn(),
    };

    const httpApiInterceptorEventsMock = {
        deactivateNotificationOnNextAPICalls: jest.fn(),
    };

    beforeEach(async () => {
        membershipCandidateServiceMock.getMembershipCandidateForUpdate.mockReturnValue(of({} as MembershipCandidateUpdate));
        generalElectionCommitteeDetailsServiceMock.generalElectionCommitteeDetails.mockReturnValue(
            of({committeeId: 'committee-1', isCandidateListValidated: false} as GeneralElectionCommitteeDetails)
        );

        await TestBed.configureTestingModule({
            imports: [MembershipCandidateEditComponent],
            providers: [
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: MembershipCandidateService, useValue: membershipCandidateServiceMock},
                {provide: GeneralElectionCommitteeDetailsService, useValue: generalElectionCommitteeDetailsServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: httpApiInterceptorEventsMock},
            ],
        })
            .overrideTemplate(MembershipCandidateEditComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(MembershipCandidateEditComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should navigate back to members tab when candidate list is completed', () => {
        component.generalElectionCommittee.set({committeeId: 'committee-1', isCandidateListValidated: true} as GeneralElectionCommitteeDetails);

        component.back();

        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'committees', 'committee-1'], {
            replaceUrl: true,
            queryParams: {tab: 'members'},
        });
    });

    it('should navigate back to candidate list tab when candidate list is not completed', () => {
        component.generalElectionCommittee.set({committeeId: 'committee-1', isCandidateListValidated: false} as GeneralElectionCommitteeDetails);

        component.back();

        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'committees', 'committee-1'], {
            replaceUrl: true,
            queryParams: {tab: 'candidateList'},
        });
    });
});
