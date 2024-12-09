using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BorrowedBook
    {
        [Key]
        public int BorrowId { get; set; }

        [Required(ErrorMessage = "BookId is required")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "MemberId is required")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "BorrowDate is required")]
        [DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; }

        [Required(ErrorMessage = "DueDate is required")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; } // Nullable in case the book hasn't been returned yet

        [Range(0, double.MaxValue, ErrorMessage = "LateFee cannot be negative")]
        public decimal LateFee { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } // E.g., "Borrowed", "Returned", "Overdue", etc.

        // Navigation Properties
        public virtual Book Book { get; set; }
        public virtual Member Member { get; set; }
    }
}
