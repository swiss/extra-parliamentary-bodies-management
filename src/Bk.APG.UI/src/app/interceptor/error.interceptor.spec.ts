import {HttpErrorResponse, HttpEvent, HttpHandler, HttpRequest} from '@angular/common/http';
import {ObNotificationService} from '@oblique/oblique';
import {of, throwError} from 'rxjs';
import {ErrorInterceptor} from './error.interceptor';

describe('ErrorInterceptor', () => {
    let interceptor: ErrorInterceptor;
    let notificationService: ObNotificationService;
    let req: HttpRequest<unknown>;

    beforeEach(() => {
        notificationService = {
            error: jest.fn(),
        } as unknown as ObNotificationService;

        interceptor = new ErrorInterceptor(notificationService);
        req = new HttpRequest('GET', '/test');
    });

    it('should pass through successful responses', () => {
        const dummyEvent = {type: 0} as HttpEvent<unknown>;
        const handler: HttpHandler = {
            handle: jest.fn(() => of(dummyEvent)),
        };

        const nextSpy = jest.fn();

        interceptor.intercept(req, handler).subscribe(nextSpy);

        expect(notificationService.error).not.toHaveBeenCalled();
        expect(nextSpy).toHaveBeenCalledWith(dummyEvent);
    });

    it('should catch 409 conflict and call notificationService.error, without propagating the error', () => {
        const conflictError = new HttpErrorResponse({
            status: 409,
            statusText: 'Conflict',
            error: {message: 'Conflict occurred'},
        });

        const handler: HttpHandler = {
            handle: jest.fn(() => throwError(() => conflictError)),
        };

        interceptor.intercept(req, handler).subscribe({
            next: () => fail('Should not emit next value'),
            error: () => fail('Should not propagate error'),
        });

        expect(notificationService.error).toHaveBeenCalledWith('error.conflict');
    });

    it('should propagate non-409 errors', () => {
        const serverError = new HttpErrorResponse({
            status: 500,
            statusText: 'Server Error',
            error: {message: 'Something went wrong'},
        });

        const handler: HttpHandler = {
            handle: jest.fn(() => throwError(() => serverError)),
        };

        let capturedError: unknown;

        interceptor.intercept(req, handler).subscribe({
            next: () => fail('Should not emit next value'),
            error: err => (capturedError = err),
        });

        expect(capturedError).toBe(serverError);
        expect(notificationService.error).not.toHaveBeenCalled();
    });
});
