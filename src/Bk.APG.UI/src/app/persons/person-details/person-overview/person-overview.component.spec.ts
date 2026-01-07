import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {ActivatedRoute, ActivatedRouteSnapshot, convertToParamMap, ParamMap} from '@angular/router';
import {PersonDetails} from '@api/PersonDetails';
import {LangChangeEvent, TranslateModule, TranslateService} from '@ngx-translate/core';
import {ObAlertModule} from '@oblique/oblique';
import {MockComponents, MockModule} from 'ng-mocks';
import {BehaviorSubject, of, Subject} from 'rxjs';
import {PersonsService} from '../../persons.service';
import {PersonOverviewBasicDataComponent} from '../../shared/person-overview-basic-data/person-overview-basic-data.component';
import {PersonDetailsService} from '../person-details.service';
import {PersonOverviewAddressComponent} from './person-overview-address/person-overview-address.component';
import {PersonOverviewInterestsComponent} from './person-overview-interests/person-overview-interests.component';
import {PersonOverviewMembershipsComponent} from './person-overview-memberships/person-overview-memberships.component';
import {PersonOverviewComponent} from './person-overview.component';

describe('PersonDetailsOverviewComponent', () => {
    let component: PersonOverviewComponent;
    let fixture: ComponentFixture<PersonOverviewComponent>;
    let personsServiceMock: Partial<PersonsService>;
    let reloadSubject: Subject<void>;
    let paramMapSubject: BehaviorSubject<ParamMap>;
    let routeMock: Partial<ActivatedRoute>;
    let onLangChangeSubject: Subject<LangChangeEvent>;
    const personDetailsServiceMock = {
        personDetails: signal({}),
    } as Partial<PersonDetailsService>;

    beforeEach(async () => {
        reloadSubject = new Subject<void>();

        paramMapSubject = new BehaviorSubject<ParamMap>(convertToParamMap({id: '123'}));
        routeMock = {
            paramMap: paramMapSubject.asObservable(),
            snapshot: {
                paramMap: convertToParamMap({id: '123'}),
            } as unknown as ActivatedRouteSnapshot,
        };

        onLangChangeSubject = new Subject<LangChangeEvent>();
        const translateServiceMock = {
            get: jest.fn(),
            currentLang: 'de',
            onLangChange: onLangChangeSubject.asObservable(),
        };

        personsServiceMock = {
            getPersonDetails: jest.fn().mockReturnValue(of({id: '123', name: 'John Doe'})),
            reload$: reloadSubject,
        };

        await TestBed.configureTestingModule({
            imports: [
                MockModule(TranslateModule),
                MockModule(ObAlertModule),
                MockComponents(
                    PersonOverviewAddressComponent,
                    PersonOverviewMembershipsComponent,
                    PersonOverviewInterestsComponent,
                    PersonOverviewBasicDataComponent
                ),
                PersonOverviewComponent,
            ],
            providers: [
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: PersonsService, useValue: personsServiceMock},
                {provide: PersonDetailsService, useValue: personDetailsServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonOverviewComponent);
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

    it('should initialize personId from route params', () => {
        expect(component.personId).toBe('123');
    });

    it('should call getPersonDetails on init', () => {
        expect(personsServiceMock.getPersonDetails).toHaveBeenCalledWith('123');
    });

    it('should update personDetails when reload is triggered', () => {
        (personsServiceMock.getPersonDetails as jest.Mock).mockReturnValueOnce(of({id: '123', surname: 'Jane Doe'} as PersonDetails));

        reloadSubject.next();
        fixture.detectChanges();

        expect(personDetailsServiceMock.personDetails!()).toEqual({id: '123', surname: 'Jane Doe'});
    });

    it('should update personId when route param changes', () => {
        routeMock.snapshot!.paramMap.get = jest.fn().mockReturnValue('456');
        paramMapSubject.next({
            get: jest.fn().mockReturnValue('456'),
        } as unknown as ParamMap);

        expect(component.personId).toBe('456');
        expect(personsServiceMock.getPersonDetails).toHaveBeenCalledWith('456');
    });

    it('should react to language changes', () => {
        onLangChangeSubject.next({lang: 'fr'} as unknown as LangChangeEvent);

        expect(personsServiceMock.getPersonDetails).toHaveBeenCalledTimes(2);
        expect(personsServiceMock.getPersonDetails).toHaveBeenCalledWith('123');
    });
});
