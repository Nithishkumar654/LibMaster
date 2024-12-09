import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ApiService } from '../../services/api.service';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Reservation } from '../../shared/interfaces';
import { SharedModule } from '../../shared/shared.module';

/**
 * ReservedBooks component to display and manage the user's reserved books.
 */
@Component({
    selector: 'app-reserved-books',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule],
    templateUrl: './reserved-books.component.html',
    styleUrl: './reserved-books.component.scss'
})
export class ReservedBooksComponent implements OnInit, AfterViewInit {

    displayedColumns: string[] = ['title', 'author', 'reservedDate', 'status', 'options'];
    dataSource = new MatTableDataSource<Reservation>();

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    isLoading: boolean = false;
    loggedIn: boolean = false;
    member: boolean = false;

    /**
     * @param _api The ApiService to interact with the backend API.
     * @param _fb The FormBuilder for handling forms (not used here but available for future extensions).
     */
    constructor(public _api: ApiService, private _fb: FormBuilder) { }

    /**
     * Initializes the component and subscribes to login and role status.
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
        this.getBooks();
    }

    /**
     * Ensures pagination and sorting are set after the view initialization.
     */
    ngAfterViewInit(): void {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }

    /**
     * Fetches the list of reserved books from the API.
     */
    private getBooks(): void {
        this.isLoading = true;

        this._api.getReservedBooks().subscribe(
            (books) => {
                this.processReservedBooks(books.message);
                this.isLoading = false;
            },
            (error) => {
                this._api.openSnackBar('Error fetching books: ' + error.error.error, 'error-color');
                this.isLoading = false;
            }
        );
    }

    /**
     * Processes and sets the data for reserved books.
     * @param books The list of reserved books.
     */
    private processReservedBooks(books: Reservation[]): void {
        this.dataSource.data = books;
    }

    /**
     * Withdraws a reservation for a specific book.
     * @param id The ID of the reserved book to cancel.
     */
    withdraw(id: number): void {
        this._api.withdrawReservation(id.toString()).subscribe({
            next: (response) => {
                this._api.openSnackBar(response.body.message, 'success-color');
                this.getBooks();
            },
            error: (error) => {
                this._api.openSnackBar('Error withdrawing reservation: ' + error.error.error, 'error-color');
            }
        });
    }
}
