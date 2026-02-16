import {TestBed} from '@angular/core/testing';
import {LOCAL_STORAGE} from '../injection.tokens';
import {SearchStorageService} from './search-storage.service';

describe('SearchStorageService', () => {
    let service: SearchStorageService;

    const localStorageMock = {
        getItem: jest.fn(),
        setItem: jest.fn(),
        removeItem: jest.fn(),
    };

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [{provide: LOCAL_STORAGE, useValue: localStorageMock}],
        });
        service = TestBed.inject(SearchStorageService);
    });

    beforeEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should return params', () => {
        localStorageMock.getItem.mockReturnValueOnce('{"success": true}');

        const result = service.getParams('test');

        expect(result).toBeTruthy();
        expect(result.success).toBe(true);
    });

    it('should set params', () => {
        const testObj = {saved: true};
        service.setParams('test', testObj);

        expect(localStorageMock.setItem).toHaveBeenCalledTimes(1);
        expect(localStorageMock.setItem).toHaveBeenCalledWith('search-params-test', JSON.stringify(testObj));
    });

    it('should patch params', () => {
        localStorageMock.getItem.mockReturnValueOnce('{"propertyKeepValue":"keep", "propertyOverride": "willbeoverridden"}');

        const patch = {
            propertyOverride: 'newvalue',
            propertyPatch: 'valueonlyfrompatch',
        };
        const expectedObjAfterPatch = {
            propertyKeepValue: 'keep',
            propertyOverride: 'newvalue',
            propertyPatch: 'valueonlyfrompatch',
        };

        service.patchParams('test', patch);

        expect(localStorageMock.setItem).toHaveBeenCalledTimes(1);
        expect(localStorageMock.setItem).toHaveBeenCalledWith('search-params-test', JSON.stringify(expectedObjAfterPatch));
    });

    it('should remove params', () => {
        service.removeParams('test');
        expect(localStorageMock.removeItem).toHaveBeenCalledTimes(1);
        expect(localStorageMock.removeItem).toHaveBeenCalledWith('search-params-test');
    });
});
