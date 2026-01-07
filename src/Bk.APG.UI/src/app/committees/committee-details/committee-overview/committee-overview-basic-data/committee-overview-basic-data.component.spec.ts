import {ComponentFixture, TestBed} from '@angular/core/testing';
import {CommitteeOverviewBasicDataComponent} from './committee-overview-basic-data.component';

describe('CommitteeOverviewBasicDataComponent', () => {
    let component: CommitteeOverviewBasicDataComponent;
    let fixture: ComponentFixture<CommitteeOverviewBasicDataComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [CommitteeOverviewBasicDataComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeOverviewBasicDataComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
