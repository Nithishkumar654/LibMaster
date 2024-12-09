import { ChangeDetectionStrategy, Component, ViewEncapsulation } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { expiryDateValidator, passwordValidator } from '../../shared/validators';
import { TransactionDTO, User } from '../../shared/interfaces';
import * as _moment from 'moment';
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

/**
 * UpgradeComponent allows users to upgrade their membership and make payment.
 */
@Component({
    selector: 'app-upgrade',
    standalone: true,
    encapsulation: ViewEncapsulation.None,
    providers: [
        provideMomentDateAdapter(MY_FORMATS),
    ],
    imports: [SharedModule, ReactiveFormsModule],
    templateUrl: './upgrade.component.html',
    styleUrl: './upgrade.component.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class UpgradeComponent {

    /** Form group to handle payment details */
    paymentForm!: FormGroup;

    /**
     * Constructor for UpgradeComponent.
     * @param _fb The FormBuilder service to create reactive forms.
     * @param _api The ApiService to interact with backend APIs.
     * @param _router The Router service to navigate between views.
     */
    constructor(private _fb: FormBuilder, private _api: ApiService, private _router: Router) { }

    /**
     * Initializes the payment form with necessary validation rules.
     */
    ngOnInit(): void {
        this.paymentForm = this._fb.group({
            membershipType: ['', [Validators.required]], // Yearly or Monthly
            cardHolderName: ['', [Validators.required, Validators.pattern(/^[a-zA-Z\s]*$/)]], // Name validation
            cardNumber: ['', [Validators.required, Validators.pattern(/^\d{4}-\d{4}-\d{4}-\d{4}$/)]], // Card number format
            expiryDate: new FormControl(moment(), [expiryDateValidator()]), // Expiry Date
            cvv: ['', [Validators.required, Validators.pattern(/^\d{3}$/)]], // CVV 3-digit
            amount: ['', [Validators.required, Validators.pattern(/^\d+$/)]], // Valid amount in rupees
        });
    }

    /**
     * Sets the month and year for the expiry date field in the payment form.
     * @param normalizedMonthAndYear The normalized month and year object to set.
     * @param datepicker The MatDatepicker instance for the expiry date field.
     */
    setMonthAndYear(normalizedMonthAndYear: Moment, datepicker: MatDatepicker<Moment>) {
        const ctrlValue = this.paymentForm.value.expiryDate.value ?? moment();
        ctrlValue.month(normalizedMonthAndYear.month());
        ctrlValue.year(normalizedMonthAndYear.year());
        this.paymentForm.value.expiryDate.setValue(ctrlValue);
        datepicker.close();
    }

    /**
     * Navigates to the registration page if the payment form is valid.
     */
    goToRegister(): void {
        if (this.paymentForm.valid) {
            const formData: User = {
                ...this.paymentForm.value,
                isActive: this.paymentForm.get('role')?.value === 'guest'
            };

            this._api.registerUser(formData).subscribe({
                next: (response) => {
                    this._api.openSnackBar('User Registration Success', 'success-color');
                    window.location.reload();
                },
                error: (error) => {
                    error = error?.error?.errors?.Email[0] ?? 'An unknown error occurred';
                    this._api.openSnackBar(error, 'error-color');
                }
            });
        } else {
            this._api.openSnackBar('Please fill all required fields', 'error-color');
        }
    }

    /** Holds the selected user role */
    selectedRole: string = '';

    /**
     * Handles the role change in the form.
     * @param role The selected role.
     */
    onRoleChange(role: string): void {
        this.selectedRole = role;
    }

    /**
     * Redirects the user to the Razorpay payment page.
     */
    redirectToRazorpay(): void {
        window.open('https://fake.razorpay.com', '_blank');
    }

    /**
     * Submits the payment information to the API to process the payment.
     */
    submitPayment() {
        const transaction: TransactionDTO = {
            transactionType: 'MembershipFee',
            amount: (+this.paymentForm.value.amount),
            plan: this.paymentForm.value.membershipType,
            details: `Membership payment for ${this.paymentForm.value.membershipType} plan`,
            userId: 0
        };

        this._api.makePayment(transaction).subscribe({
            next: (response) => {
                this._api.openSnackBar(response.message, 'success-color');
            },
            error: (error) => {
                this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
            }
        });
    }

}
