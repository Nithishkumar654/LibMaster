import { Component, OnInit } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { passwordValidator } from '../../shared/validators';
import { ApiService } from '../../services/api.service';
import { User } from '../../shared/interfaces';
import { Router } from '@angular/router';

/**
 * RegisterComponent handles user registration logic.
 * It includes role-based logic and communicates with the API for user registration.
 */
@Component({
    selector: 'app-register',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule],
    templateUrl: './register.component.html',
    styleUrl: './register.component.scss'
})
export class RegisterComponent implements OnInit {

    /** Registration form instance */
    registerForm!: FormGroup;

    /** Holds the selected role for additional logic (optional) */
    selectedRole: string = '';

    /**
     * Constructor initializes the required services.
     * @param {FormBuilder} _fb - FormBuilder for creating reactive forms.
     * @param {ApiService} _api - Service to handle API calls.
     * @param {Router} _router - Angular Router for navigation.
     */
    constructor(private _fb: FormBuilder, private _api: ApiService, private _router: Router) { }

    /**
     * Lifecycle hook: Initializes the form and redirects authenticated users to the home page.
     * @returns {void}
     */
    ngOnInit(): void {
        // Redirect logged-in users to the home page
        this._api.loggedIn.subscribe((status) => {
            if (status) {
                this._router.navigate(['/']);
            }
        });

        // Initialize the registration form with validation
        this.registerForm = this._fb.group({
            userName: ['', [Validators.required, Validators.minLength(6), Validators.pattern('^[A-Za-z0-9]+$')]],
            passwordHash: ['', [Validators.required, Validators.minLength(6), passwordValidator]],
            email: ['', [Validators.required, Validators.email]],
            role: ['', Validators.required],
        });
    }

    /**
     * Handles the form submission for user registration.
     * Communicates with the API to register a new user.
     * @returns {void}
     */
    goToRegister(): void {
        if (this.registerForm.valid) {
            const formData: User = {
                ...this.registerForm.value,
                isActive: this.registerForm.get('role')?.value === 'guest' // Set isActive for guest users
            };

            console.log('Form Data:', formData);

            // Call the API to register the user
            this._api.registerUser(formData).subscribe({
                next: (response) => {
                    console.log('Registration Response:', response.body);
                    this._api.openSnackBar('User Registration Success', 'success-color');
                    window.location.reload();
                },
                error: (error) => {
                    console.error('Registration Error:', error);
                    const errorMessage = error?.error?.errors?.Email?.[0] ?? 'An unknown error occurred';
                    this._api.openSnackBar(errorMessage, 'error-color');
                }
            });
        } else {
            this._api.openSnackBar('Please fill all required fields correctly.', 'error-color');
        }
    }

    /**
     * Updates the selected role for role-specific logic.
     * @param {string} role - The selected role value.
     * @returns {void}
     */
    onRoleChange(role: string): void {
        this.selectedRole = role;
    }

    /**
     * Redirects the user to Razorpay for membership fee payment.
     * Opens Razorpay's URL in a new browser tab.
     * @returns {void}
     */
    redirectToRazorpay(): void {
        window.open('https://fake.razorpay.com', '_blank');
    }
}
