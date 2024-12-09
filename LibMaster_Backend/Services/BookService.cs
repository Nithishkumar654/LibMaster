using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repository;

namespace LibraryManagementSystem.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository repository;
        private readonly IUserRepository userRepository;
        private readonly NotificationService notificationService;
        public BookService(IBookRepository _repo, IUserRepository repository, NotificationService notificationService)
        {
            this.repository = _repo;
            this.userRepository = repository;
            this.notificationService = notificationService;
        }

        public async Task<Book> AddBook(Book book)
        {
            try
            {
                return await repository.AddBook(book);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task BorrowBook(int userId, int id)
        {
            try
            {
                var book = await repository.GetBookById(id);
                var member
                        = await userRepository.GetMember(userId);
                var borrowed = await repository.GetBorrowedBooks(member.MemberId);

                borrowed = borrowed.Where(b => b.Status != "Returned").ToList();

                if(borrowed != null && borrowed.Find(b => b.BookId == id) != null)
                {
                    throw new Exception("Same Book Cannot Be Borrowed At A Time.");
                }

                if(borrowed.Count == 5)
                {
                    throw new Exception("Maximum Borrow Limit Reached (5).");
                }

                if (book.Status == "Available")
                {
                    book.AvailableCopies--;
                    if(book.AvailableCopies == 0)
                    {
                        book.Status = "Not Available";
                    }
                    
                    await repository.AddBorrowedBooks(new BorrowedBook()
                    {
                        BookId = id,
                        MemberId = member.MemberId,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(10),
                        Status = "Borrowed"
                    });
                    await repository.UpdateBook(book);


                    // send notification

                    string message = $@"
<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Book Borrowing Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
        }}

        .header {{
            background-color: #28a745;
            padding: 10px;
            color: white;
            text-align: center;
            border-radius: 8px 8px 0 0;
        }}

        .content {{
            padding: 20px;
            line-height: 1.6;
            color: #333333;
        }}

        .footer {{
            text-align: center;
            font-size: 12px;
            color: #888888;
            padding: 10px 0;
        }}

        .button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007BFF;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 10px;
        }}
    </style>
</head>

<body>
    <div class=""email-container"">
        <div class=""header"">
            <h1>Book Borrowing Confirmation</h1>
        </div>
        <div class=""content"">
            <p>Dear <strong>{member.Name}</strong>,</p>
            <p>We are excited to inform you that your book borrowing request has been successfully processed! 🎉</p>
            <p><strong>Book Details:</strong></p>
            <ul>
                <li><strong>Title:</strong> {book.Title}</li>
                <li><strong>Author:</strong> {book.Author}</li>
                <li><strong>Borrowed On:</strong> {DateTime.Now.ToString("MM/dd/yyyy")}</li>
                <li><strong>Due Date:</strong> {DateTime.Now.AddDays(10).ToString("MM/dd/yyyy")}</li>
            </ul>
            <p>Please make sure to return the book by the due date to avoid any late fees. If you need any assistance or have any questions regarding the borrowed books, feel free to reach out to us.</p>
            <p>Enjoy your reading! 📖✨</p>
            <p>If you'd like to borrow more books, feel free to visit our library or check out our online catalog.</p>
            <p>Thank you for being a valued member of our library!</p>
            <p>Warm regards,<br>Library System Team</p>
        </div>
        <div class=""footer"">
            &copy; {DateTime.Now.Year} Library System. All rights reserved.
        </div>
    </div>
</body>

</html>
";
                    await notificationService.SendNotification(member.ContactDetails, member.Name, message);
                }
                else
                {
                    throw new Exception("Book Not Available");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteBook(int id)
        {
            try
            {
                await repository.DeleteBook(id);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Book> GetBookById(int id)
        {
            try
            {
                var book = await repository.GetBookById(id);
                return book;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Book>> GetBooks(string? userId, string? search, int? category)
        {
            try
            {
                var books = await repository.GetBooks(search, category);
                if (userId != null) {
                    await repository.AddBookSearch(new BookSearch()
                    {
                        UserId = int.Parse(userId),
                        SearchQuery = search ?? "None",
                        SearchDate = DateTime.Now,
                        ResultsCount = books.Count(),
                    });
                }
                return books;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BorrowedBook>> GetBorrowedBooks(int userId)
        {
            try
            {
                var member = await userRepository.GetMember(userId);

                var borrowedBooks = await repository.GetBorrowedBooks(member.MemberId);
                //var books = new List<BorrowedBook>();

                foreach (var book in borrowedBooks)
                {
                    var b = await GetBookById(book.BookId);
                    book.Book = b;
                }

                return borrowedBooks;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<Report> GetReport(int userId)
        {
            throw new NotImplementedException();
        }
        
        public async Task<List<Reservation>> GetReservedBooks(int userId)
        {
            try
            {
                var member = await userRepository.GetMember(userId);
                var books = await repository.GetReserveBooks(member.MemberId);
                foreach (var book in books)
                {
                    var b = await GetBookById(book.BookId);
                    book.Book = b;
                }
                return books;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ReserveBook(int userId, int id)
        {
            try
            {
                var book = await repository.GetBookById(id);
                if (book.Status == "Not Available")
                {
                    var member = await userRepository.GetMember(userId);
                    var reserved = await repository.GetReserveBooks(member.MemberId);
                    var borrowed = await repository.GetBorrowedBooks(member.MemberId);
                    borrowed = borrowed.Where(b => b.Status != "Returned").ToList();

                    if((reserved != null && reserved.Find(b => b.BookId == id) != null) || (borrowed != null && borrowed.Find(b => b.BookId == id) != null))
                    {
                        throw new Exception("Book Already Borrowed/Reserved");
                    }

                    await repository.AddReservation(new Reservation()
                    {
                        BookId = id,
                        MemberId = member.MemberId,
                        ReservationDate = DateTime.Now,
                        Status = "Active"
                    });

                    // send notification


                    string message = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Book Reservation Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            color: #333;
            line-height: 1.6;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }}
        h2 {{
            text-align: center;
            color: #2d6a4f;
        }}
        p {{
            font-size: 16px;
            color: #555;
        }}
        .details {{
            margin: 20px 0;
            padding: 10px;
            background-color: #e9ecef;
            border-radius: 5px;
        }}
        .details p {{
            margin: 5px 0;
        }}
        .footer {{
            text-align: center;
            font-size: 14px;
            color: #888;
            margin-top: 30px;
        }}
        .btn {{
            display: inline-block;
            background-color: #2d6a4f;
            color: #fff;
            padding: 10px 20px;
            border-radius: 5px;
            text-decoration: none;
            font-weight: bold;
            text-align: center;
            margin-top: 20px;
        }}
        .btn:hover {{
            background-color: #1c4e34;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <h2>Book Reservation Confirmation</h2>
        <p>Dear {member.Name},</p>
        <p>We are pleased to inform you that your reservation for the book has been successfully completed. Below are the details of your reservation:</p>
        <p>We will update you as soon as the book is available in the library.</p>
        <div class=""details"">
            <p><strong>Book Title:</strong> {book.Title}</p>
            <p><strong>Author:</strong> {book.Author}</p>
            <p><strong>Reservation Date:</strong> {DateTime.Now.ToString("MM/dd/yyyy")}</p>
        </div>

        <p>If you have any questions or need further assistance, feel free to contact us.</p>
        
        <p>Thank you for choosing our library!</p>
        

        <div class=""footer"">
            <p>&copy; {DateTime.Now.Year} Library System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";
                    await notificationService.SendNotification(member.ContactDetails, member.Name, message);
                }
                else
                {
                    throw new Exception("Book Is Available Now. Order Now to get it.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task WithdrawReserve(int reservationId)
        {
            try
            {
                await repository.DeleteReservation(reservationId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ReturnBook(int userId, int id)
        {
            try
            {
                var book = await repository.GetBookById(id);
                book.AvailableCopies++;
                if(book.AvailableCopies == 1)
                {
                    book.Status = "Available";
                }
                var borrowedBook = await repository.GetBorrowedBookByBookId(id);
                var member = await userRepository.GetMember(userId) as Member;
                borrowedBook.ReturnDate = DateTime.Now;
                borrowedBook.Status = "Returned";
                if(borrowedBook.ReturnDate > borrowedBook.DueDate)
                {
                    TimeSpan t = ((TimeSpan)(borrowedBook.ReturnDate - borrowedBook.DueDate));
                    borrowedBook.LateFee = t.Days * 50;
                }
                await repository.UpdateBorrowedBook(borrowedBook);
                await repository.UpdateBook(book);

                // find next member in reservation
                var nxt = await repository.GetReserveBooksByBookId(id);

                if (nxt != null)
                {
                    var user = await userRepository.GetMemberById(nxt.MemberId);
                    await BorrowBook(user.UserId, id);
                    nxt.Status = "Complete";
                    await repository.UpdateReservation(nxt);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateCopies(int id, int copies)
        {
            try
            {
                var book = await repository.GetBookById(id);
                book.AvailableCopies += copies;
                book.Status = "Available";
                await repository.UpdateBook(book);

                // find next member in reservation
                var nxt = await repository.GetReserveBooksByBookId(id);

                if (nxt != null)
                {
                    var user = await userRepository.GetMemberById(nxt.MemberId);
                    await BorrowBook(user.UserId, id);
                    nxt.Status = "Complete";
                    await repository.UpdateReservation(nxt);
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
