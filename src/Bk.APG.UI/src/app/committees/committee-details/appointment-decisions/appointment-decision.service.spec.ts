import {HttpClient, HttpParams} from '@angular/common/http';
import {of, firstValueFrom} from 'rxjs';
import {AppointmentDecisionService} from './appointment-decision.service';

describe('AppointmentDecisionService', () => {
    let service: AppointmentDecisionService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        post: jest.fn(),
        put: jest.fn(),
        delete: jest.fn(),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new AppointmentDecisionService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get appointment decision list', async () => {
        const mockResponse = {
            id: '1',
            description: 'Description 1',
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));
        httpClientMock.post.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getAppointmentDecisionList('1'));
        expect(response).toBeTruthy();

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/appointment-decisions/1/list');
    });

    it('should create appointment decision', () => {
        const file = new File(['foo'], 'foo.txt', {
            type: 'text/plain',
        });
        const appointmentDecisionCreate = {
            committeeId: '1',
            appointmentDecisionDate: new Date(2025, 1, 1),
            appointmentDecisionTypeId: '2',
            appointmentDecisionLinkTypeId: '3',
            text: 'text',
            link: 'link',
            documents: [
                {
                    displayName: 'dName',
                    isOriginal: true,
                    languageId: 'DE',
                    file,
                },
            ],
        };

        service.createAppointmentDecision(appointmentDecisionCreate);

        expect(httpClientMock.post).toHaveBeenCalledWith('/api/appointment-decisions', expect.any(FormData));

        const [url, body] = (httpClientMock.post as jest.Mock).mock.calls[0];

        expect(url).toBe('/api/appointment-decisions');
        expect(body instanceof FormData).toBe(true);
        expect(body.get('appointmentDecisionDate')).toBe('2025-02-01');
        expect(body.get('appointmentDecisionTypeId')).toBe('2');
        expect(body.get('appointmentDecisionLinkTypeId')).toBe('3');
        expect(body.get('text')).toBe('text');
        expect(body.get('link')).toBe('link');

        expect(body.get('documents[0][id]')).toBe(null);
        expect(body.get('documents[0][displayName]')).toBe('dName');
        expect(body.get('documents[0][isOriginal]')).toBe('true');
        expect(body.get('documents[0][languageId]')).toBe('DE');

        const fileFromFormData = body.get('documents[0].file') as File;
        expect(fileFromFormData).toBeInstanceOf(File);
        expect(fileFromFormData.name).toBe('foo.txt');
    });

    it('should update appointment decision', () => {
        const file = new File(['foo'], 'foo.txt', {
            type: 'text/plain',
        });
        const appointmentDecisionUpdate = {
            id: '1',
            appointmentDecisionDate: new Date(2025, 1, 1),
            appointmentDecisionTypeId: '2',
            appointmentDecisionLinkTypeId: '3',
            text: 'text',
            link: 'link',
            documents: [
                {
                    id: 'docId',
                    displayName: 'dName',
                    isOriginal: true,
                    languageId: 'DE',
                    file,
                },
            ],
        };

        service.updateAppointmentDecision('1', appointmentDecisionUpdate);

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/appointment-decisions/1', expect.any(FormData));

        const [url, body] = (httpClientMock.put as jest.Mock).mock.calls[0];

        expect(url).toBe('/api/appointment-decisions/1');
        expect(body instanceof FormData).toBe(true);
        expect(body.get('appointmentDecisionDate')).toBe('2025-02-01');
        expect(body.get('appointmentDecisionTypeId')).toBe('2');
        expect(body.get('appointmentDecisionLinkTypeId')).toBe('3');
        expect(body.get('text')).toBe('text');
        expect(body.get('link')).toBe('link');

        expect(body.get('documents[0][id]')).toBe('docId');
        expect(body.get('documents[0][displayName]')).toBe('dName');
        expect(body.get('documents[0][isOriginal]')).toBe('true');
        expect(body.get('documents[0][languageId]')).toBe('DE');

        const fileFromFormData = body.get('documents[0].file') as File;
        expect(fileFromFormData).toBeInstanceOf(File);
        expect(fileFromFormData.name).toBe('foo.txt');
    });

    it('should delete appointment decision', () => {
        service.deleteAppointmentDecision('1');

        expect(httpClientMock.delete).toHaveBeenCalledWith('/api/appointment-decisions/1');
    });

    it('should download a document', () => {
        const documentId = 'DocId';
        const expectedParams = new HttpParams().set('id', documentId);

        service.downloadFile(documentId).subscribe(response => {
            expect(response).toBeTruthy();
        });

        expect(httpClientMock.get).toHaveBeenCalledWith(
            '/api/appointment-decisions/document',
            expect.objectContaining({
                params: expectedParams,
            })
        );
    });
});
