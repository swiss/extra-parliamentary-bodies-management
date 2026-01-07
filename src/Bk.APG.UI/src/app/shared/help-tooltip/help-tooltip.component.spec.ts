import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatIconTestingModule} from '@angular/material/icon/testing';
import {TranslatePipe} from '@ngx-translate/core';
import {WINDOW} from '@oblique/oblique';
import {MockPipe} from 'ng-mocks';
import {HelpTooltipComponent} from './help-tooltip.component';

describe('HelpTooltipComponent', () => {
    let component: HelpTooltipComponent;
    let fixture: ComponentFixture<HelpTooltipComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [HelpTooltipComponent, MockPipe(TranslatePipe), MatIconTestingModule],
            providers: [{provide: WINDOW, useValue: window}],
        }).compileComponents();

        fixture = TestBed.createComponent(HelpTooltipComponent);
        component = fixture.componentInstance;
        fixture.componentRef.setInput('text', 'test.translation.key');
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should use normal size by default', () => {
        const icon = fixture.nativeElement.querySelector('.help-icon') as HTMLElement;
        expect(icon.classList.contains('help-icon-big')).toBeFalsy();
    });

    it('should render big icon when size is set', () => {
        fixture.componentRef.setInput('size', 'big');
        fixture.detectChanges();

        const icon = fixture.nativeElement.querySelector('.help-icon') as HTMLElement;
        expect(icon.classList.contains('help-icon-big')).toBeTruthy();
    });
});
