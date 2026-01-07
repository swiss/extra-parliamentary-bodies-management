import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TranslatePipe} from '@ngx-translate/core';
import {ObIconModule} from '@oblique/oblique';
import {MockModule, MockPipe} from 'ng-mocks';
import {UnsavedChangesIconComponent} from './unsaved-changes-icon.component';

describe('UnsavedChangesIconComponent', () => {
    let component: UnsavedChangesIconComponent;
    let fixture: ComponentFixture<UnsavedChangesIconComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [MockPipe(TranslatePipe), UnsavedChangesIconComponent, MockModule(ObIconModule)],
        }).compileComponents();

        fixture = TestBed.createComponent(UnsavedChangesIconComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
