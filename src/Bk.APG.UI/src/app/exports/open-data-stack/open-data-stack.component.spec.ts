import {ComponentFixture, TestBed} from '@angular/core/testing';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {MockPipe} from 'ng-mocks';
import {of} from 'rxjs';
import {ConfigsService} from '../../configs.service';
import {OpenDataStackComponent} from './open-data-stack.component';
import {OpenDataStackService} from './open-data-stack.service';

describe('OpenDataStackComponent', () => {
    let component: OpenDataStackComponent;
    let fixture: ComponentFixture<OpenDataStackComponent>;
    let openDataStackServiceMock: jest.Mocked<OpenDataStackService>;

    const baseUrl = 'https://ods.example.com';
    const dashboard = '/dashboard/foo';

    const configsServiceMock = {
        frontendConfig: {
            openDataStack: {
                baseUrl,
                dashboard,
            },
        },
    } as Partial<ConfigsService>;

    const translateServiceMock = {
        getCurrentLang: jest.fn(() => 'de'),
    };

    beforeEach(async () => {
        openDataStackServiceMock = {
            exchangeToken: jest.fn().mockReturnValue(of('test-token')),
        } as unknown as jest.Mocked<OpenDataStackService>;

        await TestBed.configureTestingModule({
            imports: [OpenDataStackComponent, MockPipe(TranslatePipe)],
            providers: [
                {provide: ConfigsService, useValue: configsServiceMock},
                {provide: OpenDataStackService, useValue: openDataStackServiceMock},
                {provide: TranslateService, useValue: translateServiceMock},
            ],
        }).compileComponents();

        fixture = TestBed.createComponent(OpenDataStackComponent);
        component = fixture.componentInstance;
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    it('should create', () => {
        fixture.detectChanges();

        expect(component).toBeTruthy();
    });

    it('should initialize iframe src after token exchange', () => {
        fixture.detectChanges();

        const iframe = fixture.nativeElement.querySelector('#ods-embed') as HTMLIFrameElement;
        const expectedDashboardUrl = `${baseUrl}/embedded-analytics/bootstrap/?redirect=${encodeURIComponent(dashboard)}`;

        expect(openDataStackServiceMock.exchangeToken).toHaveBeenCalledTimes(1);
        expect(iframe.src).toBe(expectedDashboardUrl);
        expect(typeof iframe.onload).toBe('function');
    });

    it('should post embedded auth message to ods origin on iframe load', () => {
        fixture.detectChanges();

        const iframe = fixture.nativeElement.querySelector('#ods-embed') as HTMLIFrameElement;
        const postMessageMock = jest.fn();

        Object.defineProperty(iframe, 'contentWindow', {
            value: {postMessage: postMessageMock},
            configurable: true,
        });

        const onLoad = iframe.onload;
        expect(onLoad).toBeTruthy();

        onLoad?.call(iframe, new Event('load'));

        expect(postMessageMock).toHaveBeenCalledWith({type: 'embedded-auth', code: 'test-token'}, baseUrl);
        expect(iframe.onload).toBeNull();
    });

    it('should return early when iframe reference is missing', () => {
        fixture.detectChanges();

        (component as unknown as {iframeRef?: undefined}).iframeRef = undefined;

        component.ngAfterViewInit();

        expect(openDataStackServiceMock.exchangeToken).toHaveBeenCalledTimes(2);
    });
});
