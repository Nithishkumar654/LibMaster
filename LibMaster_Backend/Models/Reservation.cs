using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }


        [Required(ErrorMessage = "BookID is Required")]
        public int BookId { get; set; }


        [Required(ErrorMessage = "MemberID is Required")]
        public int MemberId { get; set; }


        [Required(ErrorMessage = "ReservationDate is Required")]
        public DateTime ReservationDate { get; set; }


        [Required(ErrorMessage = "Status is Required")]
        public string Status { get; set; } // e.g., "Active", "Cancelled", "Completed"

        // Navigation Properties
        public virtual Member Member { get; set; }
        public virtual Book Book { get; set; }
    }
}
