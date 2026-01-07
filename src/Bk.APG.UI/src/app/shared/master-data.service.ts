/* eslint-disable @typescript-eslint/no-explicit-any */
import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable, signal} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {AppointmentDecisionLinkType} from '@api/AppointmentDecisionLinkType';
import {AppointmentDecisionType} from '@api/AppointmentDecisionType';
import {Canton} from '@api/Canton';
import {CommitteeType} from '@api/CommitteeType';
import {Council} from '@api/Council';
import {Department} from '@api/Department';
import {ElectionOffice} from '@api/ElectionOffice';
import {ElectionType} from '@api/ElectionType';
import {FunctionDto} from '@api/Function';
import {Gender} from '@api/Gender';
import {InterestCommittee} from '@api/InterestCommittee';
import {InterestFunction} from '@api/InterestFunction';
import {InterestLegalForm} from '@api/InterestLegalForm';
import {Language} from '@api/Language';
import {LegalForm} from '@api/LegalForm';
import {LegislaturePeriod} from '@api/LegislaturePeriod';
import {Level} from '@api/Level';
import {MembershipAddition} from '@api/MembershipAddition';
import {Occupation} from '@api/Occupation';
import {Office} from '@api/Office';
import {Salutation} from '@api/Salutation';
import {Term} from '@api/Term';
import {TermDate} from '@api/TermDate';
import {WorklistTaskState} from '@api/WorklistTaskState';
import {WorklistTaskType} from '@api/WorklistTaskType';
import {TranslateService} from '@ngx-translate/core';
import {distinctUntilChanged, Observable, startWith, switchMap} from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class MasterDataService {
    languages = signal<Language[]>([]);
    cantons = signal<Canton[]>([]);
    genders = signal<Gender[]>([]);
    salutations = signal<Salutation[]>([]);
    interestCommittees = signal<InterestCommittee[]>([]);
    interestFunctions = signal<InterestFunction[]>([]);
    interestLegalForms = signal<InterestLegalForm[]>([]);
    legalForms = signal<LegalForm[]>([]);
    departments = signal<Department[]>([]);
    permittedDepartments = signal<Department[]>([]);
    offices = signal<Office[]>([]);
    permittedOffices = signal<Office[]>([]);
    committeeTypes = signal<CommitteeType[]>([]);
    terms = signal<Term[]>([]);
    termDates = signal<TermDate[]>([]);
    levels = signal<Level[]>([]);
    electionTypes = signal<ElectionType[]>([]);
    electionOffices = signal<ElectionOffice[]>([]);
    functions = signal<FunctionDto[]>([]);
    membershipAdditions = signal<MembershipAddition[]>([]);
    appointmentDecisionTypes = signal<AppointmentDecisionType[]>([]);
    appointmentDecisionLinkTypes = signal<AppointmentDecisionLinkType[]>([]);
    legislaturePeriods = signal<LegislaturePeriod[]>([]);
    councils = signal<Council[]>([]);
    worklistTaskTypes = signal<WorklistTaskType[]>([]);
    worklistTaskStates = signal<WorklistTaskState[]>([]);

    constructor(
        private readonly http: HttpClient,
        private readonly translateService: TranslateService
    ) {
        this.translateService.onLangChange
            .pipe(
                startWith({lang: this.translateService.currentLang}),
                distinctUntilChanged((prev, curr) => prev.lang === curr.lang),
                switchMap(() => this.getMasterData()),
                takeUntilDestroyed()
            )
            .subscribe(masterData => {
                this.languages.set(masterData.languages);
                this.cantons.set(masterData.cantons);
                this.genders.set(masterData.genders);
                this.salutations.set(masterData.salutations);
                this.interestCommittees.set(masterData.interestCommittees);
                this.interestFunctions.set(masterData.interestFunctions);
                this.interestLegalForms.set(masterData.interestLegalForms);
                this.legalForms.set(masterData.legalForms);
                this.levels.set(masterData.levels);
                this.departments.set(masterData.departments);
                this.permittedDepartments.set(masterData.permittedDepartments);
                this.offices.set(masterData.offices);
                this.permittedOffices.set(masterData.permittedOffices);
                this.committeeTypes.set(masterData.committeeTypes);
                this.terms.set(masterData.terms);
                this.termDates.set(masterData.termDates);
                this.electionTypes.set(masterData.electionTypes);
                this.electionOffices.set(masterData.electionOffices);
                this.functions.set(masterData.functions);
                this.membershipAdditions.set(masterData.membershipAdditions);
                this.appointmentDecisionTypes.set(masterData.appointmentDecisionTypes);
                this.appointmentDecisionLinkTypes.set(masterData.appointmentDecisionLinkTypes);
                this.legislaturePeriods.set(masterData.legislaturePeriods);
                this.councils.set(masterData.councils);
                this.worklistTaskTypes.set(masterData.worklistTaskTypes);
                this.worklistTaskStates.set(masterData.worklistTaskStates);
            });
    }

    public getOfficeByName(officeName: string): Observable<Office[]> {
        const params = new HttpParams().set('officeName', officeName);

        return this.http.get<Office[]>('/api/masterData/offices/search', {params});
    }

    public getOccupationsByName(occupation: string): Observable<Occupation[]> {
        const params = new HttpParams().set('occupation', occupation);

        return this.http.get<Occupation[]>('/api/masterData/occupations/search', {params});
    }

    private readonly getMasterData = () => this.http.get<any>('/api/masterData');
}
