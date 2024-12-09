import { Component, OnInit, ViewChild } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { User } from '../../shared/interfaces';
import { SharedModule } from '../../shared/shared.module';
import { Router } from '@angular/router';

/**
 * Requests component to display and manage user requests for librarian roles.
 */
@Component({
    selector: 'app-requests',
    standalone: true,
    imports: [SharedModule],
    templateUrl: './requests.component.html',
    styleUrl: './requests.component.scss'
})
export class RequestsComponent implements OnInit {

    displayedColumns: string[] = ['sno', 'librarianName', 'email', 'options'];
    dataSource = new MatTableDataSource<User>();
    role: string = '';
    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    /**
     * @param _api The ApiService to manage API calls for user requests.
     * @param _router The router service to navigate between pages.
     */
    constructor(private _api: ApiService, private _router: Router) { }

    /**
     * Initializes the component and fetches user requests.
     */
    ngOnInit(): void {
        this.getData();
    }

    /**
     * Fetches the list of user requests for librarian roles.
     */
    getData(): void {
        this._api.requests().subscribe(
            (users) => {
                this.dataSource.data = users.message;
                this.dataSource.paginator = this.paginator;
                this.dataSource.sort = this.sort;
                this.paginator.firstPage();
            },
            (error) => {
                this._api.openSnackBar('Error: ' + error.error.error, 'error-color');
            }
        );
    }

    /**
     * Accepts a user request for the librarian role.
     * @param id The ID of the user request to accept.
     */
    accept(id: number): void {
        this._api.acceptLibrarian(id.toString()).subscribe({
            next: (response) => {
                this._api.openSnackBar(response.message, 'success-color');
                this.getData();
            },
            error: (error: any) => {
                this._api.openSnackBar(error.error.error, 'error-color');
            }
        });
    }
}
