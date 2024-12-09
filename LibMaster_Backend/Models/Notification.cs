using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Notification Type is required")]
        [StringLength(50)] // Limit the length of the type
        public string Type { get; set; } // E.g., "Overdue Reminder", "New Arrival"

        [Required(ErrorMessage = "Message is required")]
        [StringLength(500)] // Add a limit for the message field
        public string Message { get; set; }

        [Required(ErrorMessage = "Timestamp is required")]
        public DateTime Timestamp { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } // Navigation to the User
    }
}
