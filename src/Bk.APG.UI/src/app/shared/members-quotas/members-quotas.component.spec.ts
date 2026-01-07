import {DecimalPipe, NgClass} from '@angular/common';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObGlobalEventsService, ObliqueModule, ObPopoverModule} from '@oblique/oblique';
import {MockPipes, MockService} from 'ng-mocks';
import {MembersQuotasComponent} from './members-quotas.component';

describe('MembersQuotasComponent', () => {
    let component: MembersQuotasComponent;
    let fixture: ComponentFixture<MembersQuotasComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MembersQuotasComponent, ObliqueModule, MockPipes(DecimalPipe, TranslatePipe), ObPopoverModule, NgClass],
            providers: [
                {provide: ObGlobalEventsService, useValue: MockService<ObGlobalEventsService>},
                {provide: TranslateService, useValue: MockService<TranslateService>},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(MembersQuotasComponent);
        component = fixture.componentInstance;

        fixture.componentRef.setInput('membersQuotas', {});

        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
