using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LibraryManagementSystem.Models
{
    public class TransactionDTO
    {
        public int UserId { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Details { get; set; }
        public string Plan { get; set; }
    }
}
