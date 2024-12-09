using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;  // Assuming this contains the Book model
using LibraryManagementSystem.Data;  // Assuming this contains the DbContext

namespace LibraryManagementSystem.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        // Create or add a new book
        public async Task<Book> AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> GetBookById(int bookId)
        {
            var book = await _context.Books
                .Where(b => b.BookId == bookId)
                .Include(b => b.Category)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    ISBN = b.ISBN,
                    PublicationDate = b.PublicationDate,
                    AvailableCopies = b.AvailableCopies,
                    Status = b.Status,
                    CategoryId = b.CategoryId,
                    Category = new BookCategory
                    {
                        CategoryId = b.Category.CategoryId,
                        CategoryName = b.Category.CategoryName,
                        Description = b.Category.Description
                    }
                })
                .FirstOrDefaultAsync();
            if(book == null)
            {
                throw new Exception("Book Not Found");
            }
            return book;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            return await _context.Books.ToListAsync();
        }


        public async Task<List<Book>> GetBooks(string? search, int? category)
        {
        try
        {
            var booksQuery = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                booksQuery = booksQuery.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
            }

            if (category.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.CategoryId == category.Value);
            }

            var books = await booksQuery
                .Include(b => b.Category)
                .Select(b => new Book
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Genre = b.Genre,
                    ISBN = b.ISBN,
                    PublicationDate = b.PublicationDate,
                    AvailableCopies = b.AvailableCopies,
                    Status = b.Status,
                    CategoryId = b.CategoryId,
                    Category = new BookCategory
                    {
                        CategoryId = b.Category.CategoryId,
                        CategoryName = b.Category.CategoryName,
                        Description = b.Category.Description
                    },
                    Inventory = new Inventory
                    {
                        InventoryId = b.Inventory.InventoryId,
                        BookId = b.Inventory.BookId,
                        Quantity = b.Inventory.Quantity,
                        Condition = b.Inventory.Condition,
                        Location = b.Inventory.Location
                    }
                })
                .ToListAsync();

            return books;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


        public async Task<List<Book>> GetAvailableBooks()
        {
            return await _context.Books
                .Where(b => b.AvailableCopies > 0)
                .Include(b => b.Category)
                .ToListAsync();
        }

        public async Task<Book> UpdateBook(Book book)
        {
            try
            {
                var existingBook = await _context.Books.FindAsync(book.BookId);

                if (existingBook == null)
                {
                    throw new Exception($"Book with ID {book.BookId} not found.");
                }

                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.Genre = book.Genre;
                existingBook.ISBN = book.ISBN;
                existingBook.PublicationDate = book.PublicationDate;
                existingBook.AvailableCopies = book.AvailableCopies;
                existingBook.Status = book.Status;
                existingBook.CategoryId = book.CategoryId;

                _context.Books.Update(existingBook);
                await _context.SaveChangesAsync();

                return existingBook;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating book: {ex.Message}");
            }
        }


        // Delete a book by its ID
        public async Task DeleteBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                throw new Exception("Book Not Found");
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task AddBookSearch(BookSearch bookSearch)
        {
            try
            {
                _context.BookSearches.Add(bookSearch);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddBorrowedBooks(BorrowedBook book)
        {
            try
            {
                _context.BorrowedBooks.Add(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BorrowedBook>> GetBorrowedBooks(int memberId)
        {
            try
            {
                var borrowedBooks = await _context.BorrowedBooks.Where(b => b.MemberId == memberId).ToListAsync();

                return borrowedBooks;
            }
            catch(Exception ex)
            { 
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Reservation>> GetReserveBooks(int memberId)
        {
            try
            {
                var reservedBooks = await _context.Reservations.Where(b => b.MemberId == memberId).ToListAsync();
                return reservedBooks;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddReservation(Reservation book)
        {
            try
            {
                _context.Reservations.Add(book);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteReservation(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            if (reservation == null)
                throw new Exception("Reservation Not Found");
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<Reservation> GetReserveBooksByBookId(int bookId)
        {
            try
            {
                return await _context.Reservations.FirstOrDefaultAsync(r => r.BookId == bookId && r.Status == "Active");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BorrowedBook> GetBorrowedBookByBookId(int bookId)
        {
            try
            {
                return await _context.BorrowedBooks.FirstOrDefaultAsync(r => r.BookId == bookId && r.Status == "Borrowed");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateBorrowedBook(BorrowedBook book)
        {
            try
            {
                var existingBook = await _context.BorrowedBooks.FindAsync(book.BorrowId);

                if (existingBook == null)
                {
                    throw new Exception($"BorrowedBook with ID {book.BorrowId} not found.");
                }

                existingBook.BookId = book.BookId;
                existingBook.MemberId = book.MemberId;
                existingBook.BorrowDate = book.BorrowDate;
                existingBook.DueDate = book.DueDate;
                existingBook.ReturnDate = book.ReturnDate;
                existingBook.Status = book.Status;
                existingBook.LateFee = book.LateFee;

                _context.BorrowedBooks.Update(existingBook);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating BorrowedBook: {ex.Message}");
            }
        }


        public async Task UpdateReservation(Reservation reservation)
        {
            try
            {
                var existingReservation = await _context.Reservations.FindAsync(reservation.ReservationId);

                if (existingReservation == null)
                {
                    throw new Exception($"Reservation with ID {reservation.ReservationId} not found.");
                }

                existingReservation.BookId = reservation.BookId;
                existingReservation.MemberId = reservation.MemberId;
                existingReservation.ReservationDate = reservation.ReservationDate;
                existingReservation.Status = reservation.Status;

                _context.Reservations.Update(existingReservation);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating reservation: {ex.Message}");
            }
        }

    }
}
