import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ReactiveFormsModule} from '@angular/forms';
import {MatOption} from '@angular/material/autocomplete';
import {MatDatepicker, MatDatepickerModule, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatSelect} from '@angular/material/select';
import {MatTooltipModule} from '@angular/material/tooltip';
import {ActivatedRoute, ActivatedRouteSnapshot, Router} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {MembershipCreate} from '@api/MembershipCreate';
import {PersonDetails} from '@api/PersonDetails';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObNotificationService, ObUnsavedChangesDirective} from '@oblique/oblique';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {MockComponents, MockDirectives, MockModule, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, throwError} from 'rxjs';
import {CommitteeOverviewBasicDataComponent} from '../../committees/committee-details/committee-overview/committee-overview-basic-data/committee-overview-basic-data.component';
import {CommitteesService} from '../../committees/committees.service';
import {PersonsService} from '../../persons/persons.service';
import {MembershipsService} from '../memberships.service';
import {MembershipDataFormComponent} from '../shared/membership-data-form/membership-data-form.component';
import {MembershipCreateComponent} from './membership-create.component';

describe('MembershipCreateComponent', () => {
    let component: MembershipCreateComponent;
    let fixture: ComponentFixture<MembershipCreateComponent>;

    let activatedRouteMock: Partial<ActivatedRoute>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let committeesServiceMock: Partial<CommitteesService>;
    let routerMock: Partial<Router>;
    let reloadSubject: BehaviorSubject<void>;
    const committeeDetails = {id: '1'} as CommitteeDetails;
    const personDetails = {id: '1', surname: 'surname', givenName: 'givenName', birthYear: 2000} as PersonDetails;
    const selectedPerson = {id: '100'} as PersonDetails;
    const selectedCommittee = {id: '200', extraParliamentaryCommission: true} as CommitteeDetails;

    const translateServiceMock = {
        instant: jest.fn(),
    };

    beforeEach(async () => {
        activatedRouteMock = {
            snapshot: {
                params: {id: '123'},
            } as unknown as ActivatedRouteSnapshot,
        };
        const masterDataServiceMock = {
            electionTypes: signal([
                {id: 'id1', text: 'type1', description: 'desc1', isDeleted: false},
                {id: 'id2', text: 'type2', description: 'desc2', isDeleted: true},
            ]),
            electionOffices: signal([
                {id: 'id1', text: 'office1', description: 'desc1', isDeleted: false},
                {id: 'id2', text: 'office2', description: 'desc2', isDeleted: true},
            ]),
            functions: signal([
                {id: 'id1', text: 'function1', description: 'desc1', isDeleted: false},
                {id: 'id2', text: 'function2', description: 'desc2', isDeleted: true},
            ]),
            committeeTypes: jest.fn(),
            terms: jest.fn(),
            membershipAdditions: jest.fn(),
        } as unknown as Partial<MasterDataService>;

        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };

        reloadSubject = new BehaviorSubject<void>(undefined);

        committeesServiceMock = {
            createMember: jest.fn(),
            validateMembership: jest
                .fn()
                .mockReturnValue(of({hasErrors: false, tooManyMembers: false, maximumDurationExceeded: false, isAlreadyActiveMember: false})),
            reload$: reloadSubject,
        };

        const errorServiceMock = {
            getControlError: jest.fn(),
        };

        routerMock = {navigate: jest.fn().mockResolvedValue(true)};

        const personsServiceMock: Partial<PersonsService> = {
            getPersonsByName: jest.fn(),
        };

        const membershipsServiceMock: Partial<MembershipsService> = {
            getMembershipForUpdate: jest.fn().mockReturnValue(of({})),
        };

        await TestBed.configureTestingModule({
            imports: [
                MockPipe(TranslatePipe),
                MockModule(ReactiveFormsModule),
                MockModule(MatDatepickerModule),
                MockModule(MatTooltipModule),
                MockComponents(
                    MatFormField,
                    MatOption,
                    MatSelect,
                    MatDatepicker,
                    MatDatepickerToggle,
                    MembershipDataFormComponent,
                    CommitteeOverviewBasicDataComponent
                ),
                MockDirectives(MatInput, MatLabel, ObUnsavedChangesDirective),
                MembershipCreateComponent,
            ],
            providers: [
                {provide: Router, useValue: routerMock},
                {provide: ActivatedRoute, useValue: activatedRouteMock},
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: MembershipsService, useValue: membershipsServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(MembershipCreateComponent);
        component = fixture.componentInstance;

        component.membershipToCreate = signal({} as unknown as MembershipCreate);
        component.committeeEntity = signal({} as unknown as CommitteeDetails);
        component.personEntity = signal({} as unknown as PersonDetails);
        // @ts-ignore
        component.form = signal({
            pristine: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });
        fixture.detectChanges();
    });

    it('should handle form validation errors without save', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        (committeesServiceMock.createMember as jest.Mock).mockReturnValue(of({id: '1'}));
        component.isForCommittee = true;
        component.committeeEntity.set(committeeDetails);

        component.personSelected = signal(signal(selectedPerson));
        // @ts-ignore
        component.form = signal({
            valid: false,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });
        component.save();
        await fixture.whenStable();
        expect(committeesServiceMock.createMember).not.toHaveBeenCalled();
        expect(reloadNextSpy).not.toHaveBeenCalled();
        expect(routerMock.navigate).not.toHaveBeenCalled();
    });

    it('should handle missing person selection without save', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        (committeesServiceMock.createMember as jest.Mock).mockReturnValue(of({id: '1'}));
        component.isForCommittee = true;

        const committeeDetails = {id: '1'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);
        component.personSelected = signal(signal(undefined));

        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(committeesServiceMock.createMember).not.toHaveBeenCalled();
        expect(reloadNextSpy).not.toHaveBeenCalled();
        expect(routerMock.navigate).not.toHaveBeenCalled();
    });

    it('should handle missing committee selection without save', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        (committeesServiceMock.createMember as jest.Mock).mockReturnValue(of({id: '1'}));
        component.isForPerson = true;

        component.personEntity.set(personDetails);
        component.committeeSelected = signal(signal(undefined));

        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(committeesServiceMock.createMember).not.toHaveBeenCalled();
        expect(reloadNextSpy).not.toHaveBeenCalled();
        expect(routerMock.navigate).not.toHaveBeenCalled();
    });

    it('should handle error at save and show message', async () => {
        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        (committeesServiceMock.createMember as jest.Mock).mockReturnValue(throwError(() => new Error('Create failed')));
        component.isForPerson = true;

        component.personEntity.set(personDetails);
        component.personSelected = signal(signal(selectedPerson));
        component.committeeSelected = signal(signal(selectedCommittee));
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(reloadNextSpy).not.toHaveBeenCalled();
        expect(routerMock.navigate).not.toHaveBeenCalled();
        expect(committeesServiceMock.createMember).toHaveBeenCalled();
        expect(notificationServiceMock.error).toHaveBeenCalledWith('memberships.save.error');
    });

    it('should save membership for committee and handle success', async () => {
        (committeesServiceMock.createMember as jest.Mock).mockReturnValue(of({id: '1'}));
        component.isForCommittee = true;

        const committeeDetails = {id: '1'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);

        component.personSelected = signal(signal(selectedPerson));
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '1'], {queryParams: {tab: 'members'}, replaceUrl: true});
        expect(notificationServiceMock.success).toHaveBeenCalledWith('memberships.save.success');
    });

    it('should save membership for person and handle success', async () => {
        (committeesServiceMock.createMember as jest.Mock).mockReturnValue(of({id: '1'}));
        component.isForPerson = true;

        component.committeeSelected = signal(signal(selectedCommittee));
        component.personEntity.set(personDetails);
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });

        component.save();
        await fixture.whenStable();
        expect(routerMock.navigate).toHaveBeenCalledWith(['persons', '1'], {queryParams: {tab: 'memberships'}, replaceUrl: true});
        expect(notificationServiceMock.success).toHaveBeenCalledWith('memberships.save.success');
    });

    it('should close and return to committee', () => {
        component.isForCommittee = true;
        const committeeDetails = {id: '1'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);
        component.close();
        expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '1'], {queryParams: {tab: 'members'}, replaceUrl: true});
    });

    it('should close and return to person', () => {
        component.isForPerson = true;
        component.personEntity.set(personDetails);
        component.close();
        expect(routerMock.navigate).toHaveBeenCalledWith(['persons', '1'], {queryParams: {tab: 'memberships'}, replaceUrl: true});
    });

    it('should get header text', () => {
        component.isForCommittee = true;
        const committeeDetails = {id: '1', description: 'desc'} as CommitteeDetails;
        component.committeeEntity.set(committeeDetails);
        const text = component.getHeaderText();
        expect(text).toBe('desc');
    });

    it.each([
        [true, false, true],
        [false, true, true],
    ])('selectionMissing returns correct value when not selected', (isForPerson, isForCommittee, expectedResult) => {
        component.isForPerson = isForPerson;
        component.isForCommittee = isForCommittee;
        component.personSelected = signal(signal(undefined));
        component.committeeSelected = signal(signal(undefined));
        const result = component.selectionMissing();

        expect(result).toBe(expectedResult);
    });

    it.each([
        [true, false, false],
        [false, true, false],
    ])('selectionMissing returns correct value when selected', (isForPerson, isForCommittee, expectedResult) => {
        component.isForPerson = isForPerson;
        component.isForCommittee = isForCommittee;
        component.personSelected = signal(signal(selectedPerson));
        component.committeeSelected = signal(signal(selectedCommittee));
        const result = component.selectionMissing();

        expect(result).toBe(expectedResult);
    });
});
