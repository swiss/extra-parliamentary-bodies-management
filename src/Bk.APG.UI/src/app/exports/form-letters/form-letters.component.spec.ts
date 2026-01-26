import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TranslatePipe} from '@ngx-translate/core';
import {MockPipe} from 'ng-mocks';
import {FormLettersComponent} from './form-letters.component';

describe('FormLettersComponent', () => {
    let component: FormLettersComponent;
    let fixture: ComponentFixture<FormLettersComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [FormLettersComponent, MockPipe(TranslatePipe)],
        })
            .overrideTemplate(FormLettersComponent, '')
            .compileComponents();

        fixture = TestBed.createComponent(FormLettersComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
