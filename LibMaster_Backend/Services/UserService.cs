using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repository;
using static System.Net.WebRequestMethods;

namespace LibraryManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly JwtService jwtsrv;
        private readonly IUserRepository _userRepo;
        private readonly NotificationService _notificationService;

        public UserService(JwtService jwtsrv, IUserRepository userRepo, NotificationService notificationService)
        {
            this.jwtsrv = jwtsrv;
            this._userRepo = userRepo;
            _notificationService = notificationService;
        }
        public async Task RegisterUser(User user)
        {
            try
            {
                user.PasswordHash = jwtsrv.HashPassword(user.PasswordHash);
                await _userRepo.AddUser(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> AuthenticateUser(string email, string password)
        {
            var user = await _userRepo.GetUserByEmail(email);

            if (user == null || !jwtsrv.ValidatePassword(password, user.PasswordHash))
            {
                throw new Exception("Invalid credentials.");
            }

            if(user.IsActive == false)
            {
                throw new Exception("You are not Allowed to Login. Please Contact Admin.");
            }

            // Generate JWT token
            var token = jwtsrv.GenerateToken(user);

            return token;
        }

        public async Task<User> GetUserById(int id)
        {
            try
            {
                return await _userRepo.GetUserById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                await _userRepo.UpdateUser(user);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task Payment(TransactionDTO transaction)
        {
            try
            {
                if(transaction.TransactionType == "MembershipFee" && await IsMember(transaction.UserId))
                {
                    throw new Exception("User is Already a Member");
                }

                await _userRepo.AddTransaction(new Transaction()
                {
                    UserId = transaction.UserId,
                    TransactionType = transaction.TransactionType,
                    Amount = transaction.Amount,
                    Date = DateTime.Now,
                    Details = transaction.Details
                });

                var user = await _userRepo.GetUserById(transaction.UserId);
                if(transaction.TransactionType.Equals("MembershipFee", StringComparison.OrdinalIgnoreCase))
                {
                    var member = await _userRepo.GetMember(transaction.UserId);
                    if(member != null)
                    {
                        member.RenewalDate = (transaction.Plan == "monthly" ? DateTime.Now.AddMonths(1) : DateTime.Now.AddYears(1));
                        await _userRepo.UpdateMember(member);
                    }
                    else {
                        await _userRepo.AddMember(new Member()
                        {
                            UserId = transaction.UserId,
                            Name = user.Username,
                            MembershipType = transaction.Plan,
                            Status = "Active",
                            ContactDetails = user.Email,
                            RenewalDate = (transaction.Plan == "monthly" ?  DateTime.Now.AddMonths(1) : DateTime.Now.AddYears(1))
                        });
                        user.Role = "member";
                        await _userRepo.UpdateUser(user);
                    }
                    string message = $@"
<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Membership Payment Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f9;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
        }}

        .header {{
            background-color: #4CAF50;
            padding: 10px;
            color: white;
            text-align: center;
            border-radius: 8px 8px 0 0;
        }}

        .content {{
            padding: 20px;
            line-height: 1.6;
            color: #333333;
        }}

        .footer {{
            text-align: center;
            font-size: 12px;
            color: #888888;
            padding: 10px 0;
        }}

        .button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #4CAF50;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 10px;
        }}
    </style>
</head>

<body>
    <div class=""email-container"">
        <div class=""header"">
            <h1>Library Membership Payment Confirmation</h1>
        </div>
        <div class=""content"">
            <p>Dear <strong>{user.Username}</strong>,</p>
            <p>We are happy to inform you that your membership payment has been successfully processed!</p>
            <p><strong>Transaction Details:</strong></p>
            <ul>
                <li><strong>Transaction Type:</strong> Membership Fee</li>
                <li><strong>Amount:</strong> ₹{transaction.Amount}</li>
                <li><strong>Payment Date:</strong> {DateTime.Now.ToString("MM/dd/yyyy")}</li>
                <li><strong>Membership Plan:</strong> {transaction.Plan}</li>
                <li><strong>Renewal Date:</strong> {(transaction.Plan == "monthly" ? DateTime.Now.AddMonths(1).ToString("MM/dd/yyyy") : DateTime.Now.AddYears(1).ToString("MM/dd/yyyy"))}</li>
            </ul>
            <p>Your membership is now active, and you can enjoy all the benefits and resources of the library. You will be notified when it is time to renew your membership.</p>
            <p>If you have any questions or need assistance, feel free to reach out to our support team at <a href=""mailto:support@library.com"">support@library.com</a>.</p>
            <p>Thank you for being a valuable member of our library community!</p>
            <p>Best regards,<br>Library System Team</p>
        </div>
        <div class=""footer"">
            &copy; {DateTime.Now.Year} Library System. All rights reserved.
        </div>
    </div>
</body>

</html>


                    ";

                    await _notificationService.SendNotification(user.Email, user.Username, message);
                }
                else
                {
                    string message = $@"
<!DOCTYPE html>
<html lang=""en"">

<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Late Fee Payment Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f9;
            margin: 0;
            padding: 0;
        }}

        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
        }}

        .header {{
            background-color: #007BFF;
            padding: 10px;
            color: white;
            text-align: center;
            border-radius: 8px 8px 0 0;
        }}

        .content {{
            padding: 20px;
            line-height: 1.6;
            color: #333333;
        }}

        .footer {{
            text-align: center;
            font-size: 12px;
            color: #888888;
            padding: 10px 0;
        }}

        .button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #007BFF;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-top: 10px;
        }}
    </style>
</head>

<body>
    <div class=""email-container"">
        <div class=""header"">
            <h1>Late Fee Payment Confirmation</h1>
        </div>
        <div class=""content"">
            <p>Dear <strong>{user.Username}</strong>,</p>
            <p>We are pleased to inform you that your Late Fee Payment has been successfully processed. Thank you for settling the overdue charges.</p>
            <p><strong>Payment Details:</strong></p>
            <ul>
                <li><strong>Transaction Type:</strong> Late Fee Payment</li>
                <li><strong>Amount Paid:</strong> ₹{transaction.Amount}</li>
                <li><strong>Payment Date:</strong> {DateTime.Now.ToString("MM/dd/yyyy")}</li>
            </ul>
            <p>Your account is now updated, and you are free to borrow books again or access other library services.</p>
            <p>If you have any questions or need further assistance, please feel free to contact us.</p>
            <p>Thank you for your prompt payment!</p>
            <p>Best regards,<br>Library System Team</p>
        </div>
        <div class=""footer"">
            &copy; {DateTime.Now.Year} Library System. All rights reserved.
        </div>
    </div>
</body>

</html>

                    ";


                    await _notificationService.SendNotification(user.Email, user.Username, message);
                }



            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> IsMember(int userId)
        {
            try
            {
                var user = await _userRepo.GetMember(userId);
                return user != null && user.RenewalDate > DateTime.Now;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task Accept(int userId)
        {
            try
            {
                var user = await GetUserById(userId);
                user.IsActive = true;
                await _userRepo.UpdateUser(user);

                //send email
                string message = $@"
 <!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Librarian Approval Notification</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            color: #333;
        }}
        .container {{
            width: 100%;
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
        }}
        .header {{
            text-align: center;
            color: #4CAF50;
        }}
        .header h1 {{
            font-size: 24px;
            margin: 0;
        }}
        .content {{
            font-size: 16px;
            line-height: 1.6;
        }}
        .content p {{
            margin: 15px 0;
        }}
        .content ul {{
            margin: 15px 0;
            padding-left: 20px;
        }}
        .content li {{
            margin-bottom: 8px;
        }}
        .footer {{
            margin-top: 20px;
            text-align: center;
            font-size: 14px;
            color: #777;
        }}
        .footer p {{
            margin: 0;
        }}
        .btn {{
            display: inline-block;
            background-color: #4CAF50;
            color: #fff;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 20px;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Congratulations! Your Librarian Request Has Been Approved 🎉</h1>
        </div>
        <div class=""content"">
            <p>Dear <strong>{user.Username}</strong>,</p>
            <p>We are thrilled to inform you that your request to become a <strong>Librarian</strong> has been <strong>approved</strong> by the admin! 🎉</p>
            <p>You are now officially part of our library team, and we couldn't be more excited to have you onboard.</p>

            <p><strong>Key Details Regarding Your Role:</strong></p>
            <ul>
                <li><strong>Role:</strong> Librarian</li>
                <li><strong>Start Date:</strong> {DateTime.Now.ToString("MM/dd/yyyy")}</li>
                <li><strong>Responsibilities:</strong>
                    <ul>
                        <li>Manage and oversee the book inventory</li>
                        <li>Assist members with book borrowing and returns</li>
                        <li>Support members with inquiries and recommendations</li>
                        <li>Maintain and update library records</li>
                    </ul>
                </li>
            </ul>

            <p>As a librarian, you now have access to our library system to manage and monitor daily operations. Your contribution will play a vital role in enhancing the library experience for all members. 📚</p>

            <p>If you have any questions or need further guidance regarding your new role, please don’t hesitate to:</p>
            <ul>
                <li>Contact the admin team</li>
                <li>Refer to the internal documentation for librarians</li>
            </ul>

            <p>We look forward to working together and making our library an even better place for everyone! ✨</p>

            <p><strong>Welcome to the team!</strong></p>
        </div>
        <div class=""footer"">
            <p>Warm regards,</p>
            <p><strong>Library System Team</strong></p>
        </div>
    </div>
</body>
</html>


                ";
                await _notificationService.SendNotification(user.Email, user.Username, message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<User>> GetRequests()
        {
            try
            {
                var users = await _userRepo.GetRequests();
                return users;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Transaction>> GetTransactions(int userId)
        {
            try
            {
                return await _userRepo.GetTransactions(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not find transaction {userId}");
            }
        }

        public async Task<string> SendOTP(string email)
        {
            try
            {
                var user = await _userRepo.GetUserByEmail(email);
                if(user == null)
                {
                    throw new Exception("No User Found with the email provided.");
                }

                var otp = new Random().Next(100000, 999999).ToString();

                string token = jwtsrv.GenerateOTPToken(email, otp);

                string message = $@"
            <html>
            <body style='font-family: Arial, sans-serif; background-color: #f8f9fa; padding: 20px;'>
                <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 10px; 
                             box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); padding: 20px;'>
                    <h2 style='color: #007bff; text-align: center;'>Your OTP Code</h2>
                    <p style='font-size: 16px; color: #333;'>Hello <strong>{user.Username}</strong>,</p>
                    <p style='font-size: 16px; color: #333;'>Your One-Time Password (OTP) is:</p>
                    <div style='text-align: center; margin: 20px 0;'>
                        <span style='font-size: 24px; font-weight: bold; color: #28a745;'>{otp}</span>
                    </div>
                    <p style='font-size: 14px; color: #666;'>
                        This OTP is valid for the next 10 minutes. If you did not request this, please ignore this email.
                    </p>
                    <hr style='border: none; border-top: 1px solid #ddd;' />
                    <p style='font-size: 12px; text-align: center; color: #999;'>
                        &copy; {DateTime.UtcNow.Year} Library Management System. All rights reserved.
                    </p>
                </div>
            </body>
            </html>
        ";
                await _notificationService.SendNotification(email, user.Username, message);

                return token;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task ResetPass(ResetPassDTO resetPassDTO)
        {
            try
            {
                var user = await _userRepo.GetUserByEmail(resetPassDTO.Email);
                user.PasswordHash = jwtsrv.HashPassword(resetPassDTO.Password);

                await _userRepo.UpdateUser(user);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
