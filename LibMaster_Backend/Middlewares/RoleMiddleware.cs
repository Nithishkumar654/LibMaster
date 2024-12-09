using System.Security.Claims;

namespace LibraryManagementSystem.Middlewares
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate next;

        public RoleMiddleware(RequestDelegate next) { this.next = next; }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            string role = "User";

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Role "))
            {
                var token = authHeader.Substring("Role ".Length).Trim(); // Role Admin
                Console.WriteLine(token);
                if (token == "Admin")
                {
                    role = "Admin";
                }
            }

            var claims = new[] { new Claim(ClaimTypes.Role, role) };

            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            context.User = user;

            await next(context);
        }
    }
}
