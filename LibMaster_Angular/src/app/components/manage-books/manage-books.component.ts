import { Component, OnInit } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { BookDTO } from '../../shared/interfaces';

/**
 * ManageBooksComponent handles the creation and management of book details.
 * It allows the user to add books and update inventory.
 */
@Component({
    selector: 'app-manage-books',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule],
    templateUrl: './manage-books.component.html',
    styleUrl: './manage-books.component.scss'
})
export class ManageBooksComponent implements OnInit {

    /** Form group for adding a new book. */
    bookForm!: FormGroup;

    /** Form group for managing book inventory. */
    inventoryForm!: FormGroup;

    /**
     * Initializes the ManageBooksComponent with required services.
     * @param {Router} _router - Angular Router for navigation.
     * @param {ApiService} _api - Service for API calls.
     * @param {FormBuilder} _fb - FormBuilder for creating reactive forms.
     */
    constructor(
        private _router: Router,
        private _api: ApiService,
        private _fb: FormBuilder
    ) { }

    /**
     * Lifecycle hook: Initializes the component and form groups.
     * @returns {void}
     */
    ngOnInit(): void {
        this.initializeBookForm();
        this.initializeInventoryForm();
    }

    /**
     * Initializes the form for adding a new book.
     * @returns {void}
     */
    private initializeBookForm(): void {
        this.bookForm = this._fb.group({
            title: ['Sample Title', Validators.required],
            author: ['Sample Author', Validators.required],
            genre: ['Sample Genre', Validators.required],
            isbn: [
                '3213213213213',
                [Validators.required, Validators.pattern('^[0-9]{10}$')]
            ],
            publicationDate: [new Date(), Validators.required],
            availableCopies: [6, [Validators.required, Validators.min(0)]],
            status: ['Available', Validators.required],
            categoryId: ['3', Validators.required],
        });
    }

    /**
     * Initializes the form for managing book inventory.
     * @returns {void}
     */
    private initializeInventoryForm(): void {
        this.inventoryForm = this._fb.group({
            bookId: [0, Validators.required],
            quantity: [1, [Validators.required, Validators.min(1)]],
            condition: ['Good', Validators.required],
            location: ['Shelf G1', Validators.required],
        });
    }

    /**
     * Navigates the user back to the books page.
     * @returns {void}
     */
    update(): void {
        this._router.navigate(['/books']);
    }

    /**
     * Adds a new book using the form data and calls the API.
     * @returns {void}
     */
    add(): void {
        if (this.bookForm.valid) {
            const formData = this.bookForm.value;
            formData.categoryId = +formData.categoryId; // Ensure categoryId is a number

            const book: BookDTO = { ...formData };

            console.log('Adding Book:', book);

            this._api.addBook(book).subscribe({
                next: (response) => {
                    console.log('Book added successfully:', response);
                    this._api.openSnackBar('Book added successfully!', 'success-color');
                },
                error: (error) => {
                    console.error('Error adding book:', error);
                    this._api.openSnackBar(
                        error.error?.error ?? error.error?.title ?? 'Failed to add book',
                        'error-color'
                    );
                }
            });
        } else {
            this._api.openSnackBar('Please fill in all required fields correctly.', 'error-color');
        }
    }
}
