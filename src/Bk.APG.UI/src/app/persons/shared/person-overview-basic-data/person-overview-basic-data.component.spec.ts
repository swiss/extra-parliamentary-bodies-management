import {ComponentFixture, TestBed} from '@angular/core/testing';
import {PersonOverviewBasicDataComponent} from './person-overview-basic-data.component';

describe('PersonOverviewBasicDataComponent', () => {
    let component: PersonOverviewBasicDataComponent;
    let fixture: ComponentFixture<PersonOverviewBasicDataComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [PersonOverviewBasicDataComponent],
        }).compileComponents();

        fixture = TestBed.createComponent(PersonOverviewBasicDataComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
