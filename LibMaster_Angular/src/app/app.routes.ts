import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { BooksComponent } from './components/books/books.component';
import { RequestsComponent } from './components/requests/requests.component';
import { ManageBooksComponent } from './components/manage-books/manage-books.component';
import { UpgradeComponent } from './components/upgrade/upgrade.component';
import { BorrowedBooksComponent } from './components/borrowed-books/borrowed-books.component';
import { ReservedBooksComponent } from './components/reserved-books/reserved-books.component';
import { TransactionHistoryComponent } from './components/transaction-history/transaction-history.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'register',
        component: RegisterComponent
    },
    {
        path: 'books',
        component: BooksComponent
    },
    {
        path: 'requests',
        component: RequestsComponent
    },
    {
        path: 'manage',
        component: ManageBooksComponent
    },
    {
        path: 'upgrade',
        component: UpgradeComponent
    },
    {
        path: 'borrowed',
        component: BorrowedBooksComponent
    },
    {
        path: 'reserved',
        component: ReservedBooksComponent
    },
    {
        path: 'history',
        component: TransactionHistoryComponent
    },
    {
        path: 'forgot',
        component: ForgotPasswordComponent
    }
];
