import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TranslatePipe} from '@ngx-translate/core';
import {MockPipe} from 'ng-mocks';
import {ConfigsService} from '../../../../configs.service';
import {ContactPointsComponent} from './contact-points.component';

describe('ContactPointsComponent', () => {
    let component: ContactPointsComponent;
    let fixture: ComponentFixture<ContactPointsComponent>;

    const configsServiceMock = {
        frontendConfig: {
            entityIds: {
                contactPoint: {
                    secretariatId: 'secretariatId',
                    dpoId: 'dpoId',
                },
            },
        },
    };

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), ContactPointsComponent],
            providers: [{provide: ConfigsService, useValue: configsServiceMock}],
        }).compileComponents();

        fixture = TestBed.createComponent(ContactPointsComponent);
        fixture.componentRef.setInput('contactPoints', []);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
