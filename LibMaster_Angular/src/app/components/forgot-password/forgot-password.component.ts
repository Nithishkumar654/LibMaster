import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { UserDTO } from '../../shared/interfaces';
import { SharedModule } from '../../shared/shared.module';
import { passwordValidator } from '../../shared/validators';

/**
 * Component for handling forgot password functionality.
 * Allows users to reset their passwords by requesting an OTP and providing a new password.
 */
@Component({
    selector: 'app-forgot-password',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule, FormsModule],
    templateUrl: './forgot-password.component.html',
    styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent implements OnInit {

    /** Reactive form group for login/reset password form */
    loginForm!: FormGroup;

    /** OTP sent state */
    otp: boolean = false;

    /** Stores the entered OTP */
    otpS: string = '';

    /**
     * Initializes the ForgotPasswordComponent with required services.
     * @param {Router} _router - Service to navigate between routes.
     * @param {ApiService} _api - Service for API interactions.
     * @param {FormBuilder} _fb - FormBuilder to create reactive forms.
     */
    constructor(private _router: Router, private _api: ApiService, private _fb: FormBuilder) { }

    /**
     * Navigates to the registration page.
     * @returns {void}
     */
    goToRegister(): void {
        this._router.navigate(['/register']);
    }

    /**
     * Lifecycle hook: Initializes the component and reactive form.
     * Checks if the user is already logged in and redirects if necessary.
     * @returns {void}
     */
    ngOnInit(): void {
        this._api.loggedIn.subscribe((status: boolean) => {
            if (status) {
                this._router.navigate(['/']);
            }
        });

        this.loginForm = this._fb.group({
            email: this._fb.control('', [Validators.required, Validators.email]),
            passwordHash: this._fb.control('', [Validators.required, Validators.minLength(6), passwordValidator]),
        });
    }

    /**
     * Handles user login and processes the form submission.
     * On successful login, the user is redirected to the home page.
     * @returns {void}
     */
    login(): void {
        console.log(this.loginForm.value);

        if (this.loginForm.valid) {
            const formData: UserDTO = { ...this.loginForm.value };

            this._api.loginUser(formData).subscribe({
                next: (response) => {
                    console.log(response);
                    if (response && response.body) {
                        localStorage.setItem('Token', response.body.message ?? '');
                        this._api.changeToTrue();
                        window.location.reload();
                        this._api.openSnackBar('Login Successful', 'success-color');
                        this._router.navigate(['/']);
                    } else {
                        this._api.openSnackBar('Unexpected response format', 'error-color');
                    }
                },
                error: (error) => {
                    console.log(error);
                    if (error.error) {
                        this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
                    } else {
                        this._api.openSnackBar('Something went wrong. Please try again later.', 'error-color');
                    }
                }
            });
        } else {
            this._api.openSnackBar('Please fill in the required fields correctly.', 'error-color');
        }
    }

    /**
     * Sends an OTP to the user's email for password reset verification.
     * Updates the OTP sent state to true.
     * @returns {void}
     */
    sendOtp(): void {
        if (!this.loginForm.valid) return;

        this.otp = true;
        this._api.sendOtp(this.loginForm.value.email).subscribe({
            next: (response: { message: string }) => {
                console.log(response);
                localStorage.setItem('otp', response.message);
                this._api.openSnackBar('OTP sent successfully!', 'success-color');
            },
            error: (error) => {
                console.log(error);
                this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
            }
        });
    }

    /**
     * Resets the user's password using the provided OTP, email, and new password.
     * On success, redirects the user to the login page.
     * @returns {void}
     */
    resetPass(): void {
        this._api.resetPass(this.loginForm.value.email, this.loginForm.value.passwordHash, this.otpS).subscribe({
            next: (response: { message: string }) => {
                console.log(response);
                localStorage.removeItem('otp');
                this._api.openSnackBar(response.message, 'success-color');
                this._router.navigate(['/login']);
            },
            error: (error) => {
                console.log(error);
                this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
            }
        });
    }
}
