import {AfterViewInit, Component, ElementRef, Signal, ViewChild, effect, inject, signal} from '@angular/core';
import {toSignal} from '@angular/core/rxjs-interop';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {MatOption} from '@angular/material/core';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatSelect} from '@angular/material/select';
import {OpenDataStackDashboard} from '@api/OpenDataStackDashboard';
import {TranslatePipe} from '@ngx-translate/core';
import {ConfigsService} from '../../configs.service';
import {OpenDataStackService} from './open-data-stack.service';

interface DashboardForm {
    dashboardId: FormControl<string | null>;
}

@Component({
    selector: 'apg-open-data-stack',
    templateUrl: './open-data-stack.component.html',
    styleUrls: ['./open-data-stack.component.scss'],
    imports: [ReactiveFormsModule, TranslatePipe, MatFormField, MatLabel, MatSelect, MatOption],
})
export class OpenDataStackComponent implements AfterViewInit {
    protected readonly dashboardForm: FormGroup<DashboardForm>;

    protected readonly dashboards = signal<OpenDataStackDashboard[]>([]);
    protected readonly isIframeVisible = signal(false);

    private readonly configsService = inject(ConfigsService);
    private readonly openDataStackService = inject(OpenDataStackService);
    private readonly formBuilder = inject(FormBuilder);

    private readonly selectedDashboard: Signal<string | null>;

    @ViewChild('odsEmbed')
    private readonly iframeRef?: ElementRef<HTMLIFrameElement>;

    constructor() {
        this.dashboardForm = this.formBuilder.group<DashboardForm>({
            dashboardId: this.formBuilder.control<string | null>(null),
        });

        this.selectedDashboard = toSignal(this.dashboardForm.controls.dashboardId.valueChanges, {initialValue: this.dashboardForm.controls.dashboardId.value});

        effect(() => {
            if (this.selectedDashboard()) {
                this.onDashboardChange();
            }
        });
    }

    public ngAfterViewInit(): void {
        this.openDataStackService.exchangeToken().subscribe(code => {
            const iframe = this.iframeRef?.nativeElement;
            if (!iframe) {
                return;
            }

            const initialDashboard = `/superset/dashboard/${this.configsService.frontendConfig.openDataStack.initialDashboardId}/?standalone=2`;
            iframe.src = `${this.configsService.frontendConfig.openDataStack.baseUrl}/embedded-analytics/bootstrap/?redirect=${encodeURIComponent(
                initialDashboard
            )}`;

            iframe.onload = () => {
                iframe.contentWindow?.postMessage({type: 'embedded-auth', code}, new URL(this.configsService.frontendConfig.openDataStack.baseUrl).origin);

                // Wait for the auth redirect/reload before loading dashboards.
                iframe.onload = () => {
                    this.loadDashboards();
                    iframe.onload = null;
                };
            };
        });
    }

    private loadDashboards(): void {
        this.openDataStackService.getDashboards().subscribe(dashboards => {
            this.dashboards.set(dashboards);
            if (dashboards.length > 0) {
                this.dashboardForm.controls.dashboardId.setValue(dashboards[0].id);
            }
        });
    }

    private onDashboardChange(): void {
        const selectedId = this.selectedDashboard();
        if (!selectedId) {
            return;
        }

        const selectedDashboard = this.dashboards().find(d => d.id === selectedId);
        if (!selectedDashboard) {
            return;
        }

        const iframe = this.iframeRef?.nativeElement;
        if (!iframe) {
            return;
        }

        const dashboardPath = `${this.configsService.frontendConfig.openDataStack.baseUrl}${selectedDashboard.embedRedirect}`;
        iframe.src = dashboardPath;
        this.isIframeVisible.set(true);
    }
}
