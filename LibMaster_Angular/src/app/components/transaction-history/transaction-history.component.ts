import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ApiService } from '../../services/api.service';
import { SharedModule } from '../../shared/shared.module';
import { Transaction } from '../../shared/interfaces';

/**
 * TransactionHistory component to display and manage the transaction history of a user.
 */
@Component({
    selector: 'app-transaction-history',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule],
    templateUrl: './transaction-history.component.html',
    styleUrl: './transaction-history.component.scss'
})
export class TransactionHistoryComponent implements OnInit, AfterViewInit {

    /** Columns displayed in the transaction history table */
    displayedColumns: string[] = ['sno', 'transactionType', 'amount', 'date', 'details'];
    dataSource = new MatTableDataSource<Transaction>();

    /** ViewChild references for paginator and sorter */
    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    /** Flag to indicate if the data is loading */
    isLoading: boolean = false;

    /** Flag to indicate if the user is logged in */
    loggedIn: boolean = false;

    /** Flag to check if the logged-in user is a member */
    member: boolean = false;

    /**
     * Constructor for TransactionHistoryComponent.
     * @param _api The ApiService to interact with backend APIs.
     * @param _fb The FormBuilder service to create reactive forms.
     */
    constructor(public _api: ApiService, private _fb: FormBuilder) { }

    /**
     * Initializes paginator and sorter after the view is initialized.
     */
    ngAfterViewInit(): void {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }

    /**
     * Initializes the component, subscribes to login and role changes, and fetches transaction data.
     */
    ngOnInit(): void {
        this._api.loggedIn.subscribe(status => {
            this.loggedIn = status;
        });
        if (this.loggedIn) {
            this._api.role.subscribe(r => {
                this.member = (r == 'member');
            });
        }
        this.getTransactions();
    }

    /**
     * Fetches the transaction history data from the API.
     */
    private getTransactions(): void {
        this.isLoading = true;

        this._api.getTransactions().subscribe(
            (transactions) => {
                this.dataSource.data = transactions.message;
                this.isLoading = false;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                this.paginator.firstPage();
            },
            (error) => {
                this._api.openSnackBar('Error: ' + error.error.error, 'error-color');
            }
        );
    }
}
