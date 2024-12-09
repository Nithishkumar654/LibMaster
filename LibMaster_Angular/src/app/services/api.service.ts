import { inject, Injectable } from '@angular/core';
import { Book, BookDTO, TransactionDTO, User, UserDTO } from '../shared/interfaces';
import { HttpClient, HttpHeaders, HttpParams, HttpResponse } from '@angular/common/http';
import { BehaviorSubject, catchError, map, Observable, of } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackbarComponent } from '../shared/snackbar/snackbar.component';

@Injectable({
    providedIn: 'root'
})
export class ApiService {

    url: string = 'https://localhost:7128/api';

    private loggedInSubject: BehaviorSubject<boolean>;
    public loggedIn: Observable<boolean>;

    private roleSubject: BehaviorSubject<string>;
    public role: Observable<string>;

    constructor(private _http: HttpClient, private snackBar: MatSnackBar) {
        const token = localStorage.getItem('Token');
        this.loggedInSubject = new BehaviorSubject<boolean>(!!token);
        this.loggedIn = this.loggedInSubject.asObservable();
        this.roleSubject = new BehaviorSubject<string>('');
        this.role = this.roleSubject.asObservable();
        this.checkAuthorization();
    }

    private checkAuthorization(): void {
        const token = localStorage.getItem('Token');
        if (token) {
            this.authorized().subscribe(isAuthorized => {
                if (isAuthorized) {
                    this.loggedInSubject.next(true);
                } else {
                    this.loggedInSubject.next(false);
                }
            });
        } else {
            this.loggedInSubject.next(false);
        }
    }

    getBooks(search?: string, category?: string): Observable<any> {

        let params = new HttpParams();

        let apiUrl = this.url;
        apiUrl += "/Books";
        if (search != null) {
            params = params.set('search', search);
            apiUrl += `?search=${search}`;
            if (category) {
                apiUrl += `&category=${category}`
            }
        }
        else if (category != null) {
            apiUrl += `?category=${category}`;
            params = params.set('category', category);
        }

        return this._http.get<any>(apiUrl);
    }

    getBorrowedBooks(): Observable<any> {
        const apiUrl = `${this.url}/Books/borrow`;
        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.get<any>(apiUrl, { headers });
    }

    getReservedBooks(): Observable<any> {
        const apiUrl = `${this.url}/Books/reserved`;
        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.get<any>(apiUrl, { headers });
    }


    getTransactions(): Observable<any> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${localStorage.getItem('Token')}`,
        });

        return this._http.get<any>(`${this.url}/Users/transactions`, { headers });
    }

    borrowBook(bookId: string): Observable<any> {
        const apiUrl = `${this.url}/Books/borrow/${bookId}`;
        const token = localStorage.getItem('Token');
        console.log(apiUrl)
        console.log(token);
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.put<any>(apiUrl, null, { headers, observe: 'response' });
    }

    returnBook(bookId: string): Observable<any> {
        const apiUrl = `${this.url}/Books/return/${bookId}`;
        const token = localStorage.getItem('Token');
        console.log(apiUrl)
        console.log(token);
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.put<any>(apiUrl, null, { headers, observe: 'response' });
    }

    reserveBook(bookId: string): Observable<any> {
        const apiUrl = `${this.url}/Books/reserve/${bookId}`;
        const token = localStorage.getItem('Token');
        console.log(apiUrl)
        console.log(token);
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.put<any>(apiUrl, null, { headers, observe: 'response' });
    }

    withdrawReservation(bookId: string): Observable<any> {
        const apiUrl = `${this.url}/Books/withdraw/${bookId}`;
        const token = localStorage.getItem('Token');
        console.log(apiUrl)
        console.log(token);
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.put<any>(apiUrl, null, { headers, observe: 'response' });
    }

    updateBook(bookId: string, copies: number): Observable<any> {
        const apiUrl = `${this.url}/Books/${bookId}?copies=${copies}`;

        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.put<any>(apiUrl, null, { headers });
    }

    private _snackBar = inject(MatSnackBar);
    openSnackBar(message: string, color: string) {
        this._snackBar.openFromComponent(SnackbarComponent, {
            data: {
                message: message,
                color: color
            }
        })
    }


    registerUser(user: User): Observable<HttpResponse<User>> {
        const apiUrl = `${this.url}/Users/register`;
        const headers = new HttpHeaders({
            'Content-Type': 'application/json',
        });

        return this._http.post<User>(apiUrl, user, {
            headers,
            observe: 'response',
        });
    }

    loginUser(user: UserDTO): Observable<any> {
        const apiUrl = `${this.url}/Users/login`;
        const headers = new HttpHeaders({
            'Content-Type': 'application/json',
        });
        return this._http.post<any>(apiUrl, user, {
            headers,
            observe: 'response',
        });
    }

    getUser(): Observable<any> {
        const apiUrl = `${this.url}/Users`;
        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.get<any>(apiUrl, { headers });
    }

    logoutUser(): void {
        localStorage.removeItem('Token');
        this.loggedInSubject.next(false);
    }

    saveProfile(user: User): Observable<any> {
        const apiUrl = `${this.url}/Users`;
        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.put<any>(apiUrl, user, { headers });
    }

    sendOtp(email: string): Observable<any> {
        const apiUrl = `${this.url}/Users/sendotp?email=${email}`;
        return this._http.get<any>(apiUrl);
    }

    resetPass(email: string, password: string, otp: string): Observable<any> {
        const apiUrl = `${this.url}/Users/resetpass`;

        const dto = {
            email: email,
            password: password
        };
        console.log(dto);
        const token = localStorage.getItem('otp');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`,
            'Otp': `Otp ${otp}`
        })

        return this._http.post<any>(apiUrl, dto, { headers });
    }

    changeToTrue(): void {
        this.loggedInSubject.next(true);
    }


    authorized(): Observable<boolean> {
        const token = localStorage.getItem('Token');
        const apiUrl = `${this.url}/Users/authorized`;
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.post<any>(apiUrl, null, { headers, observe: 'response', responseType: 'text' as 'json' }).pipe(
            map(response => {
                console.log(response)
                this.roleSubject.next(response.body);
                return response.status == 200;
            }),
            catchError((error) => {
                return of(false);
            })
        )
    }

    requests(): Observable<any> {
        const apiUrl = `${this.url}/Users/requests`;
        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.get<User[]>(apiUrl, { headers });
    }


    acceptLibrarian(id: string): Observable<any> {
        const apiUrl = `${this.url}/Users/AcceptLibrarian/${id}`;

        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        })
        return this._http.post<any>(apiUrl, null, { headers });
    }


    makePayment(transaction: TransactionDTO): Observable<any> {
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${localStorage.getItem('Token')}`,
        });

        return this._http.post(`${this.url}/Users/payment`, transaction, { headers });
    }


    addBook(book: BookDTO): Observable<any> {
        const apiUrl = `${this.url}/Books`;

        const token = localStorage.getItem('Token');
        const headers = new HttpHeaders({
            'Authorization': `Bearer ${token}`
        });

        return this._http.post<any>(apiUrl, book, { headers });
    }
}
