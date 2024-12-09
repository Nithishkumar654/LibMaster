import { Component, Inject, OnInit } from '@angular/core';
import { SharedModule } from '../shared.module';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Book } from '../interfaces';
import { ApiService } from '../../services/api.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

/**
 * Dialog component for book management operations such as borrowing, reserving, and updating book information.
 */
@Component({
    selector: 'app-dialog',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule, FormsModule],
    templateUrl: './dialog.component.html',
    styleUrl: './dialog.component.scss',
})
export class DialogComponent implements OnInit {

    /** Number of copies to be added */
    addCopies: number = 0;

    /** Status of user login */
    loggedIn: boolean = false;

    /** Boolean to check if user is admin */
    admin: boolean = false;

    /** Boolean to check if user is librarian */
    librarian: boolean = false;

    /** Form group for managing book information */
    bookForm!: FormGroup;

    /**
     * Creates an instance of DialogComponent.
     * @param dialogRef Reference to the dialog instance.
     * @param data Data passed to the dialog (book details).
     * @param _api Service to handle API requests.
     * @param _fb Form builder to create reactive forms.
     */
    constructor(
        public dialogRef: MatDialogRef<DialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: Book,
        private _api: ApiService,
        private _fb: FormBuilder
    ) { }

    /**
     * Initializes the component and sets up form controls and user roles.
     */
    ngOnInit(): void {
        this._api.loggedIn.subscribe(status => {
            this.loggedIn = status;
        });
        if (this.loggedIn) {
            this._api.role.subscribe(r => {
                this.admin = (r == 'admin');
                this.librarian = (r == 'librarian');
            })
        }

        // Initialize the form group with book details
        this.bookForm = this._fb.group({
            bookId: [this.data.bookId, Validators.required],
            title: [this.data.title, Validators.required],
            author: [this.data.author, Validators.required],
            genre: [this.data.genre, Validators.required],
            isbn: [this.data.isbn, Validators.required],
            publicationDate: [this.data.publicationDate, Validators.required],
            availableCopies: [this.data.availableCopies, Validators.required],
            status: [this.data.status, Validators.required],
            categoryId: [this.data.categoryId, Validators.required],
            category: this._fb.group({
                categoryId: [this.data.category.categoryId, Validators.required],
                categoryName: [this.data.category.categoryName, Validators.required],
                description: [this.data.category.description, Validators.required],
            }),
            inventory: this._fb.group({
                inventoryId: [this.data.inventory.inventoryId, Validators.required],
                bookId: [this.data.inventory.bookId, Validators.required],
                quantity: [this.data.inventory.quantity, Validators.required],
                condition: [this.data.inventory.condition, Validators.required],
                location: [this.data.inventory.location, Validators.required]
            })
        })
    }

    /**
     * Handles the borrow operation for a book.
     * Sends a request to borrow the book and shows the response in a snackbar.
     */
    borrow(): void {
        this._api.borrowBook(this.data.bookId.toString()).subscribe(
            (response) => {
                this._api.openSnackBar(response.body.message, 'success-color');
            },
            (error) => {
                this._api.openSnackBar('Error: ' + (error.error.error ?? error.error.title ?? ' Only Members can Borrow or Reserve Books'), 'error-color')
            }
        )
        this.dialogRef.close();
    }

    /**
     * Handles the reserve operation for a book.
     * Sends a request to reserve the book and shows the response in a snackbar.
     */
    reserve(): void {
        this._api.reserveBook(this.data.bookId.toString()).subscribe(
            (response) => {
                this._api.openSnackBar(response.body.message, 'success-color');
            },
            (error) => {
                this._api.openSnackBar('Error: ' + (error.error.error ?? error.error.title ?? ' Only Members can Borrow or Reserve Books'), 'error-color')
            }
        )
        this.dialogRef.close();
    }

    bookImages: string[] = [
        'https://picsum.photos/200/300?random=1',
        'https://picsum.photos/200/300?random=2',
        'https://picsum.photos/200/300?random=3',
        'https://picsum.photos/200/300?random=4',
        'https://picsum.photos/200/300?random=5'
    ];

    // Method to get a random book image URL
    getRandomImage(): string {
        const randomIndex = Math.floor(Math.random() * this.bookImages.length);
        return this.bookImages[randomIndex];
    }

    /**
     * Handles saving the updated number of copies of a book.
     * Checks if the number of copies is valid and sends a request to update the book.
     */
    save(): void {
        if (this.addCopies < 0) {
            this._api.openSnackBar('Copies cannot be negative', 'error-color');
            return;
        }

        this._api.updateBook(this.bookForm.value.bookId, this.addCopies).subscribe({
            next: (response) => {
                this._api.openSnackBar(response.message, 'success-color');
            },
            error: (error) => {
                this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
            }
        });

        this.dialogRef.close();
    }
}
