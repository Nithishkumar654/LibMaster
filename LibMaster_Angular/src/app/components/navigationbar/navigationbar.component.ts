import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';

/**
 * NavigationbarComponent handles the application's navigation bar logic.
 * It displays links based on the user's authentication and role status.
 */
@Component({
    selector: 'app-navigationbar',
    standalone: true,
    imports: [SharedModule],
    templateUrl: './navigationbar.component.html',
    styleUrl: './navigationbar.component.scss'
})
export class NavigationbarComponent implements OnInit {
    /** Output event to communicate visibility changes to the parent component. */
    @Output() handleShow = new EventEmitter<void>();

    /** Input property to toggle a boolean state for visibility (e.g., mobile menu). */
    @Input() show!: boolean;

    /** User authentication and role states. */
    loggedIn: boolean = false;
    librarian: boolean = false;
    guest: boolean = false;
    member: boolean = false;

    /**
     * Constructor initializes required services.
     * @param {Router} _router - Angular Router for navigation.
     * @param {ApiService} _api - Service for handling API calls and user authentication.
     */
    constructor(private _router: Router, private _api: ApiService) { }

    /**
     * Lifecycle hook: Initializes the component, checks user authentication status,
     * and sets the user's role-based flags.
     * @returns {void}
     */
    ngOnInit(): void {
        // Subscribe to authentication status
        this._api.loggedIn.subscribe(status => {
            this.loggedIn = status;

            // If logged in, subscribe to role changes
            if (this.loggedIn) {
                this._api.role.subscribe(role => {
                    this.librarian = role === 'librarian';
                    this.guest = role === 'guest';
                    this.member = role === 'member';
                });
            }
        });
    }

    /**
     * Navigates to the registration page.
     * @returns {void}
     */
    goToRegister(): void {
        this._router.navigate(['/register']);
    }

    /**
     * Navigates to the login page.
     * @returns {void}
     */
    goToLogin(): void {
        this._router.navigate(['/login']);
    }

    /**
     * Logs out the user, clears the authentication state, and redirects to login.
     * @returns {void}
     */
    logout(): void {
        this._api.logoutUser(); // Clears token/session
        this._router.navigate(['/login']);
    }

    /**
     * Navigates to the management page (e.g., manage books/users).
     * @returns {void}
     */
    manage(): void {
        this._router.navigate(['/manage']);
    }

    /**
     * Navigates to the membership upgrade page.
     * @returns {void}
     */
    upgrade(): void {
        this._router.navigate(['/upgrade']);
    }
}
