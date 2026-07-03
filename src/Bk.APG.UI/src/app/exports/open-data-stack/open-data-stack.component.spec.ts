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
    const initialDashboardId = 'foo';

    const configsServiceMock = {
        frontendConfig: {
            openDataStack: {
                baseUrl,
                initialDashboardId,
            },
        },
    } as Partial<ConfigsService>;

    const translateServiceMock = {
        getCurrentLang: jest.fn(() => 'de'),
    };

    beforeEach(async () => {
        openDataStackServiceMock = {
            exchangeToken: jest.fn().mockReturnValue(of('test-token')),
            getDashboards: jest.fn().mockReturnValue(of([])),
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
        const expectedDashboardUrl = `${baseUrl}/embedded-analytics/bootstrap/?redirect=${encodeURIComponent(`/superset/dashboard/${initialDashboardId}/?standalone=2`)}`;

        expect(openDataStackServiceMock.exchangeToken).toHaveBeenCalledTimes(1);
        expect(iframe.src).toBe(expectedDashboardUrl);
        expect(iframe.style.display).toBe('none');
        expect(typeof iframe.onload).toBe('function');
    });

    it('should post embedded auth on first iframe load and fetch dashboards on second load', () => {
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
        expect(openDataStackServiceMock.getDashboards).not.toHaveBeenCalled();
        expect(typeof iframe.onload).toBe('function');

        const secondOnLoad = iframe.onload;
        secondOnLoad?.call(iframe, new Event('load'));

        expect(openDataStackServiceMock.getDashboards).toHaveBeenCalledTimes(1);
        expect(iframe.onload).toBeNull();
    });

    it('should load the first dashboard in iframe after dashboards are fetched', () => {
        openDataStackServiceMock.getDashboards.mockReturnValueOnce(
            of([
                {
                    id: 'dashboard-1',
                    title: 'Dashboard 1',
                    status: 'published',
                    embedRedirect: '/superset/dashboard/dashboard-1/?standalone=2',
                },
            ])
        );

        fixture.detectChanges();

        const iframe = fixture.nativeElement.querySelector('#ods-embed') as HTMLIFrameElement;
        const postMessageMock = jest.fn();

        Object.defineProperty(iframe, 'contentWindow', {
            value: {postMessage: postMessageMock},
            configurable: true,
        });

        const onLoad = iframe.onload;
        onLoad?.call(iframe, new Event('load'));

        const secondOnLoad = iframe.onload;
        secondOnLoad?.call(iframe, new Event('load'));
        fixture.detectChanges();

        expect(openDataStackServiceMock.getDashboards).toHaveBeenCalledTimes(1);
        expect(iframe.src).toBe(`${baseUrl}/superset/dashboard/dashboard-1/?standalone=2`);
        expect(iframe.style.display).toBe('block');
    });

    it('should keep iframe hidden when no dashboards are returned', () => {
        fixture.detectChanges();

        const iframe = fixture.nativeElement.querySelector('#ods-embed') as HTMLIFrameElement;
        const postMessageMock = jest.fn();

        Object.defineProperty(iframe, 'contentWindow', {
            value: {postMessage: postMessageMock},
            configurable: true,
        });

        const onLoad = iframe.onload;
        onLoad?.call(iframe, new Event('load'));

        const secondOnLoad = iframe.onload;
        secondOnLoad?.call(iframe, new Event('load'));
        fixture.detectChanges();

        expect(openDataStackServiceMock.getDashboards).toHaveBeenCalledTimes(1);
        expect(iframe.style.display).toBe('none');
    });

    it('should return early when iframe reference is missing', () => {
        fixture.detectChanges();

        const callsBefore = openDataStackServiceMock.exchangeToken.mock.calls.length;

        (component as unknown as {iframeRef?: undefined}).iframeRef = undefined;

        component.ngAfterViewInit();

        expect(openDataStackServiceMock.exchangeToken).toHaveBeenCalledTimes(callsBefore + 1);
    });
});
