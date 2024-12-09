using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }


        [Required(ErrorMessage = "UserID is Required")]
        public int UserId { get; set; }


        [Required(ErrorMessage = "TransactionType is Required")]
        public string TransactionType { get; set; } // E.g., "Late Fee", "Membership Fee", "Book Purchase"


        [Required(ErrorMessage = "Amount is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Amount cant be negative.")]
        public decimal Amount { get; set; }


        [Required(ErrorMessage = "Date is Required")]
        public DateTime Date { get; set; }
        public string Details { get; set; }

        public virtual User User { get; set; }
    }

}
