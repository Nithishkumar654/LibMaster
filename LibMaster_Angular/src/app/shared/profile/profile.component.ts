import { Component, OnInit } from '@angular/core';
import { SharedModule } from '../shared.module';
import { ApiService } from '../../services/api.service';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { User } from '../interfaces';

/**
 * Profile component to display and manage user profile details.
 */
@Component({
    selector: 'app-profile',
    standalone: true,
    imports: [SharedModule, ReactiveFormsModule, FormsModule],
    templateUrl: './profile.component.html',
    styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {

    /** Form group to handle user profile data */
    userForm!: FormGroup;

    /** Boolean to check if the user is logged in */
    loggedIn: boolean = false;

    /** User object to store the profile data */
    user!: User;

    /** Boolean to disable form fields for editing */
    disabled: boolean = true;

    /**
     * Creates an instance of ProfileComponent.
     * @param dialogRef Reference to the dialog instance.
     * @param _api Service to handle API requests.
     * @param _fb Form builder to create reactive forms.
     */
    constructor(
        public dialogRef: MatDialogRef<ProfileComponent>,
        private _api: ApiService, private _fb: FormBuilder) { }

    /**
     * Initializes the component by subscribing to logged-in status
     * and fetching the user profile data.
     */
    ngOnInit(): void {
        this._api.loggedIn.subscribe(status => {
            this.loggedIn = status;
        });

        // Initialize the user form with default values
        this.userForm = this._fb.group({
            userName: ['', Validators.required],
            email: [{ value: '', disabled: true }, Validators.required],
            role: [{ value: '', disabled: true }, Validators.required],
            isActive: [{ value: false, disabled: true }, Validators.required]
        });

        // Fetch user profile from the API
        this._api.getUser().subscribe({
            next: (response) => {
                // Clean up the response before using it
                delete response.message.passwordHash;

                const user = response.message;
                this.user = user;

                // Populate the form with user data
                this.userForm = this._fb.group({
                    userName: this._fb.control(user.username, Validators.required),
                    email: this._fb.control({ value: user.email, disabled: true }, [Validators.required]),
                    role: this._fb.control({ value: user.role, disabled: true }, Validators.required),
                    isActive: this._fb.control({ value: user.isActive, disabled: true }, Validators.required)
                })
            },
            error: (error) => {
                this._api.openSnackBar(error.error.error ?? error.error.title, 'error-color');
            }
        });
    }

    /**
     * Saves the user profile data after validation.
     * Sends an API request to save the updated profile.
     */
    save(): void {
        const user: User = {
            userName: this.userForm.get('userName')?.value,
            passwordHash: 'pass', // Password handling should be done securely
            email: this.userForm.get('email')?.value,
            role: this.userForm.get('role')?.value,
            isActive: this.userForm.get('isActive')?.value
        };

        // Call the saveProfile API method to save the updated profile
        this._api.saveProfile(user).subscribe({
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
