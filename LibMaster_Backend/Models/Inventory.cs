using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required(ErrorMessage = "Book ID is required.")]
        [ForeignKey("Book")] // Foreign key is explicitly mapped to the BookId property
        public int BookId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Condition is required.")]
        [StringLength(50, ErrorMessage = "Condition cannot exceed 50 characters.")]
        public string Condition { get; set; } // e.g., "Good", "Damaged", "Lost"

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
        public string Location { get; set; } // Physical location in the library

        // Navigation Property
        public virtual Book Book { get; set; }
    }
}
