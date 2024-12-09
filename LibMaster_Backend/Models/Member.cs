using LibraryManagementSystem.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can only contain alphabetic characters and spaces.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact details are required.")]
        [EmailAddress]
        public string ContactDetails { get; set; } // Can include phone, email, or address

        [Required(ErrorMessage = "Membership type is required.")]
        [MembershipType("Standard", "Premium", "Student", "Researcher")]  // Allowing specific membership types
        public string MembershipType { get; set; } // E.g., "Standard", "Premium", "Student",

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } // E.g., "Active", "Inactive"    

        // Adding Renewal Date
        [Required(ErrorMessage = "Renewal Date is required.")]
        [DataType(DataType.Date)]
        public DateTime RenewalDate { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Navigation Properties
        [JsonIgnore]
        public virtual List<BorrowedBook> BorrowedBooks { get; set; } = new();
        [JsonIgnore]
        public virtual List<Reservation> Reservations { get; set; } = new();
    }
}
