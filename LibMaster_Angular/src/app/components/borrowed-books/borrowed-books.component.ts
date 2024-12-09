import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { BorrowedBook } from '../../shared/interfaces';
import { MatPaginator } from '@angular/material/paginator';
import { ApiService } from '../../services/api.service';
import { MatSort } from '@angular/material/sort';
import { BorrowViewComponent } from '../../shared/borrow-view/borrow-view.component';

/**
 * Component for displaying and managing a list of borrowed books.
 */
@Component({
    selector: 'app-borrowed-books',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule],
    templateUrl: './borrowed-books.component.html',
    styleUrl: './borrowed-books.component.scss'
})
export class BorrowedBooksComponent implements OnInit, AfterViewInit {
    /** Reference to MatDialog for opening modals */
    readonly dialog = inject(MatDialog);

    /** Columns displayed in the borrowed books table */
    displayedColumns: string[] = ['title', 'author', 'borrowDate', 'dueDate', 'status', 'options'];

    /** Data source for the MatTable */
    dataSource = new MatTableDataSource<BorrowedBook>();

    /** Reference to the MatPaginator for pagination */
    @ViewChild(MatPaginator) paginator!: MatPaginator;

    /** Reference to the MatSort for sorting */
    @ViewChild(MatSort) sort!: MatSort;

    /** Indicates whether the user is logged in */
    loggedIn: boolean = false;

    /** Indicates whether the user has a member role */
    member: boolean = false;

    /** Loading state indicator */
    isLoading: boolean = false;

    /**
     * Constructor to inject services.
     * @param {ApiService} _api - Service to handle API calls
     * @param {FormBuilder} _fb - FormBuilder to create reactive forms
     */
    constructor(public _api: ApiService, private _fb: FormBuilder) { }

    /**
     * Lifecycle hook for AfterViewInit.
     * Initializes paginator and sort for the data table.
     * @returns {void}
     */
    ngAfterViewInit(): void {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
    }

    /**
     * Lifecycle hook for OnInit.
     * Checks user login and role status, then fetches borrowed books.
     * @returns {void}
     */
    ngOnInit(): void {
        this._api.loggedIn.subscribe((status: boolean) => {
            this.loggedIn = status;
        });

        if (this.loggedIn) {
            this._api.role.subscribe((r: string) => {
                this.member = (r === 'member');
            });
        }

        this.getBooks();
    }

    /**
     * Fetches the list of borrowed books from the API.
     * Processes the data and populates the table.
     * @returns {void}
     */
    private getBooks(): void {
        this.isLoading = true;

        this._api.getBorrowedBooks().subscribe(
            (books) => {
                const borrowedBooks: BorrowedBook[] = books.message.map((element: BorrowedBook) => ({
                    bookId: element.bookId,
                    memberId: element.memberId,
                    borrowDate: element.borrowDate,
                    dueDate: element.dueDate,
                    returnDate: element.returnDate,
                    lateFee: element.lateFee,
                    status: element.status,
                    book: {
                        bookId: element.book.bookId,
                        title: element.book.title,
                        author: element.book.author
                    }
                }));

                this.dataSource.data = borrowedBooks;
                this.isLoading = false;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                this.paginator.firstPage();
            },
            (error: any) => {
                this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
                this.isLoading = false;
            }
        );
    }

    /**
     * Opens a dialog to view the details of a borrowed book.
     * @param {string} enterAnimationDuration - Duration for the dialog's enter animation
     * @param {string} exitAnimationDuration - Duration for the dialog's exit animation
     * @param {BorrowedBook} book - The borrowed book object containing details
     * @returns {void}
     */
    openDialog(enterAnimationDuration: string, exitAnimationDuration: string, book: BorrowedBook): void {
        this.dialog.open(BorrowViewComponent, {
            enterAnimationDuration,
            exitAnimationDuration,
            data: book
        });
    }
}
