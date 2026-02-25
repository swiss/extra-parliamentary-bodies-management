import {Component} from '@angular/core';
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
import {Router} from '@angular/router';
import {FormLettersSenderList} from '@api/FormLettersSenderList';
import {TranslatePipe, TranslateService} from '@ngx-translate/core';
import {ObButtonDirective} from '@oblique/oblique';
import {ConfirmDialogComponent} from '@shared/confirm-dialog/confirm-dialog.component';
import {distinctUntilChanged, map, startWith, switchMap} from 'rxjs';
import {FormLettersSenderService} from './form-letters-sender.service';

export type FormLettersSenderColumns = 'description' | 'surname' | 'givenName' | 'senderFunction' | 'department' | 'actions';

@Component({
    selector: 'apg-form-letters-sender',
    templateUrl: './form-letters-sender.component.html',
    styleUrl: './form-letters-sender.component.scss',
    imports: [
        MatTable,
        MatColumnDef,
        MatHeaderCellDef,
        MatHeaderCell,
        MatCellDef,
        MatCell,
        MatHeaderRowDef,
        MatHeaderRow,
        MatRowDef,
        MatRow,
        MatNoDataRow,
        MatIcon,
        MatIconButton,
        MatButton,
        MatSort,
        MatSortHeader,
        MatTooltip,
        ObButtonDirective,
        TranslatePipe,
    ],
})
export class FormLettersSenderComponent {
    readonly displayedColumns: FormLettersSenderColumns[] = ['description', 'senderFunction', 'surname', 'givenName', 'department', 'actions'];
    dataSource = new MatTableDataSource<FormLettersSenderList>();

    constructor(
        protected readonly formLettersSenderService: FormLettersSenderService,
        private readonly translateService: TranslateService,
        private readonly router: Router,
        private readonly dialog: MatDialog
    ) {
        this.translateService.onLangChange
            .pipe(
                startWith({lang: this.translateService.getCurrentLang()}),
                map(lang => lang.lang),
                distinctUntilChanged(),
                switchMap(() => this.formLettersSenderService.getFormLettersSenderList()),
                takeUntilDestroyed()
            )
            .subscribe(senders => {
                this.dataSource.data = senders;
            });

        this.formLettersSenderService.reload$
            .pipe(
                switchMap(() => this.formLettersSenderService.getFormLettersSenderList()),
                takeUntilDestroyed()
            )
            .subscribe(senders => {
                this.dataSource.data = senders;
            });
    }

    editSender(id: string) {
        void this.router.navigate(['general-election', 'exports', 'formLettersSenders', id]);
    }

    createSender() {
        void this.router.navigate(['general-election', 'exports', 'formLettersSenders', 'create']);
    }

    deleteSender(sender: FormLettersSenderList) {
        this.dialog
            .open(ConfirmDialogComponent, {
                data: {
                    message: this.translateService.instant('formLetter.sender.delete.confirm', {
                        sender: sender.description,
                    }),
                },
            })
            .afterClosed()
            .subscribe(confirmed => {
                if (confirmed) {
                    this.formLettersSenderService.deleteFormLettersSender(sender.id).subscribe({
                        next: () => {
                            this.formLettersSenderService.reload$.next();
                        },
                    });
                }
            });
    }
}
