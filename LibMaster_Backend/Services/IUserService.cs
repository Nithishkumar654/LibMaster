using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Services
{
    public interface IUserService
    {
        public Task RegisterUser(User user);
        public Task<string> AuthenticateUser(string email, string password);
        public Task<User> GetUserById(int id);
        public Task UpdateUser(User user);
        public Task Payment(TransactionDTO transaction);
        public Task<bool> IsMember(int userId);
        public Task Accept(int userId);
        public Task<List<User>> GetRequests();
        public Task<List<Transaction>> GetTransactions(int userId);
        public Task<string> SendOTP(string email);
        public Task ResetPass(ResetPassDTO resetPassDTO);
    }
}
