namespace LibraryManagementSystem.Middlewares
{
    public static class RoleMiddlewareExtension
    {
        public static IApplicationBuilder UseRoleMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RoleMiddleware>();
        }
    }
}
