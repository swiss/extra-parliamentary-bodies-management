import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TranslateModule} from '@ngx-translate/core';
import {MembersTooltipContentComponent} from './members-tooltip-content.component';

describe('MembersTooltipContentComponent', () => {
    let component: MembersTooltipContentComponent;
    let fixture: ComponentFixture<MembersTooltipContentComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MembersTooltipContentComponent, TranslateModule.forRoot()],
        }).compileComponents();

        fixture = TestBed.createComponent(MembersTooltipContentComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
