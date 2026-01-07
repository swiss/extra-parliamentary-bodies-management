import {HttpRequest, HttpResponse} from '@angular/common/http';
import {of} from 'rxjs';
import {DateOnlyInterceptor} from './date-only.interceptor';

describe('DateOnlyInterceptor', () => {
    let interceptor: DateOnlyInterceptor;

    const nextMock = {
        handle: jest.fn(),
    };

    beforeEach(() => {
        interceptor = new DateOnlyInterceptor();
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(interceptor).toBeTruthy();
    });

    it('should convert outgoing Date objects to DateOnly strings', done => {
        const testDate = new Date(2024, 0, 15); // Jan 15, 2024
        const body = {
            beginDate: testDate,
            endDate: new Date(2024, 11, 31),
            nested: {
                someDate: new Date(2023, 5, 10),
            },
        };

        const request = new HttpRequest('POST', '/api/test', body);
        nextMock.handle.mockReturnValue(of(new HttpResponse({body: {}})));

        interceptor.intercept(request, nextMock).subscribe(() => {
            const capturedRequest = nextMock.handle.mock.calls[0][0] as HttpRequest<unknown>;
            expect(capturedRequest.body).toEqual({
                beginDate: '2024-01-15',
                endDate: '2024-12-31',
                nested: {
                    someDate: '2023-06-10',
                },
            });
            done();
        });
    });

    it('should convert incoming DateOnly strings to Date objects', done => {
        const responseBody = {
            beginDate: '2024-01-15',
            endDate: '2024-12-31',
            nested: {
                someDate: '2023-06-10',
            },
            // Should not convert datetime strings
            created: '2024-01-15T10:30:00Z',
        };

        const request = new HttpRequest('GET', '/api/test');
        nextMock.handle.mockReturnValue(of(new HttpResponse({body: responseBody})));

        interceptor.intercept(request, nextMock).subscribe((event: unknown) => {
            if (event instanceof HttpResponse) {
                const body = event.body as Record<string, unknown>;
                expect(body.beginDate).toBeInstanceOf(Date);
                expect(body.endDate).toBeInstanceOf(Date);
                expect((body.beginDate as Date).getFullYear()).toBe(2024);
                expect((body.beginDate as Date).getMonth()).toBe(0); // January
                expect((body.beginDate as Date).getDate()).toBe(15);
                // Should leave datetime strings alone
                expect(body.created).toBe('2024-01-15T10:30:00Z');
                done();
            }
        });
    });

    it('should handle arrays', done => {
        const body = {
            dates: [new Date(2024, 0, 1), new Date(2024, 0, 2)],
        };

        const request = new HttpRequest('POST', '/api/test', body);
        nextMock.handle.mockReturnValue(of(new HttpResponse({body: {}})));

        interceptor.intercept(request, nextMock).subscribe(() => {
            const capturedRequest = nextMock.handle.mock.calls[0][0] as HttpRequest<unknown>;
            expect(capturedRequest.body).toEqual({
                dates: ['2024-01-01', '2024-01-02'],
            });
            done();
        });
    });

    it('should handle null and undefined values', done => {
        const body = {
            beginDate: null,
            endDate: undefined,
            someString: 'test',
        };

        const request = new HttpRequest('POST', '/api/test', body);
        nextMock.handle.mockReturnValue(of(new HttpResponse({body: {}})));

        interceptor.intercept(request, nextMock).subscribe(() => {
            const capturedRequest = nextMock.handle.mock.calls[0][0] as HttpRequest<unknown>;
            expect(capturedRequest.body).toEqual({
                beginDate: null,
                endDate: undefined,
                someString: 'test',
            });
            done();
        });
    });

    it('should skip date conversion for FormData requests', done => {
        const formData = new FormData();
        const testDate = new Date(2024, 0, 15);
        formData.append('beginDate', testDate.toISOString());
        formData.append('someText', 'test value');

        const request = new HttpRequest('POST', '/api/test', formData);
        nextMock.handle.mockReturnValue(of(new HttpResponse({body: {}})));

        interceptor.intercept(request, nextMock).subscribe(() => {
            const capturedRequest = nextMock.handle.mock.calls[0][0] as HttpRequest<unknown>;
            // FormData should be passed through unchanged
            expect(capturedRequest.body).toBeInstanceOf(FormData);
            expect(capturedRequest.body).toBe(formData);
            done();
        });
    });

    it('should not modify FormData body', done => {
        const formData = new FormData();
        formData.append('id', '123');
        formData.append('name', 'test');

        const request = new HttpRequest('POST', '/api/upload', formData);
        nextMock.handle.mockReturnValue(of(new HttpResponse({body: {}})));

        interceptor.intercept(request, nextMock).subscribe(() => {
            const capturedRequest = nextMock.handle.mock.calls[0][0] as HttpRequest<unknown>;
            expect(capturedRequest.body).toBe(formData);
            expect(capturedRequest.body).toBeInstanceOf(FormData);
            done();
        });
    });

    it('should not modify Blob responses', done => {
        const blobData = new Blob(['test file content'], {type: 'application/pdf'});
        const request = new HttpRequest('GET', '/api/download');
        nextMock.handle.mockReturnValue(of(new HttpResponse({body: blobData})));

        interceptor.intercept(request, nextMock).subscribe((event: unknown) => {
            if (event instanceof HttpResponse) {
                expect(event.body).toBeInstanceOf(Blob);
                expect(event.body).toBe(blobData);
                done();
            }
        });
    });
});
