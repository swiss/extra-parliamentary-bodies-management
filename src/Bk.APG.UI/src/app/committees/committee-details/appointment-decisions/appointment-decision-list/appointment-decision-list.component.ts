import {DatePipe, NgClass} from '@angular/common';
import {AfterViewInit, Component, computed, OnDestroy, ViewChild} from '@angular/core';
import {takeUntilDestroyed, toSignal} from '@angular/core/rxjs-interop';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatDialog} from '@angular/material/dialog';
import {MatIcon} from '@angular/material/icon';
import {MatSort, MatSortHeader} from '@angular/material/sort';
import {
    MatCell,
    MatCellDef,
    MatColumnDef,
    MatHeaderCell,
    MatHeaderCellDef,
    MatHeaderRow,
    MatHeaderRowDef,
    MatNoDataRow,
    MatRow,
    MatRowDef,
    MatTable,
    MatTableDataSource,
} from '@angular/material/table';
import {MatTooltip} from '@angular/material/tooltip';
import {ActivatedRoute, Router} from '@angular/router';
import {AppointmentDecisionList} from '@api/AppointmentDecisionList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {HelpTooltipComponent} from '@shared/help-tooltip/help-tooltip.component';
import {combineLatest, distinctUntilChanged, map, merge, of, startWith, Subject, switchMap, tap} from 'rxjs';
import {AuthService} from '../../../../auth/auth.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {AppointmentDecisionService} from '../appointment-decision.service';

export type AppointmentDecisionColumns =
    | 'appointmentDecisionDate'
    | 'appointmentDecisionType'
    | 'appointmentDecisionLinkType'
    | 'text'
    | 'link'
    | 'fileName'
    | 'modified'
    | 'actions';

@Component({
    selector: 'apg-appointment-decision-list',
    templateUrl: './appointment-decision-list.component.html',
    styleUrl: './appointment-decision-list.component.scss',
    imports: [
        MatButton,
        ObButtonDirective,
        MatTooltip,
        MatIcon,
        MatIconButton,
        MatTable,
        MatSort,
        MatColumnDef,
        MatHeaderCellDef,
        MatHeaderCell,
        MatSortHeader,
        MatCellDef,
        MatCell,
        NgClass,
        MatHeaderRowDef,
        MatHeaderRow,
        MatRowDef,
        MatRow,
        MatNoDataRow,
        DatePipe,
        HelpTooltipComponent,
        TranslatePipe,
        ObAlertModule,
    ],
})
export class AppointmentDecisionListComponent implements AfterViewInit, OnDestroy {
    committeeId!: string;

    readonly displayedColumns: AppointmentDecisionColumns[] = [
        'appointmentDecisionDate',
        'appointmentDecisionType',
        'fileName',
        'text',
        'appointmentDecisionLinkType',
        'link',
        'modified',
        'actions',
    ];

    dataSource = new MatTableDataSource<AppointmentDecisionList>();

    canEdit = computed(() => this.committeeDetailsService.committeeDetails()?.canEdit);

    @ViewChild(MatSort) sort!: MatSort;

    protected isAdmin = toSignal(this.authService.isAdmin$);
    protected isDepartmentUser = toSignal(this.authService.isDepartmentUser$);
    protected isSecretariatUser = toSignal(this.authService.isSecretariatUser$);
    protected isOfficeUser = toSignal(this.authService.isOfficeUser$);

    private readonly destroyed$ = new Subject<void>();
    private readonly refresh = new Subject<void>();

    constructor(
        protected readonly committeeDetailsService: CommitteeDetailsService,
        private readonly translateService: TranslateService,
        private readonly appointmentDecisionService: AppointmentDecisionService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly dialog: MatDialog,
        private readonly notificationService: ObNotificationService,
        private readonly authService: AuthService
    ) {
        const currentLanguage$ = this.translateService.onLangChange.pipe(
            startWith({lang: this.translateService.currentLang}),
            map(lang => lang.lang),
            distinctUntilChanged(),
            takeUntilDestroyed()
        );

        const refresh$ = merge(
            this.refresh.pipe(switchMap(() => of(this.route.snapshot.paramMap.get('id')))),
            this.route.paramMap.pipe(tap(paramMap => (this.committeeId = paramMap.get('id')!))),
            this.appointmentDecisionService.reload$
        );
        const loading$ = combineLatest([refresh$, currentLanguage$]);

        loading$.pipe(switchMap(() => this.appointmentDecisionService.getAppointmentDecisionList(this.committeeId))).subscribe(appointmentDecisions => {
            this.dataSource.data = appointmentDecisions;
        });
    }

    ngAfterViewInit(): void {
        this.dataSource.sort = this.sort;
    }

    ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    createAppointmentDecision() {
        void this.router.navigate([`committees/${this.committeeId}/appointmentDecisions/create`]);
    }

    editAppointmentDecision(id: string) {
        void this.router.navigate(['appointmentDecisions', id], {relativeTo: this.route});
    }

    deleteEntry(id: string) {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '400px',
            data: {
                title: this.translateService.instant('appointmentDecision.delete.title'),
                message: this.translateService.instant('appointmentDecision.delete.text'),
            },
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result === true) {
                this.appointmentDecisionService.deleteAppointmentDecision(id).subscribe({
                    next: async () => {
                        this.appointmentDecisionService.reload$.next();
                        void this.router.navigate(['committees', this.committeeId], {replaceUrl: true, queryParams: {tab: 'decisions'}});
                        return this.notificationService.success('appointmentDecision.delete.success');
                    },
                    error: () => this.notificationService.error('appointmentDecision.delete.error'),
                });
            }
        });
    }

    downloadDocument(id: string, fileName: string): void {
        this.appointmentDecisionService.downloadFile(id).subscribe({
            next: blob => {
                const suggestedFileName = fileName;
                const url = window.URL.createObjectURL(blob);
                const anchor = document.createElement('a');
                anchor.href = url;
                anchor.download = suggestedFileName;
                anchor.click();
                window.URL.revokeObjectURL(url);
            },
            error: error => {
                console.error('Download failed:', error);
            },
        });
    }
}
