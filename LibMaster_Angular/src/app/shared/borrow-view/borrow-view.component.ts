import { ChangeDetectionStrategy, Component, Inject, OnInit, ViewEncapsulation } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { BorrowedBook, TransactionDTO } from '../interfaces';
import { ApiService } from '../../services/api.service';
import { FormBuilder, FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { SharedModule } from '../shared.module';
import * as _moment from 'moment';
import { expiryDateValidator } from '../validators';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import { default as _rollupMoment, Moment } from 'moment';
import { MatDatepicker } from '@angular/material/datepicker';

const moment = _rollupMoment || _moment;

export const MY_FORMATS = {
    parse: {
        dateInput: 'MM/YYYY',
    },
    display: {
        dateInput: 'MM/YYYY',
        monthYearLabel: 'MMM YYYY',
        dateA11yLabel: 'LL',
        monthYearA11yLabel: 'MMMM YYYY',
    },
};

@Component({
    selector: 'app-borrow-view',
    standalone: true,
    encapsulation: ViewEncapsulation.None,
    providers: [
        provideMomentDateAdapter(MY_FORMATS),
    ],
    imports: [SharedModule, ReactiveFormsModule, FormsModule],
    templateUrl: './borrow-view.component.html',
    styleUrl: './borrow-view.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class BorrowViewComponent implements OnInit {

    loggedIn: boolean = false;
    member: boolean = false;
    paymentForm!: FormGroup;
    bookForm!: FormGroup;
    categories = ['Fiction', 'Non-Fiction', 'Science', 'History', 'Technology', 'Children'];

    /**
     * Constructor for BorrowViewComponent
     * @param dialogRef - Reference to the dialog for closing
     * @param data - Data passed to the dialog
     * @param _api - ApiService to interact with backend
     * @param _fb - FormBuilder to create forms
     */
    constructor(
        public dialogRef: MatDialogRef<BorrowViewComponent>,
        @Inject(MAT_DIALOG_DATA) public data: BorrowedBook,
        private _api: ApiService,
        private _fb: FormBuilder
    ) { }

    /**
     * OnInit lifecycle hook to initialize the component state
     */
    ngOnInit(): void {
        this._api.loggedIn.subscribe(status => {
            this.loggedIn = status;
        });
        if (this.loggedIn) {
            this._api.role.subscribe(r => {
                this.member = (r == 'member');
            })
        }

        this.bookForm = this._fb.group({
            bookId: [this.data.bookId, Validators.required],
            memberId: [this.data.memberId, Validators.required],
            borrowDate: [this.data.borrowDate, Validators.required],
            dueDate: [this.data.dueDate, Validators.required],
            returnDate: [this.data.returnDate, Validators.required],
            lateFee: [this.data.lateFee, Validators.required],
            status: [this.data.status, Validators.required],
            book: this._fb.group({
                bookId: [this.data.book.bookId, Validators.required],
                title: [this.data.book.title, Validators.required],
                author: [this.data.book.author, Validators.required],
            })
        });

        this.paymentForm = this._fb.group({
            cardHolderName: ['', [Validators.required, Validators.pattern(/^[a-zA-Z\s]*$/)]],
            cardNumber: ['', [Validators.required, Validators.pattern(/^\d{4}-\d{4}-\d{4}-\d{4}$/)]],
            expiryDate: new FormControl(moment(), [expiryDateValidator()]),
            cvv: ['', [Validators.required, Validators.pattern(/^\d{3}$/)]],
        });
    }

    /**
     * Sets the month and year for the expiry date in the payment form.
     * @param normalizedMonthAndYear - Selected month and year
     * @param datepicker - Reference to the datepicker
     */
    setMonthAndYear(normalizedMonthAndYear: Moment, datepicker: MatDatepicker<Moment>): void {
        const ctrlValue = this.paymentForm.value.expiryDate.value ?? moment();
        ctrlValue.month(normalizedMonthAndYear.month());
        ctrlValue.year(normalizedMonthAndYear.year());
        this.paymentForm.value.expiryDate.setValue(ctrlValue);
        datepicker.close();
    }

    /**
     * Calculates the late fee based on the due date and the current date.
     * @returns The calculated late fee
     */
    calculateLateFee(): number {
        const today = new Date();
        const dueDate = new Date(this.bookForm.value.dueDate);

        if (today > dueDate) {
            const timeDifference = today.getTime() - dueDate.getTime();
            const daysLate = Math.ceil(timeDifference / (1000 * 3600 * 24));

            return daysLate * 50;
        }

        return 0;
    }

    /**
     * Initiates the book return process and closes the dialog on success.
     */
    return(): void {
        this._api.returnBook(this.data.bookId.toString()).subscribe(
            (response) => {
                this._api.openSnackBar(response.body.message, 'success-color');
            },
            (error) => {
                this._api.openSnackBar('Error: ' + (error.error.error ?? error.error.title ?? ' Only Members can Borrow or Reserve Books'), 'error-color')
            }
        );
        this.dialogRef.close();
    }

    showForm: boolean = false;

    /**
     * Shows the form to pay the fee.
     */
    show(): void {
        this.showForm = true;
    }

    /**
     * Initiates the payment for the late fee and closes the dialog on success.
     */
    payFee(): void {
        const transaction: TransactionDTO = {
            userId: 0,
            transactionType: 'LateFee',
            amount: this.paymentForm.value.amount,
            details: `Late fee Payment for the ${this.bookForm.value.book.title}`,
            plan: ''
        }
        this._api.makePayment(transaction).subscribe({
            next: (response) => {
                this.return();
                this._api.openSnackBar(response.message, 'success-color');
            },
            error: (error) => {
                this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
            }
        });
        this.dialogRef.close();
    }
}
