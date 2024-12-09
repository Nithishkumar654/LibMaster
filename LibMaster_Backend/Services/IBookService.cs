using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services
{
    public interface IBookService
    {
        public Task<Book> AddBook(Book book);
        public Task<Book> GetBookById(int id);
        public Task<List<Book>> GetBooks(string? userId, string? search, int? category);
        public Task UpdateCopies(int id, int copies);
        public Task DeleteBook(int id);
        public Task BorrowBook(int userId, int id);
        public Task ReturnBook(int userId, int id);
        public Task ReserveBook(int userId, int id);
        public Task WithdrawReserve(int id);
        public Task<Report> GetReport(int userId);
        public Task<List<Reservation>> GetReservedBooks(int userId);
        public Task<List<BorrowedBook>> GetBorrowedBooks(int userId);
    }
}
