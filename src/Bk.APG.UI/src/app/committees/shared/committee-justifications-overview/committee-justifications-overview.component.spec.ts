import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MockPipe} from 'ng-mocks';
import {CommitteeJustificationsOverviewComponent} from './committee-justifications-overview.component';

describe('CommitteeJustificationsOverviewComponent', () => {
    let component: CommitteeJustificationsOverviewComponent;
    let fixture: ComponentFixture<CommitteeJustificationsOverviewComponent>;

    const translateServiceMock = {
        instant: jest.fn(),
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), CommitteeJustificationsOverviewComponent],
            providers: [{provide: TranslateService, useValue: translateServiceMock}],
        }).compileComponents();

        fixture = TestBed.createComponent(CommitteeJustificationsOverviewComponent);
        fixture.componentRef.setInput('committeeDetails', []);

        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
