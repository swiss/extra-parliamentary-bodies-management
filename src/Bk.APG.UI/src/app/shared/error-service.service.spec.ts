import {TestBed} from '@angular/core/testing';
import {TranslateService} from '@ngx-translate/core';
import {ErrorService} from './error-service.service';

describe('ErrorService', () => {
    let service: ErrorService;

    const translateServiceMock = {
        instant: jest.fn(),
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [{provide: TranslateService, useValue: translateServiceMock}],
        });

        translateServiceMock.instant = jest.fn(value => value);

        service = TestBed.inject(ErrorService);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get control error', () => {
        expect(translateServiceMock.instant).toHaveBeenCalledTimes(0);

        expect(service.getControlError({error: true, 'another-error': false}, 'test.label', 'test.prefix')).toEqual('test.prefix.error');
        expect(translateServiceMock.instant).toHaveBeenCalledTimes(1);
        expect(translateServiceMock.instant).toHaveBeenLastCalledWith('test.prefix.error', {field: 'test.label'});
    });

    it('adds default error params to translate data', () => {
        const error = {min: {min: 1, actual: -3.5}};

        expect(service.getControlError(error, 'test.label', 'test.prefix')).toEqual('test.prefix.min');
        expect(translateServiceMock.instant).toHaveBeenLastCalledWith('test.prefix.min', {field: 'test.label', min: 1, actual: -3.5});
    });

    it('can handle empty and null errors', () => {
        expect(service.getControlError(null, 'test.label', 'test.prefix')).toEqual('');
        expect(translateServiceMock.instant).toHaveBeenCalledTimes(0);

        expect(service.getControlError({}, 'test.label', 'test.prefix')).toEqual('');
        expect(translateServiceMock.instant).toHaveBeenCalledTimes(0);
    });
});
