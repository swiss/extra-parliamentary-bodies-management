import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatTableModule} from '@angular/material/table';
import {TranslateModule} from '@ngx-translate/core';
import {MockModule} from 'ng-mocks';
import {PersonOverviewInterestsComponent} from './person-overview-interests.component';

describe('PersonsInterestComponent', () => {
    let component: PersonOverviewInterestsComponent;
    let fixture: ComponentFixture<PersonOverviewInterestsComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [PersonOverviewInterestsComponent, MockModule(TranslateModule), MockModule(MatTableModule), PersonOverviewInterestsComponent],
            providers: [],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonOverviewInterestsComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
