using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repository
{
    public interface IBookRepository
    {
        public Task<Book> AddBook(Book book);
        public  Task<Book> GetBookById(int bookId);
        public  Task<List<Book>> GetAllBooks();
        public  Task<List<Book>> GetAvailableBooks();
        public  Task<Book> UpdateBook(Book book);
        public  Task DeleteBook(int bookId);
        public  Task<List<Book>> GetBooks(string? searchQuery, int? categoryId);
        public Task AddBookSearch(BookSearch bookSearch);
        public Task AddBorrowedBooks(BorrowedBook book);
        public Task AddReservation(Reservation book);
        public Task DeleteReservation(int reservationId);
        public Task<List<BorrowedBook>> GetBorrowedBooks(int memberId);
        public Task<List<Reservation>> GetReserveBooks(int memberId);
        public Task<Reservation> GetReserveBooksByBookId(int bookId);
        public Task<BorrowedBook> GetBorrowedBookByBookId(int bookId);
        public Task UpdateBorrowedBook(BorrowedBook book);
        public Task UpdateReservation(Reservation book);
    }
}
