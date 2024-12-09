using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Repository
{
    public interface IUserRepository
    {
        public  Task<User> AddUser(User user);
        public  Task<User> UpdateUser(User user);
        public  Task<Member> UpdateMember(Member user);
        public  Task DeleteUser(int userId);
        public  Task<User> GetUserById(int userId);
        public  Task<User> GetUserByEmail(string email);
        public Task<Member> GetMember(int userId);
        public Task<Member> GetMemberById(int memberId);
        public  Task<IEnumerable<User>> GetAllUsers();
        public  Task<bool> EmailExists(string email);
        public  Task<User> AuthenticateUser(string email, string password);
        public  Task ChangePassword(int userId, string newPassword);
        public  Task<bool> ResetPassword(string email);
        public Task AddTransaction(Transaction transaction);
        public Task AddMember(Member member);
        public Task<List<User>> GetRequests();
        public Task<List<Transaction>> GetTransactions(int userId);
    }
}
