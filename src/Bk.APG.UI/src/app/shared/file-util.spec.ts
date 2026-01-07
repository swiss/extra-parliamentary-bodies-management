import {HttpResponse} from '@angular/common/http';
import {downloadFileFromHttpResponse} from './file-util';

describe('fileUtil', () => {
    const windowMock = {
        URL: {
            createObjectURL: jest.fn(),
        },
    };

    const documentMock = {
        createElement: jest.fn(),
        body: {
            appendChild: jest.fn(),
            removeChild: jest.fn(),
        },
    };

    const elementMock = {
        setAttribute: jest.fn(),
        click: jest.fn(),
        href: '',
    };

    const responseMock = {
        headers: {
            get: jest.fn(),
        },
    };

    beforeEach(() => {
        documentMock.createElement.mockReturnValue(elementMock);
        windowMock.URL.createObjectURL.mockReturnValue('TEST-OBJECT-URL');
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should get file name from response headers', () => {
        responseMock.headers.get.mockReturnValue('attachment; filename="FooBar"; filename*=UTF-8\'\'FooBar');

        downloadFileFromHttpResponse(
            responseMock as unknown as HttpResponse<Blob>,
            'test.pdf',
            windowMock as unknown as Window,
            documentMock as unknown as Document
        );

        expect(documentMock.createElement).toHaveBeenCalledWith('a');
        expect(elementMock.href).toEqual('TEST-OBJECT-URL');
        expect(elementMock.setAttribute).toHaveBeenCalledWith('download', 'FooBar');
        expect(documentMock.body.appendChild).toHaveBeenCalledWith(elementMock);
        expect(elementMock.click).toHaveBeenCalledTimes(1);
        expect(documentMock.body.removeChild).toHaveBeenCalledWith(elementMock);
    });

    it('should use fallback filename if response header is empty', () => {
        responseMock.headers.get.mockReturnValue(null);

        downloadFileFromHttpResponse(
            responseMock as unknown as HttpResponse<Blob>,
            'test.pdf',
            windowMock as unknown as Window,
            documentMock as unknown as Document
        );

        expect(documentMock.createElement).toHaveBeenCalledWith('a');
        expect(elementMock.href).toEqual('TEST-OBJECT-URL');
        expect(elementMock.setAttribute).toHaveBeenCalledWith('download', 'test.pdf');
        expect(documentMock.body.appendChild).toHaveBeenCalledWith(elementMock);
        expect(elementMock.click).toHaveBeenCalledTimes(1);
        expect(documentMock.body.removeChild).toHaveBeenCalledWith(elementMock);
    });
});
