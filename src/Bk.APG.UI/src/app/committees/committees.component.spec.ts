import {provideHttpClient} from '@angular/common/http';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {Sort} from '@angular/material/sort';
import {ActivatedRoute, Router} from '@angular/router';
import {TranslateService} from '@ngx-translate/core';
import {MockService} from 'ng-mocks';
import {Subject} from 'rxjs';
import {CommitteesComponent} from './committees.component';
import {CommitteesService} from './committees.service';

describe('CommitteesComponent', () => {
    let component: CommitteesComponent;
    let fixture: ComponentFixture<CommitteesComponent>;
    let routeMock: Partial<ActivatedRoute>;
    let routerMock: Partial<Router>;

    beforeEach(async () => {
        const translateServiceMock = {
            currentLang: 'en',
            onLangChange: new Subject(),
        };

        routeMock = {};
        routerMock = {
            navigate: jest.fn().mockResolvedValue(true),
        };

        await TestBed.configureTestingModule({
            imports: [CommitteesComponent],
            providers: [
                {provide: TranslateService, useValue: translateServiceMock},
                {provide: CommitteesService, useValue: MockService(CommitteesService)},
                {provide: ActivatedRoute, useValue: routeMock},
                {provide: Router, useValue: routerMock},
                provideHttpClient(),
            ],
            schemas: [NO_ERRORS_SCHEMA],
        })
            .overrideTemplate(CommitteesComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(CommitteesComponent);
        component = fixture.componentInstance;
        component.reload$.next();
        fixture.detectChanges();
    });

    describe('onPageChange', () => {
        it('should update pagingParams and trigger reload$', () => {
            const reloadSpy = jest.spyOn(component.reload$, 'next');
            const pageEvent = {pageSize: 100, pageIndex: 2, length: 200};

            component.onPageChange(pageEvent);

            expect(component.pagingParams.pageSize).toBe(pageEvent.pageSize);
            expect(component.pagingParams.pageIndex).toBe(pageEvent.pageIndex);
            expect(reloadSpy).toHaveBeenCalled();
        });
    });

    describe('onSort', () => {
        it('should update currentSort and trigger reload$', () => {
            const reloadSpy = jest.spyOn(component.reload$, 'next');
            const sortEvent = {active: 'committeeId', direction: 'desc'};

            component.onSort(sortEvent as Sort);

            expect(component.currentSort.active).toBe(sortEvent.active);
            expect(component.currentSort.direction).toBe(sortEvent.direction);
            expect(reloadSpy).toHaveBeenCalled();
        });
    });

    describe('onFilter', () => {
        it('should update filterValue and trigger reload$', () => {
            const reloadSpy = jest.spyOn(component.reload$, 'next');
            const filterParams = {
                freeText: 'test',
                hasActiveMembership: [true],
                cantons: ['canton1'],
                languages: ['lang1'],
            };

            component.onFilter(filterParams);

            expect(component.filterValue).toEqual(filterParams);
            expect(reloadSpy).toHaveBeenCalled();
        });
    });

    describe('openCommittee', () => {
        it('should navigate using router', async () => {
            const committeeId = 'committee123';

            component.openCommittee(committeeId);

            expect(routerMock.navigate).toHaveBeenCalledWith([committeeId], {
                relativeTo: routeMock,
            });
        });
    });
});
