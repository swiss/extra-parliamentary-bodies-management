import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {Router} from '@angular/router';
import {CommitteeCreate} from '@api/CommitteeCreate';
import {TranslatePipe} from '@ngx-translate/core';
import {ObHttpApiInterceptorEvents, ObNotificationService} from '@oblique/oblique';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {MockComponents, MockPipe} from 'ng-mocks';
import {BehaviorSubject, of, throwError} from 'rxjs';
import {CommitteesService} from '../committees.service';
import {CommitteeDataFormComponent} from '../shared/committee-data-form/committee-data-form.component';
import {CommitteeCreateComponent} from './committee-create.component';

describe('CommitteeCreateComponent', () => {
    let component: CommitteeCreateComponent;
    let fixture: ComponentFixture<CommitteeCreateComponent>;
    let committeesServiceMock: Partial<CommitteesService>;
    let routerMock: Partial<Router>;
    let notificationServiceMock: Partial<ObNotificationService>;
    let interceptorEventsMock: Partial<ObHttpApiInterceptorEvents>;
    let reloadSubject: BehaviorSubject<void>;

    beforeEach(async () => {
        reloadSubject = new BehaviorSubject<void>(undefined);
        committeesServiceMock = {
            createCommittee: jest.fn(),
            reload$: reloadSubject,
            getCommitteeForCreate: jest.fn().mockReturnValue(of({})),
        };

        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };
        notificationServiceMock = {
            success: jest.fn(),
            error: jest.fn(),
        };
        interceptorEventsMock = {
            deactivateNotificationOnNextAPICalls: jest.fn(),
        };

        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), MockComponents(CommitteeDataFormComponent, HelpTooltipComponent), CommitteeCreateComponent],
            providers: [
                {provide: CommitteesService, useValue: committeesServiceMock},
                {provide: Router, useValue: routerMock},
                {provide: ObNotificationService, useValue: notificationServiceMock},
                {provide: ObHttpApiInterceptorEvents, useValue: interceptorEventsMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeCreateComponent);
        component = fixture.componentInstance;
        // @ts-ignore
        component.form = () => ({
            pristine: true,
        });
        component.committeeToCreate = signal({} as unknown as CommitteeCreate);

        fixture.detectChanges();
        await fixture.whenStable();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('save should mark form as touched if invalid', () => {
        // @ts-ignore
        component.form = signal({
            valid: false,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });
        component.save();
        expect(component.form().markAsTouched).toHaveBeenCalled();
        expect(committeesServiceMock.createCommittee).not.toHaveBeenCalled();
    });

    it('save should call createCommittee and handle success', async () => {
        // @ts-ignore
        component.form = signal({
            valid: true,
            markAsTouched: jest.fn(),
            reset: jest.fn(),
        });
        (committeesServiceMock.createCommittee as jest.Mock).mockReturnValue(of({id: '1'}));

        const reloadNextSpy = jest.spyOn(reloadSubject, 'next');
        component.save();

        expect(committeesServiceMock.createCommittee).toHaveBeenCalled();
        expect(reloadNextSpy).toHaveBeenCalled();
        expect(routerMock.navigate).toHaveBeenCalledWith(['committees', '1'], {queryParams: {tab: 'data'}, replaceUrl: true});
        await fixture.whenStable();
        expect(notificationServiceMock.success).toHaveBeenCalledWith('committee.details.data.success');
    });

    it('save should handle createCommittee error', () => {
        // @ts-ignore
        component.form = () => ({
            valid: true,
            reset: jest.fn(),
        });
        (committeesServiceMock.createCommittee as jest.Mock).mockReturnValue(throwError(() => new Error('Create failed')));

        component.save();

        expect(committeesServiceMock.createCommittee).toHaveBeenCalled();
        expect(notificationServiceMock.error).toHaveBeenCalledWith('committee.details.data.error');
    });
});
