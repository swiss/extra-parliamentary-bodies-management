import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {GeneralElectionCommitteeDetails} from '@api/GeneralElectionCommitteeDetails';
import {ErrorService} from '@shared/error-service.service';
import {MasterDataService} from '@shared/master-data.service';
import {ConfigsService} from '../../../../configs.service';
import {PersonsService} from '../../../../persons/persons.service';
import {MembershipCandidateDataFormComponent} from './membership-candidate-data-form.component';

describe('MembershipCandidateDataFormComponent', () => {
    let component: MembershipCandidateDataFormComponent;
    let fixture: ComponentFixture<MembershipCandidateDataFormComponent>;

    const masterDataServiceMock = {
        electionOffices: signal([]),
        functions: signal([]),
        genders: signal([]),
        languages: signal([]),
        electionTypes: signal([]),
        membershipAdditions: signal([]),
    } as unknown as Partial<MasterDataService>;

    const errorServiceMock = {
        getControlError: jest.fn(),
    };

    const personsServiceMock = {
        getPersonDetails: jest.fn(),
    };

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                gender: {
                    femaleId: 'femaleId',
                },
                electionOffice: {
                    federalGovernmentId: 'federalGovernmentId',
                },
            },
        },
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MembershipCandidateDataFormComponent],
            providers: [
                {provide: MasterDataService, useValue: masterDataServiceMock},
                {provide: ErrorService, useValue: errorServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: ConfigsService, useValue: configsServiceMock},
            ],
        })
            .overrideTemplateUsingTestingModule(MembershipCandidateDataFormComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(MembershipCandidateDataFormComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should disable endDate and function when candidate list is completed', () => {
        component.generalElectionCommittee.set({isCandidateListCompleted: true} as GeneralElectionCommitteeDetails);
        fixture.detectChanges();

        expect(component.membershipCandidateForm.controls.endDate.disabled).toBe(true);
        expect(component.membershipCandidateForm.controls.functionId.disabled).toBe(true);
    });

    it('should enable endDate and function when candidate list is not completed', () => {
        component.generalElectionCommittee.set({isCandidateListCompleted: false} as GeneralElectionCommitteeDetails);
        fixture.detectChanges();

        expect(component.membershipCandidateForm.controls.endDate.enabled).toBe(true);
        expect(component.membershipCandidateForm.controls.functionId.enabled).toBe(true);
    });
});
