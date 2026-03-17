import {DatePipe, NgClass} from '@angular/common';
import {AfterViewInit, Component, computed, DestroyRef, OnDestroy, ViewChild} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
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
import {ContactPointList} from '@api/ContactPointList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObAlertModule, ObButtonDirective, ObNotificationService} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {combineLatest, distinctUntilChanged, map, merge, of, startWith, Subject, switchMap, tap} from 'rxjs';
import {ConfigsService} from '../../../../configs.service';
import {CommitteeDetailsService} from '../../committee-details.service';
import {ContactPointsService} from '../contact-points.service';

export type ContactPointsColumns =
    | 'contactPointType'
    | 'companyName'
    | 'section'
    | 'beginDate'
    | 'endDate'
    | 'street'
    | 'poBox'
    | 'zipCity'
    | 'phone'
    | 'email'
    | 'personName'
    | 'title'
    | 'language'
    | 'gender'
    | 'personalPhone'
    | 'personalMobile'
    | 'personalEmail'
    | 'releasePersonData'
    | 'actions';

@Component({
    selector: 'apg-contact-point-list',
    templateUrl: './contact-point-list.component.html',
    styleUrl: './contact-point-list.component.scss',
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
        TranslatePipe,
        ObAlertModule,
    ],
})
export class ContactPointListComponent implements AfterViewInit, OnDestroy {
    committeeId!: string;

    readonly displayedColumns: ContactPointsColumns[] = [
        'contactPointType',
        'beginDate',
        'endDate',
        'companyName',
        'personName',
        'phone',
        'email',
        'zipCity',
        'actions',
    ];

    readonly reload$ = new Subject<void>();

    dataSource = new MatTableDataSource<ContactPointList>();
    committeeDetails = this.committeeDetailsService.committeeDetails;
    canEdit = computed(() => this.committeeDetailsService.committeeDetails()?.canEdit);

    @ViewChild(MatSort) sort!: MatSort;

    private readonly destroyed$ = new Subject<void>();
    private readonly refresh = new Subject<void>();

    constructor(
        protected readonly configsService: ConfigsService,
        protected readonly committeeDetailsService: CommitteeDetailsService,
        private readonly translateService: TranslateService,
        private readonly contactPointsService: ContactPointsService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly dialog: MatDialog,
        private readonly notificationService: ObNotificationService,
        private readonly dr: DestroyRef
    ) {
        const currentLanguage$ = this.translateService.onLangChange.pipe(
            startWith({lang: this.translateService.getCurrentLang()}),
            map(lang => lang.lang),
            distinctUntilChanged(),
            takeUntilDestroyed()
        );

        const refresh$ = merge(
            this.refresh.pipe(switchMap(() => of(this.route.snapshot.paramMap.get('id')))),
            this.route.paramMap.pipe(tap(paramMap => (this.committeeId = paramMap.get('id')!))),
            this.contactPointsService.reload$
        );
        const loading$ = combineLatest([refresh$, currentLanguage$]);

        loading$.pipe(switchMap(() => this.contactPointsService.getContactPointList(this.committeeId))).subscribe(contactPoints => {
            this.dataSource.data = contactPoints;
        });
    }

    ngAfterViewInit(): void {
        this.dataSource.sort = this.sort;
    }

    ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    editContactPoint(id: string) {
        void this.router.navigate(['contactpoints', id], {relativeTo: this.route});
    }

    copyContactPoint(id: string) {
        void this.router.navigate(['contactpoints', id, 'copy'], {relativeTo: this.route});
    }

    deleteContactPoint(id: string) {
        const dialogRef = this.dialog.open(ConfirmDialogComponent, {
            width: '500px',
            data: {
                title: this.translateService.instant('contactPoint.delete.title'),
                message: this.translateService.instant('contactPoint.delete.text'),
            },
        });

        dialogRef
            .afterClosed()
            .pipe(takeUntilDestroyed(this.dr))
            .subscribe(result => {
                if (result === true) {
                    this.contactPointsService.deleteContactPoint(id).subscribe({
                        next: () => {
                            this.contactPointsService.reload$.next();
                            return this.notificationService.success('contactPoint.delete.success');
                        },
                        error: () => this.notificationService.error('contactPoint.delete.error'),
                    });
                }
            });
    }

    createContactPoint() {
        void this.router.navigate(['contactpoints', 'create'], {relativeTo: this.route});
    }

    hasDataProtectionOfficer(): boolean {
        return (
            this.committeeDetailsService
                .committeeDetails()
                ?.contactPoints?.some(
                    y => (!y.endDate || y.endDate > new Date()) && y.contactPointTypeId === this.configsService.frontendConfig.entityIds.contactPoint.dpoId
                ) ?? false
        );
    }

    hasSecretariat(): boolean {
        return (
            this.committeeDetailsService
                .committeeDetails()
                ?.contactPoints?.some(
                    y =>
                        (!y.endDate || y.endDate > new Date()) &&
                        y.contactPointTypeId === this.configsService.frontendConfig.entityIds.contactPoint.secretariatId
                ) ?? false
        );
    }
}
