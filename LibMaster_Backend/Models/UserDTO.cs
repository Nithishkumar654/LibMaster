using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class UserDTO
    {
        [Required(ErrorMessage = "Email Address is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
