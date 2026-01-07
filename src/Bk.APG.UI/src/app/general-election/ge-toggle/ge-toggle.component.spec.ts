/* eslint-disable @typescript-eslint/no-explicit-any */
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {MatButtonToggleChange} from '@angular/material/button-toggle';
import {GeneralElectionService} from '../general-election.service';
import {GeneralElectionToggleComponent} from './ge-toggle.component';

describe('GeneralElectionToggleComponent', () => {
    let component: GeneralElectionToggleComponent;
    let fixture: ComponentFixture<GeneralElectionToggleComponent>;
    let mockGeneralElectionService: jest.Mocked<GeneralElectionService>;

    beforeEach(async () => {
        mockGeneralElectionService = {
            toggleGeneralElection: jest.fn(),
            isGeneralElectionVisible: jest.fn(),
            isGeneralElectionEnabled: jest.fn(),
        } as any;

        await TestBed.configureTestingModule({
            imports: [GeneralElectionToggleComponent],
            providers: [{provide: GeneralElectionService, useValue: mockGeneralElectionService}],
        }).compileComponents();

        fixture = TestBed.createComponent(GeneralElectionToggleComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('onToggle', () => {
        it('should call toggleGeneralElection with true when toggle event value is true', () => {
            const mockEvent = {value: true} as MatButtonToggleChange;

            component.onToggle(mockEvent);

            expect(mockGeneralElectionService.toggleGeneralElection).toHaveBeenCalledWith(true);
            expect(mockGeneralElectionService.toggleGeneralElection).toHaveBeenCalledTimes(1);
        });

        it('should call toggleGeneralElection with false when toggle event value is false', () => {
            const mockEvent = {value: false} as MatButtonToggleChange;

            component.onToggle(mockEvent);

            expect(mockGeneralElectionService.toggleGeneralElection).toHaveBeenCalledWith(false);
            expect(mockGeneralElectionService.toggleGeneralElection).toHaveBeenCalledTimes(1);
        });
    });
});
