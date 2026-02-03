import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ActivatedRoute, Router} from '@angular/router';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObGlobalEventsService} from '@oblique/oblique';
import {MembersQuotasComponent} from '@shared/members-quotas/members-quotas.component';
import {MockPipe, MockService} from 'ng-mocks';
import {of} from 'rxjs';
import {GeneralElectionCommitteesService} from '../../ge-committees.service';
import {GeneralElectionCommitteeCandidateListService} from '../ge-committee-candidate-list/ge-committee-candidate-list.service';
import {GeneralElectionCommitteeDetailsService} from '../ge-committee-details.service';
import {GeneralElectionCommitteeMembershipsComponent} from './ge-committee-memberships.component';

describe('GeneralElectionCommitteeMembershipsComponent', () => {
    let component: GeneralElectionCommitteeMembershipsComponent;
    let fixture: ComponentFixture<GeneralElectionCommitteeMembershipsComponent>;

    const activatedRouteMock = {snapshot: {params: {id: '123'}}};
    const translateServiceMock = {
        currentLang: 'en',
        onLangChange: of({lang: 'en'}),
        instant: jest.fn().mockReturnValue('Bis 10%'),
    };
    const routerMock = {
        navigate: jest.fn(),
    };
    const generalElectionCommitteesServiceMock = {
        getGeneralElectionCommitteeMembers: jest.fn().mockReturnValue(
            of({
                committeeQuotas: {},
                activeMemberships: [
                    {id: '1', surname: 'Clark', givenName: 'Jim', employmentLevel: '10'},
                    {id: '2', surname: 'Schwarzenegger', givenName: 'Arnold', employmentLevel: '0'},
                ],
                inactiveMemberships: [],
            })
        ),
    };
    const geCommitteeDetailsServiceMock = {
        committeeDetails: signal<GeneralElectionCommitteeDetails>({canEdit: true} as GeneralElectionCommitteeDetails),
    };
    const candidateListServiceMock = {
        reload$: of(null),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), GeneralElectionCommitteeMembershipsComponent, MembersQuotasComponent],
            providers: [
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: Router, useValue: routerMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: GeneralElectionCommitteesService, useValue: generalElectionCommitteesServiceMock},
                {provide: GeneralElectionCommitteeDetailsService, useValue: geCommitteeDetailsServiceMock},
                {provide: GeneralElectionCommitteeCandidateListService, useValue: candidateListServiceMock},
                {provide: ObGlobalEventsService, useValue: MockService<ObGlobalEventsService>},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(GeneralElectionCommitteeMembershipsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should navigate to membership candidate edit when opening membership candidate', () => {
        component.openMembershipCandidate('candidate-1');

        expect(routerMock.navigate).toHaveBeenCalledWith(['general-election', 'committees', '123', 'membership-candidate', 'candidate-1']);
    });
});
