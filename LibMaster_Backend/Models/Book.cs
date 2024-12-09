using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s\-]+$", ErrorMessage = "Only alphabetic characters, spaces, and hyphens are allowed.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Genre is required.")]
        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
        [RegularExpression(@"^[A-Za-z\s\-]+$", ErrorMessage = "Only alphabetic characters, spaces, and hyphens are allowed.")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10 and 13 characters.")]
        [RegularExpression(@"^\d{9}[\d|X]$", ErrorMessage = "ISBN must be a valid number.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Publication Date is required.")]
        [DataType(DataType.Date)]
        public DateTime PublicationDate { get; set; }

        [Required(ErrorMessage = "Available Copies are required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Available copies cannot be negative.")]
        public int AvailableCopies { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        // Navigation Properties
        public virtual BookCategory Category { get; set; }

        [JsonIgnore]
        public virtual List<BorrowedBook> BorrowedBooks { get; set; } = new();

        [JsonIgnore]
        public virtual List<Reservation> Reservations { get; set; } = new();

        // Inventory relation
        public virtual Inventory Inventory { get; set; }
    }
}
