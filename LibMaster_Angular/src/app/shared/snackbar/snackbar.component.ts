import { Component, Inject, inject, Input } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_SNACK_BAR_DATA, MatSnackBar, MatSnackBarAction, MatSnackBarActions, MatSnackBarLabel, MatSnackBarRef } from '@angular/material/snack-bar';
import { SharedModule } from '../shared.module';

@Component({
    selector: 'app-snackbar',
    standalone: true,
    imports: [SharedModule, MatSnackBarLabel, MatButtonModule, MatSnackBarActions, MatSnackBarAction],
    templateUrl: './snackbar.component.html',
    styleUrl: './snackbar.component.scss'
})
export class SnackbarComponent {
    snackBarRef = inject(MatSnackBarRef);
    constructor(@Inject(MAT_SNACK_BAR_DATA) public data: any) { }
}
