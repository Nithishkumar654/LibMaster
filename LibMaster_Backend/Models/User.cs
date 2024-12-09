using LibraryManagementSystem.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Username must be between 6 and 100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; } // Store hashed passwords only for security

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } 

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [UniqueEmail]
        public string Email { get; set; }

        [Required(ErrorMessage = "Active status is required.")]
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual Member? Member { get; set; }
        [JsonIgnore]
        public virtual List<Notification> Notifications { get; set; } = new();
        [JsonIgnore]
        public virtual List<Transaction> Transactions { get; set; } = new();
        [JsonIgnore]
        public virtual List<Report> Reports { get; set; } = new();
        [JsonIgnore]
        public virtual ICollection<BookSearch> BookSearches { get; set; } = new List<BookSearch>();

    }
}
