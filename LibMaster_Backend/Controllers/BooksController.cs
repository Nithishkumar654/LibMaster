using LibraryManagementSystem.Filters;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }


        [HttpPost]
        [JwtValidation]
        [RoleAuthorize("librarian")]
        public async Task<IActionResult> AddBook(Book book)
        {
            try
            {
                if (book == null || !ModelState.IsValid)
                {
                    return BadRequest( new { error = ModelState });
                }

                await _bookService.AddBook(book);
                return CreatedAtAction(nameof(GetBookById), new { id = book.BookId }, book);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("book/{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            try
            {
                var book = await _bookService.GetBookById(id);
                if (book == null)
                {
                    return NotFound(new { error = "Book Not Found" });
                }
                return Ok(new { message = book});
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks(string? search, int? category)
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                Console.WriteLine(userId);
                var books = await _bookService.GetBooks(userId?.ToString(), search, category);
                return Ok(new { message = books });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

        [HttpPut("{id}")]
        [JwtValidation]
        [RoleAuthorize("librarian")]
        public async Task<IActionResult> UpdateCopies(int id, int copies)
        {
            try
            {
                await _bookService.UpdateCopies(id, copies);    
                return Ok(new { message = "Book Updated Successfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

        [HttpDelete("{id}")]
        [JwtValidation]
        [RoleAuthorize("librarian")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await _bookService.DeleteBook(id);
                return Ok(new { message = "Book Deleted Succesfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

        [HttpPut("borrow/{id}")]    
        [JwtValidation]
        [RoleAuthorize("member")]
        public async Task<IActionResult> BorrowBook(int id)
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest(new { error = "User Not Found" });
                }
                await _bookService.BorrowBook(int.Parse(userId.ToString()), id);
                return Ok(new { message = "Book Borrowed Successfully." });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

        [HttpPut("return/{id}")]
        [JwtValidation]
        [RoleAuthorize("member")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound(new { error = "User Not Found" });
                await _bookService.ReturnBook(int.Parse(userId.ToString()), id);
                return Ok(new { message = "Book Returned Successfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

        [HttpPut("reserve/{id}")]
        [JwtValidation]
        [RoleAuthorize("member")]
        public async Task<IActionResult> ReserveBook(int id)
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound(new { error = "User Not Found" });
                await _bookService.ReserveBook(int.Parse(userId.ToString()), id);
                return Ok(new { message = "Book Reserved Successfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }


        [HttpPut("withdraw/{id}")]
        [JwtValidation]
        [RoleAuthorize("member")]
        public async Task<IActionResult> WithDrawReservation(int id)
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound(new { error = "User Not Found" });
                await _bookService.WithdrawReserve(id);
                return Ok(new { message = "Reserve Request WithDrawn Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("report")]
        [RoleAuthorize("guest,member")]
        [JwtValidation]
        public async Task<IActionResult> GetReport()
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound(new { error = "User Not Found" });

                var report = await _bookService.GetReport(int.Parse(userId.ToString()));
                return Ok(new { message = report });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

        [HttpGet("reserved")]
        [JwtValidation]
        [RoleAuthorize("member")]
        public async Task<IActionResult> GetReservedBooks()
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound(new { error = "User Not Found" });

                var books = await _bookService.GetReservedBooks(int.Parse(userId.ToString()));
                return Ok(new { message = books });
            }
            catch(Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

        [HttpGet("borrow")]
        [JwtValidation]
        [RoleAuthorize("member")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound(new { error = "User Not Found" });

                var books = await _bookService.GetBorrowedBooks(int.Parse(userId.ToString()));
                return Ok(new { message = books });
            }
            catch (Exception ex)
            {
                return BadRequest(new{error = ex.Message});
            }
        }

    }
}
