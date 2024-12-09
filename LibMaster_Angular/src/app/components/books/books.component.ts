import { AfterViewInit, Component, inject, OnInit, ViewChild } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MatTableDataSource } from '@angular/material/table';
import { Book } from '../../shared/interfaces';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ApiService } from '../../services/api.service';
import { DialogComponent } from '../../shared/dialog/dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';

/**
 * Component for displaying and managing a list of books.
 */
@Component({
    selector: 'app-books',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule],
    templateUrl: './books.component.html',
    styleUrl: './books.component.scss'
})
export class BooksComponent implements OnInit, AfterViewInit {
    /** Form group for filtering books */
    filterForm!: FormGroup;

    /** Reference to MatDialog for opening modals */
    readonly dialog = inject(MatDialog);

    /** Columns displayed in the table */
    displayedColumns: string[] = ['title', 'author', 'genre', 'availableCopies', 'options'];

    /** Data source for the MatTable */
    dataSource = new MatTableDataSource<Book>();

    /** Reference to the MatPaginator for pagination */
    @ViewChild(MatPaginator) paginator!: MatPaginator;

    /** Reference to the MatSort for sorting */
    @ViewChild(MatSort) sort!: MatSort;

    /** Loading state indicator */
    isLoading: boolean = false;

    /** User login status */
    loggedIn: boolean = false;

    /** Admin role flag */
    admin: boolean = false;

    /** Librarian role flag */
    librarian: boolean = false;

    /**
     * Constructor to inject services.
     * @param _api - The API service for interacting with backend endpoints
     * @param _fb - The FormBuilder service to create reactive forms
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
     * Initializes form, user roles, and loads initial book data.
     * @returns {void}
     */
    ngOnInit(): void {
        this.filterForm = this._fb.group({
            category: this._fb.control('category'),
            search: this._fb.control('')
        });

        this._api.loggedIn.subscribe((status: boolean) => {
            this.loggedIn = status;
        });

        if (this.loggedIn) {
            this._api.role.subscribe((r: string) => {
                this.admin = (r === 'admin');
                this.librarian = (r === 'librarian');
            });
        }

        this.callFilter();
    }

    /**
     * Triggers the filtering process by calling the `filter` method.
     * @returns {void}
     */
    callFilter(): void {
        this.filter();
    }

    /**
     * Filters and fetches books based on the search term and category.
     * Updates the data source with filtered results.
     * @returns {void}
     */
    filter(): void {
        const search: string = this.filterForm.get('search')?.value?.trim() || null;
        const category: string =
            this.filterForm.get('category')?.value === 'category'
                ? null
                : this.filterForm.get('category')?.value;

        this.isLoading = true;

        this._api.getBooks(search, category).subscribe(
            (books: { message: Book[] }) => {
                this.dataSource.data = books.message;
                this.isLoading = false;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                this.paginator.firstPage();
            },
            (error: any) => {
                this._api.openSnackBar('Error: ' + error.error.error, 'error-color');
                this.isLoading = false;
            }
        );
    }

    /**
     * Opens a dialog for viewing or editing a specific book.
     * @param {string} enterAnimationDuration - Duration for the dialog's enter animation
     * @param {string} exitAnimationDuration - Duration for the dialog's exit animation
     * @param {Book} book - The book object containing book details
     * @returns {void}
     */
    openDialog(enterAnimationDuration: string, exitAnimationDuration: string, book: Book): void {
        this.dialog.open(DialogComponent, {
            enterAnimationDuration,
            exitAnimationDuration,
            data: book
        });
    }
}
