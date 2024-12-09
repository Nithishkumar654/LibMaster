using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using System.Security.Claims;

namespace LibraryManagementSystem.Filters
{
    public class ValidateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime dt)
            {
                if (dt > DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage ?? "Car must be manufactured in the past.");
                }
            }
            return ValidationResult.Success;
        }
    }

    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Email is required.");
            }

            string email = value.ToString();
            var dbContext = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;

            if (dbContext == null)
            {
                throw new InvalidOperationException("DbContext is not available for dependency injection.");
            }

            // Extract userId from the JWT token (to check if the email belongs to the user being updated)
            var userId = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            var currentUserId = userId?.HttpContext?.User?.Claims
                                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Get the user being validated (this will be the user being created or updated)
            var currentUser = validationContext.ObjectInstance as User;

            // Skip uniqueness check if we are updating the same user
            if (currentUserId != null)
            {
                return ValidationResult.Success;
            }

            // Check if the email already exists in the database
            if (dbContext.Users.Any(u => u.Email == email))
            {
                return new ValidationResult("Email Address is already in use.");
            }

            return ValidationResult.Success;
        }
    }

    public class MembershipTypeAttribute : ValidationAttribute
    {
        private readonly string[] _validMembershipTypes;

        public MembershipTypeAttribute(params string[] validMembershipTypes)
        {
            _validMembershipTypes = validMembershipTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Membership Type is required.");
            }

            if (Array.Exists(_validMembershipTypes, type => type.Equals(value.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult($"Invalid Membership Type. Valid types are: {string.Join(", ", _validMembershipTypes)}.");
            }
        }
    }
}
