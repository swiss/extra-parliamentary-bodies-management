import {signal} from '@angular/core';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatTableModule} from '@angular/material/table';
import {MembershipDetails} from '@api/MembershipDetails';
import {TranslateModule} from '@ngx-translate/core';
import {MockModule} from 'ng-mocks';
import {PersonOverviewMembershipsComponent} from './person-overview-memberships.component';

describe('PersonsMembershipComponent', () => {
    let component: PersonOverviewMembershipsComponent;
    let fixture: ComponentFixture<PersonOverviewMembershipsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [PersonOverviewMembershipsComponent, MockModule(TranslateModule), MockModule(MatTableModule), PersonOverviewMembershipsComponent],
            providers: [],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonOverviewMembershipsComponent);
        fixture.componentRef.setInput('memberships', signal<MembershipDetails[]>([]));
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
        expect(component.memberships).toBeDefined();
    });
});
