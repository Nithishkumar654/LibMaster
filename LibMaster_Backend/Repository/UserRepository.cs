using BCrypt.Net;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;  // Assuming you have a Models namespace for entities
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. Add a new user
        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // 2. Update an existing user
        public async Task<User> UpdateUser(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<Member> UpdateMember(Member user)
        {
            _context.Members.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // 3. Delete a user by ID
        public async Task DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User Not Found");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        // 4. Get user by ID
        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // 5. Get user by email
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<User>> GetRequests()
        {
            return await _context.Users.Where(user => user.IsActive == false).ToListAsync();
        }

        // 6. Get all users
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // 7. Check if email exists
        public async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // 8. Authenticate user by email and password (assuming password hashing is implemented)
        public async Task<User> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            if (ValidatePassword(password, user.PasswordHash))
                return user;

            return null;
        }

        // 9. Change user password
        public async Task ChangePassword(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User Not Found");
            user.PasswordHash = HashPassword(newPassword); 
            await _context.SaveChangesAsync();
        }

        // 10. Reset password (send reset link, etc.)
        public async Task<bool> ResetPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return false;

            // Implement logic to generate and send a password reset link here
            // e.g., send email with token, store it, etc.
            return true;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool ValidatePassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        public async Task<Member> GetMember(int userId)
        {
            try
            {
                var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);
                return member;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Member> GetMemberById(int memberId)
        {
            try
            {
                var member = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == memberId);
                if (member == null)
                {
                    throw new Exception("Member Not Found");
                }
                return member;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddTransaction(Transaction transaction)
        {
            try
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AddMember(Member member)
        {
            try
            {
                _context.Members.Add(member);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Transaction>> GetTransactions(int userId)
        {
            try
            {
                return await _context.Transactions.Where(t => t.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
