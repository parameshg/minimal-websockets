using WebSockets;

public class Middleware
{
    private readonly RequestDelegate next;

    private readonly IServer server;

    public Middleware(RequestDelegate next, IServer server)
    {
        this.next = next;

        this.server = server;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(PathString.FromUriComponent("/ws")))
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using (var socket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    if (context.Request.Path.HasValue)
                    {
                        var segments = context.Request.Path.Value.Split('/');

                        if (Guid.TryParse(segments.LastOrDefault(), out var id))
                        {
                            await server.Accept(id, socket);
                        }
                    }
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        else
        {
            await next(context);
        }
    }
}