import {HttpClient} from '@angular/common/http';
import {ContactPointCreate} from '@api/ContactPointCreate';
import {firstValueFrom, of} from 'rxjs';
import {ContactPointsService} from './contact-points.service';

describe('ContactPointsService', () => {
    let service: ContactPointsService;

    const httpClientMock = {
        get: jest.fn(() => of()),
        put: jest.fn(() => of()),
        post: jest.fn(() => of()),
        delete: jest.fn(() => of()),
    } as unknown as jest.Mocked<HttpClient>;

    beforeEach(() => {
        service = new ContactPointsService(httpClientMock);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get contact point list', async () => {
        const mockResponse = {
            id: '1',
            description: 'Description 1',
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getContactPointList('1'));
        expect(response).toBeTruthy();

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/contact-points/1/list');
    });

    it('should get empty contact point for create', async () => {
        const mockResponse = {
            id: '1',
            committeeId: 'IAmAGuid',
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getContactPointForCreate('1'));
        expect(response).toBeTruthy();

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/contact-points/1/create');
    });

    it('should create contactPoint', () => {
        const contactPointCreate = {} as ContactPointCreate;

        service.createContactPoint(contactPointCreate);

        expect(httpClientMock.post).toHaveBeenCalledWith('/api/contact-points', contactPointCreate);
    });

    it('should get contact point for update', async () => {
        const mockResponse = {
            id: '1',
            committeeId: 'IAmAGuid',
        };
        httpClientMock.get.mockReturnValue(of(mockResponse));

        const response = await firstValueFrom(service.getContactPointForUpdate('1'));
        expect(response).toBeTruthy();

        expect(httpClientMock.get).toHaveBeenCalledWith('/api/contact-points/1/update');
    });

    it('should update contact point', () => {
        service.updateContactPoint({
            id: '1',
            committeeId: 'IAmAGuid',
            committeeDetailId: 'MeToo',
            contactPointTypeUri: 'my.uri.com',
            beginDate: new Date(2025, 0, 1),
            endDate: new Date(2025, 0, 1),
            companyName: 'Test Amt',
            section: 'Sektion A',
            languageId: 'lang',
            genderId: 'gender',
            surname: 'surname',
            givenName: 'givenname',
            street: 'Strasse',
            poBox: '',
            phone: 'phone',
            email: 'email',
            personalPhone: 'persPhon',
            personalMobile: 'persMobile',
            personalEmail: 'persEmail@mail.com',
            zip: '5600',
            city: 'Teststadt',
            title: 'title',
            releasePersonData: false,
            isCopy: false,
            committeeBeginDate: new Date(2024, 0, 1),
            rowVersion: 666,
        });

        expect(httpClientMock.put).toHaveBeenCalledWith('/api/contact-points/1', {
            id: '1',
            committeeId: 'IAmAGuid',
            committeeDetailId: 'MeToo',
            contactPointTypeUri: 'my.uri.com',
            beginDate: new Date(2025, 0, 1),
            endDate: new Date(2025, 0, 1),
            companyName: 'Test Amt',
            section: 'Sektion A',
            languageId: 'lang',
            genderId: 'gender',
            surname: 'surname',
            givenName: 'givenname',
            street: 'Strasse',
            poBox: '',
            phone: 'phone',
            email: 'email',
            personalPhone: 'persPhon',
            personalMobile: 'persMobile',
            personalEmail: 'persEmail@mail.com',
            zip: '5600',
            city: 'Teststadt',
            title: 'title',
            releasePersonData: false,
            isCopy: false,
            committeeBeginDate: new Date(2024, 0, 1),
            rowVersion: 666,
        });
    });

    it('should delete contact point', () => {
        service.deleteContactPoint('1');

        expect(httpClientMock.delete).toHaveBeenCalledWith('/api/contact-points/1');
    });
});
