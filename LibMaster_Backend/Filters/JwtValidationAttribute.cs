using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using LibraryManagementSystem.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LibraryManagementSystem.Filters
{
    public class JwtValidationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var token = authorizationHeader.StartsWith("Bearer ") ? authorizationHeader.Substring(7) : authorizationHeader;
            var jwtService = context.HttpContext.RequestServices.GetRequiredService<JwtService>();

            if (string.IsNullOrEmpty(token) || !jwtService.ValidateToken(token, out var principal))
            {
                // If the token is invalid, return Unauthorized
                context.Result = new UnauthorizedResult();
                return;
            }
            // Set the user claims
            context.HttpContext.User = principal;
        }
    }

    public class OtpVerification : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var otpHeader = context.HttpContext.Request.Headers["Otp"].FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || string.IsNullOrEmpty(otpHeader))
            {
                context.Result = new JsonResult(new { error = "Authorization token or OTP missing." }) { StatusCode = 400 };
                return;
            }

            var token = authorizationHeader.StartsWith("Bearer ") ? authorizationHeader.Substring(7) : authorizationHeader;
            var jwtService = context.HttpContext.RequestServices.GetRequiredService<JwtService>();

            var otp = otpHeader.StartsWith("Otp ") ? otpHeader.Substring(4) : otpHeader;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(otp) || !jwtService.ValidateOtpToken(token, otp))
            {
                // If the token is invalid, return Unauthorized
                context.Result = new JsonResult(new { error = "Invalid OTP" }) { StatusCode = 401 };
                return;
            }
        }
    }
}
