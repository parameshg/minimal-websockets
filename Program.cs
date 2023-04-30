using WebSockets;
using WebSockets.Repositories;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<ISchemaRepository, SchemaRepository>();

        builder.Services.AddSingleton<IServer, Server>();

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));

        var app = builder.Build();

        app.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        });

        app.UseServer();

        app.Run();
    }
}