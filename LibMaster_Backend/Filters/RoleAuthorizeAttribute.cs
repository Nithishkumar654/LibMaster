using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace LibraryManagementSystem.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] roles;

        public RoleAuthorizeAttribute(string roles)
        {
            this.roles = roles.Split(',').Select(r => r.Trim()).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userRole = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            Console.WriteLine(userRole);
            if (userRole == null || !roles.Contains(userRole))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
