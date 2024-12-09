import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { SharedModule } from './shared/shared.module';
import { NavigationbarComponent } from './components/navigationbar/navigationbar.component';
import { FooterComponent } from "./components/footer/footer.component";
import { ApiService } from './services/api.service';
import { MatDialog } from '@angular/material/dialog';
import { ProfileComponent } from './shared/profile/profile.component';
@Component({
    selector: 'app-root',
    standalone: true,
    imports: [RouterOutlet, SharedModule, NavigationbarComponent, FooterComponent, RouterLink],
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss'
})
export class AppComponent {
    title = 'LibraryManagementSystem';
    show: boolean = true;

    constructor(private _router: Router, private _api: ApiService) { }

    loggedIn: boolean = false;
    admin: boolean = false;
    librarian: boolean = false;
    member: boolean = false;
    guest: boolean = false;

    readonly dialog = inject(MatDialog);


    ngOnInit(): void {
        this._api.loggedIn.subscribe(status => {
            this.loggedIn = status;
        });
        if (this.loggedIn) {
            this._api.role.subscribe(r => {
                this.admin = (r == 'admin');
                this.member = (r == 'member');
                this.librarian = (r == 'librarian');
                this.guest = (r == 'guest');

            })
        }
    }

    handleShow(): void {
        this.show = !this.show;
    }

    goToRegister(): void {
        this._router.navigate(['/register']);
    }

    goToLogin(): void {
        this._router.navigate(['/login']);
    }

    logout(): void {
        this._api.logoutUser();
        this.goToLogin();
    }

    goToRequests(): void {
        this._router.navigate(['/requests']);
    }

    manage(): void {
        this._router.navigate(['/manage']);
    }

    profile(): void {
        this.dialog.open(ProfileComponent);
    }
}
