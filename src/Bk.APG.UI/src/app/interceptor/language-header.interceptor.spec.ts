import {HttpRequest} from '@angular/common/http';
import {TranslateService} from '@ngx-translate/core';
import {LanguageHeaderInterceptor} from './language-header.interceptor';

describe('LanguageHeaderInterceptor', () => {
    let interceptor: LanguageHeaderInterceptor;

    const translateServiceMock = {
        getCurrentLang: () => 'fr',
    };

    const headerMock = {
        append: jest.fn(),
        test: 'test',
    };

    const requestMock = {
        clone: jest.fn(),
        headers: headerMock,
    };

    const nextMock = {
        handle: jest.fn(),
    };

    beforeEach(() => {
        headerMock.append.mockImplementation(() => headerMock);
        requestMock.clone.mockImplementation(() => requestMock);

        interceptor = new LanguageHeaderInterceptor(translateServiceMock as unknown as TranslateService);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(interceptor).toBeTruthy();
    });

    it('should intercept and add the current language', () => {
        interceptor.intercept(requestMock as unknown as HttpRequest<unknown>, nextMock);

        expect(requestMock.clone).toHaveBeenCalledTimes(1);
        expect(requestMock.clone).toHaveBeenCalledWith({headers: headerMock});
        expect(nextMock.handle).toHaveBeenCalledTimes(1);
        expect(nextMock.handle).toHaveBeenCalledWith(requestMock);
        expect(headerMock.append).toHaveBeenCalledTimes(1);
        expect(headerMock.append).toHaveBeenCalledWith('Accept-Language', 'fr');
    });

    it('should intercept and add de when no current language is set', () => {
        translateServiceMock.getCurrentLang = () => '';

        interceptor.intercept(requestMock as unknown as HttpRequest<unknown>, nextMock);

        expect(requestMock.clone).toHaveBeenCalledTimes(1);
        expect(requestMock.clone).toHaveBeenCalledWith({headers: headerMock});
        expect(nextMock.handle).toHaveBeenCalledTimes(1);
        expect(nextMock.handle).toHaveBeenCalledWith(requestMock);
        expect(headerMock.append).toHaveBeenCalledTimes(1);
        expect(headerMock.append).toHaveBeenCalledWith('Accept-Language', 'de');
    });
});
