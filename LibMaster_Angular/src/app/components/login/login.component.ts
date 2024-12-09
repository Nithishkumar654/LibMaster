import { Component, OnInit } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserDTO } from '../../shared/interfaces';

/**
 * LoginComponent handles user login functionality. 
 * It provides a form for user authentication and redirects upon successful login.
 */
@Component({
    selector: 'app-login',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule, RouterLink],
    templateUrl: './login.component.html',
    styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {

    /** FormGroup for the login form. */
    loginForm!: FormGroup;

    /**
     * Initializes the LoginComponent with required services.
     * @param {Router} _router - Angular Router service for navigation.
     * @param {ApiService} _api - Service for API calls and user authentication.
     * @param {FormBuilder} _fb - FormBuilder for reactive form management.
     */
    constructor(
        private _router: Router,
        private _api: ApiService,
        private _fb: FormBuilder
    ) { }

    /**
     * Lifecycle hook: Initializes the component.
     * @returns {void}
     */
    ngOnInit(): void {
        // Check if the user is already logged in
        this._api.loggedIn.subscribe((status: boolean) => {
            if (status) {
                this._router.navigate(['/']);
            }
        });

        // Initialize login form with validation rules
        this.loginForm = this._fb.group({
            email: this._fb.control('', [Validators.required, Validators.email]),
            password: this._fb.control('', [Validators.required])
        });
    }

    /**
     * Redirects the user to the registration page.
     * @returns {void}
     */
    goToRegister(): void {
        this._router.navigate(['/register']);
    }

    /**
     * Handles the login process by validating the form and making an API request.
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
}
