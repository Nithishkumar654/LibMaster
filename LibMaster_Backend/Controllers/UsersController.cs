using LibraryManagementSystem.Filters;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("AcceptLibrarian/{id}")]
        [JwtValidation]
        [RoleAuthorize("admin")]
        public async Task<IActionResult> AcceptLibrarian(int id)
        {
            try
            {
                await _userService.Accept(id);
                return Ok(new { message = "Librarian Added" });
            }
            catch(Exception ex)
            {
                return BadRequest( new { error = ex.Message });
            }
        }


        [HttpGet("requests")]
        [JwtValidation]
        [RoleAuthorize("admin")]
        public async Task<IActionResult> GetLibrarianRequests()
        {
            try
            {
                var users = await _userService.GetRequests();
                return Ok(new { message = users });
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                if (user == null || !ModelState.IsValid)
                {
                    throw new Exception("Error Occurred");
                }
                if(user.Role == "librarian")
                {
                    user.IsActive = false;
                }
                await _userService.RegisterUser(user);
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO user)
        {
            if (user == null || !ModelState.IsValid)
            {
                return BadRequest(new { error = "Error while Logging in.."});
            }
            try
            {
                string token = await _userService.AuthenticateUser(user.Email, user.Password);
                return Ok(new { message = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("authorized")]
        [JwtValidation]
        public IActionResult IsLoggedIn()
        {
            var role = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            Console.WriteLine(role) ;
            return Ok(role);
        }


        [HttpGet]
        [JwtValidation]
        public async Task<IActionResult> GetUserById()
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest(new { error = "UnAuthorized" });
                }
                var user = await _userService.GetUserById(Convert.ToInt32(userId));
                
                return Ok(new { message = user });
            }
            catch (Exception ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut]
        [JwtValidation]
        public async Task<IActionResult> UpdateUser(User user)
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest(new { error = "UnAuthorized" });
                }
                
                var ex_user = await _userService.GetUserById(Convert.ToInt32(userId));
                ex_user.Username = user.Username;

                await _userService.UpdateUser(ex_user);
                return Ok(new { message = "User Updated Successfully" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("payment")]
        [JwtValidation]
        [RoleAuthorize("guest,member")]
        public async Task<IActionResult> Payment(TransactionDTO transaction)
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if(userId == null)
                {
                    return BadRequest(new { error = "UnAuthorized" });
                }
                if (transaction.TransactionType != "MembershipFee" && transaction.TransactionType != "LateFee")
                {
                    return BadRequest(new { error = "Invalid Membership Type" });
                }
                transaction.UserId = Convert.ToInt32(userId);
                await _userService.Payment(transaction);
                return Ok(new { message = "Payment Successful" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("transactions")]
        [JwtValidation]
        [RoleAuthorize("member")]
        public async Task<IActionResult> Transactions()
        {
            try
            {
                var userId = User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest(new { error = "UnAuthorized" });
                }

                var transactions = await _userService.GetTransactions(Convert.ToInt32(userId));
                return Ok(new { message = transactions });
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet("sendotp")]
        public async Task<IActionResult> SendOTP(string email)
        {
            try
            {
                var token = await _userService.SendOTP(email);
                return Ok(new { message = token });
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("resetpass")]
        [OtpVerification]
        public async Task<IActionResult> ResetPass(ResetPassDTO dto)
        {
            try
            {
                await _userService.ResetPass(dto);
                return Ok(new {message = "Password Reset Successful"});
            }
            catch(Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }   
}
