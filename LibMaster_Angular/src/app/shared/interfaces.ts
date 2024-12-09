export interface Book {
    bookId: number;
    title: string;
    author: string;
    genre: string;
    isbn: string;
    publicationDate: Date;
    availableCopies: number;
    status: string;
    categoryId: number;
    category: {
        categoryId: number;
        categoryName: string;
        description: string;
    };
    inventory: Inventory;
}

export interface BookDTO {
    bookId: number;
    title: string;
    author: string;
    genre: string;
    isbn: string;
    publicationDate: Date;
    availableCopies: number;
    status: string;
    categoryId: number;
}

export interface BorrowedBook {
    bookId: number;
    memberId: number;
    borrowDate: Date;
    dueDate: Date;
    returnDate: Date;
    lateFee: number;
    status: string;
    book: {
        bookId: number;
        title: string;
        author: string;
    }
}

export interface Reservation {
    reservationId: number,
    bookId: number;
    reservationDate: Date;
    status: string;
    book: {
        bookId: number;
        title: string;
        author: string;
    }
}

export interface Inventory {
    inventoryId: number;
    bookId: number;
    quantity: number;
    condition: string;
    location: string;
}

export interface User {
    userName: string;
    passwordHash: string;
    email: string;
    role: string;
    isActive: boolean;
}

export interface UserDTO {
    email: string;
    password: string;
}

export interface TransactionDTO {
    userId: number;
    transactionType: string;
    amount: number;
    plan: string;
    details: string;
}

export interface Transaction {
    transactionId: number;
    userId: number;
    transactionType: string;
    amount: number;
    date: Date;
    details: string;
}