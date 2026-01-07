import {NO_ERRORS_SCHEMA, signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ActivatedRoute, ActivatedRouteSnapshot, convertToParamMap, ParamMap} from '@angular/router';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {LangChangeEvent, TranslateModule, TranslateService} from '@ngx-translate/core';
import {ObAlertModule} from '@oblique/oblique';
import {MockComponents, MockModule} from 'ng-mocks';
import {BehaviorSubject, of, Subject} from 'rxjs';
import {CommitteesService} from '../../committees.service';
import {CommitteeJustificationsOverviewComponent} from '../../shared/committee-justifications-overview/committee-justifications-overview.component';
import {CommitteeDetailsService} from '../committee-details.service';
import {CommitteeOverviewBasicDataComponent} from './committee-overview-basic-data/committee-overview-basic-data.component';
import {CommitteeOverviewComponent} from './committee-overview.component';
import {ContactPointsComponent} from './contact-points/contact-points.component';

describe('CommitteeOverviewComponent', () => {
    let component: CommitteeOverviewComponent;
    let fixture: ComponentFixture<CommitteeOverviewComponent>;
    let committeesServiceMock: Partial<CommitteesService>;
    let reloadSubject: BehaviorSubject<void>;
    let paramMapSubject: Subject<ParamMap>;
    let routeMock: Partial<ActivatedRoute>;
    let onLangChangeSubject: Subject<LangChangeEvent>;
    let committeeDetailsServiceMock: Partial<CommitteeDetailsService>;

    beforeEach(async () => {
        reloadSubject = new BehaviorSubject<void>(undefined);

        paramMapSubject = new BehaviorSubject<ParamMap>(convertToParamMap({id: '123'}));
        routeMock = {
            paramMap: paramMapSubject.asObservable(),
            snapshot: {
                paramMap: convertToParamMap({id: '123'}),
            } as unknown as ActivatedRouteSnapshot,
        };

        onLangChangeSubject = new Subject<LangChangeEvent>();
        const translateServiceMock = {
            currentLang: 'de',
            onLangChange: onLangChangeSubject.asObservable(),
            get: jest.fn(),
        };

        committeesServiceMock = {
            getCommitteeDetails: jest.fn().mockReturnValue(of({id: '123', description: 'foo'})),
            reload$: reloadSubject,
        };

        committeeDetailsServiceMock = {
            committeeDetails: signal<CommitteeDetails>({} as CommitteeDetails),
        };

        await TestBed.configureTestingModule({
            imports: [
                MockModule(TranslateModule),
                MockModule(ObAlertModule),
                MockComponents(ContactPointsComponent, CommitteeJustificationsOverviewComponent, CommitteeOverviewBasicDataComponent),
                CommitteeOverviewComponent,
            ],
            providers: [
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: CommitteeDetailsService, useValue: committeeDetailsServiceMock},
            ],
            schemas: [NO_ERRORS_SCHEMA],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeOverviewComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
        await fixture.whenStable();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create the component', () => {
        expect(component).toBeTruthy();
    });

    it('should initialize committeeId from route params', () => {
        expect(component.committeeId).toBe('123');
    });

    it('should call getCommitteeDetails on init', () => {
        expect(committeesServiceMock.getCommitteeDetails).toHaveBeenCalledWith('123');
    });

    it('should update committeeDetails when reload is triggered', () => {
        (committeesServiceMock.getCommitteeDetails as jest.Mock).mockReturnValueOnce(of({id: '123', description: 'foo'} as unknown as CommitteeDetails));

        reloadSubject.next();
        fixture.detectChanges();

        expect(committeeDetailsServiceMock.committeeDetails!()).toEqual({id: '123', description: 'foo'});
    });

    it('should update committeeId when route param changes', () => {
        routeMock.snapshot!.paramMap.get = jest.fn().mockReturnValue('456');
        paramMapSubject.next({
            get: jest.fn().mockReturnValue('456'),
        } as unknown as ParamMap);

        expect(component.committeeId).toBe('456');
        expect(committeesServiceMock.getCommitteeDetails).toHaveBeenCalledWith('456');
    });

    it('should react to language changes', () => {
        onLangChangeSubject.next({lang: 'fr'} as unknown as LangChangeEvent);

        expect(committeesServiceMock.getCommitteeDetails).toHaveBeenCalledTimes(2);
        expect(committeesServiceMock.getCommitteeDetails).toHaveBeenCalledWith('123');
    });
});
