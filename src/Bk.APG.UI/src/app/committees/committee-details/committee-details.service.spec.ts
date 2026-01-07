import {TestBed} from '@angular/core/testing';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {CommitteeDetailsService} from './committee-details.service';

describe('CommitteeDetailsService', () => {
    let service: CommitteeDetailsService;

    const isObserverSubject = new BehaviorSubject(false);
    const authServiceMock = {
        isObserver$: isObserverSubject.asObservable(),
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [CommitteeDetailsService, {provide: AuthService, useValue: authServiceMock}],
        });
        service = TestBed.inject(CommitteeDetailsService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should have initial computed values as false', () => {
        expect(service.isDataTabDisabled()).toBe(false);
        expect(service.isContactsTabDisabled()).toBe(false);
        expect(service.isDecisionsTabDisabled()).toBe(false);
        expect(service.isJustificationsFormDirty()).toBe(false);
        expect(service.isMembersTabDisabled()).toBe(false);
    });

    it('should update isDataTabDisabled based on on forms dirty flags', () => {
        expect(service.isDataTabDisabled()).toBe(false);

        service.isContactsFormDirty.set(true);
        expect(service.isDataTabDisabled()).toBe(true);
        service.isContactsFormDirty.set(false);
        expect(service.isDataTabDisabled()).toBe(false);

        service.isDecisionsFormDirty.set(true);
        expect(service.isDataTabDisabled()).toBe(true);
        service.isDecisionsFormDirty.set(false);
        expect(service.isDataTabDisabled()).toBe(false);

        service.isJustificationsFormDirty.set(true);
        expect(service.isDataTabDisabled()).toBe(true);
        service.isJustificationsFormDirty.set(false);
        expect(service.isDataTabDisabled()).toBe(false);

        service.isMembersFormDirty.set(true);
        expect(service.isDataTabDisabled()).toBe(true);
        service.isMembersFormDirty.set(false);
        expect(service.isDataTabDisabled()).toBe(false);
    });

    it('should update isContactsTabDisabled based on on forms dirty flags', () => {
        expect(service.isContactsTabDisabled()).toBe(false);

        service.isDataFormDirty.set(true);
        expect(service.isContactsTabDisabled()).toBe(true);
        service.isDataFormDirty.set(false);
        expect(service.isContactsTabDisabled()).toBe(false);

        service.isDecisionsFormDirty.set(true);
        expect(service.isContactsTabDisabled()).toBe(true);
        service.isDecisionsFormDirty.set(false);
        expect(service.isContactsTabDisabled()).toBe(false);

        service.isJustificationsFormDirty.set(true);
        expect(service.isContactsTabDisabled()).toBe(true);
        service.isJustificationsFormDirty.set(false);
        expect(service.isContactsTabDisabled()).toBe(false);

        service.isMembersFormDirty.set(true);
        expect(service.isContactsTabDisabled()).toBe(true);
        service.isMembersFormDirty.set(false);
        expect(service.isContactsTabDisabled()).toBe(false);
    });

    it('should update isDecisionsTabDisabled based on on forms dirty flags', () => {
        expect(service.isDecisionsTabDisabled()).toBe(false);

        service.isDataFormDirty.set(true);
        expect(service.isDecisionsTabDisabled()).toBe(true);
        service.isDataFormDirty.set(false);
        expect(service.isDecisionsTabDisabled()).toBe(false);

        service.isContactsFormDirty.set(true);
        expect(service.isDecisionsTabDisabled()).toBe(true);
        service.isContactsFormDirty.set(false);
        expect(service.isDecisionsTabDisabled()).toBe(false);

        service.isJustificationsFormDirty.set(true);
        expect(service.isDecisionsTabDisabled()).toBe(true);
        service.isJustificationsFormDirty.set(false);
        expect(service.isDecisionsTabDisabled()).toBe(false);

        service.isMembersFormDirty.set(true);
        expect(service.isDecisionsTabDisabled()).toBe(true);
        service.isMembersFormDirty.set(false);
        expect(service.isDecisionsTabDisabled()).toBe(false);
    });

    it('should update isJustificationsTabDisabled based on on forms dirty flags', () => {
        expect(service.isJustificationsTabDisabled()).toBe(false);

        service.isDataFormDirty.set(true);
        expect(service.isJustificationsTabDisabled()).toBe(true);
        service.isDataFormDirty.set(false);
        expect(service.isJustificationsTabDisabled()).toBe(false);

        service.isContactsFormDirty.set(true);
        expect(service.isJustificationsTabDisabled()).toBe(true);
        service.isContactsFormDirty.set(false);
        expect(service.isJustificationsTabDisabled()).toBe(false);

        service.isDecisionsFormDirty.set(true);
        expect(service.isJustificationsTabDisabled()).toBe(true);
        service.isDecisionsFormDirty.set(false);
        expect(service.isJustificationsTabDisabled()).toBe(false);

        service.isMembersFormDirty.set(true);
        expect(service.isJustificationsTabDisabled()).toBe(true);
        service.isMembersFormDirty.set(false);
        expect(service.isJustificationsTabDisabled()).toBe(false);
    });

    it('should update isMembersTabDisabled based on on forms dirty flags', () => {
        expect(service.isMembersTabDisabled()).toBe(false);

        service.isDataFormDirty.set(true);
        expect(service.isMembersTabDisabled()).toBe(true);
        service.isDataFormDirty.set(false);
        expect(service.isMembersTabDisabled()).toBe(false);

        service.isContactsFormDirty.set(true);
        expect(service.isMembersTabDisabled()).toBe(true);
        service.isContactsFormDirty.set(false);
        expect(service.isMembersTabDisabled()).toBe(false);

        service.isDecisionsFormDirty.set(true);
        expect(service.isMembersTabDisabled()).toBe(true);
        service.isDecisionsFormDirty.set(false);
        expect(service.isMembersTabDisabled()).toBe(false);

        service.isJustificationsFormDirty.set(true);
        expect(service.isMembersTabDisabled()).toBe(true);
        service.isJustificationsFormDirty.set(false);
        expect(service.isMembersTabDisabled()).toBe(false);
    });
});
