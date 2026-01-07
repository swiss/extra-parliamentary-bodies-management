import {TestBed} from '@angular/core/testing';
import {BehaviorSubject} from 'rxjs';
import {AuthService} from '../../auth/auth.service';
import {PersonDetailsService} from './person-details.service';

describe('PersonDetailsService', () => {
    let service: PersonDetailsService;

    const isObserverSubject = new BehaviorSubject(false);
    const authServiceMock = {
        isObserver$: isObserverSubject.asObservable(),
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [PersonDetailsService, {provide: AuthService, useValue: authServiceMock}],
        });
        service = TestBed.inject(PersonDetailsService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should have initial computed values as false', () => {
        isObserverSubject.next(false);
        expect(service.isDataTabDisabled()).toBe(false);
        expect(service.isInterestsTabDisabled()).toBe(false);
        expect(service.isMembershipsTabDisabled()).toBe(false);
    });

    it('should update isDataTabDisabled based on isInterestsFormDirty', () => {
        isObserverSubject.next(false);
        expect(service.isDataTabDisabled()).toBe(false);
        service.isInterestsFormDirty.set(true);
        expect(service.isDataTabDisabled()).toBe(true);
        service.isInterestsFormDirty.set(false);
        expect(service.isDataTabDisabled()).toBe(false);
    });

    it('should update isInterestsTabDisabled based on isDataFormDirty', () => {
        isObserverSubject.next(false);
        expect(service.isInterestsTabDisabled()).toBe(false);
        service.isDataFormDirty.set(true);
        expect(service.isInterestsTabDisabled()).toBe(true);
        service.isDataFormDirty.set(false);
        expect(service.isInterestsTabDisabled()).toBe(false);
    });

    it('should update isMembershipsTabDisabled based on forms dirty flags', () => {
        isObserverSubject.next(false);
        expect(service.isMembershipsTabDisabled()).toBe(false);
        service.isDataFormDirty.set(true);
        expect(service.isMembershipsTabDisabled()).toBe(true);
        service.isDataFormDirty.set(false);
        service.isInterestsFormDirty.set(true);
        expect(service.isMembershipsTabDisabled()).toBe(true);
        service.isDataFormDirty.set(true);
        expect(service.isMembershipsTabDisabled()).toBe(true);
        service.isDataFormDirty.set(false);
        service.isInterestsFormDirty.set(false);
        expect(service.isMembershipsTabDisabled()).toBe(false);
    });
});
