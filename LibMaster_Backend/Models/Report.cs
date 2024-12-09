using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [Required(ErrorMessage = "Type is Required")]
        public string Type { get; set; } // E.g., "Borrowing Summary", "Popular Books"


        [Required(ErrorMessage = "GeneratedDate is Required")]
        public DateTime GeneratedDate { get; set; }


        [Required(ErrorMessage = "Data is Required")]
        public string Data { get; set; } // JSON


        [Required(ErrorMessage = "CreatedBy is Required")]
        public int UserId { get; set; } // UserId of the creator

        public virtual User User{ get; set; }
    }

}
