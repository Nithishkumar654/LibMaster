using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BookSearch
    {
        [Key]
        public int SearchId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Search Query is required")]
        [StringLength(500, ErrorMessage = "Search query cannot exceed 500 characters")]
        public string SearchQuery { get; set; }

        [Required(ErrorMessage = "Search Date is required")]
        [DataType(DataType.Date)]
        public DateTime SearchDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Results count cannot be negative")]
        public int ResultsCount { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
    }
}

