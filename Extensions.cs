namespace WebSockets
{
    public static class Extensions
    {
        public static IApplicationBuilder UseServer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware>();
        }
    }
}