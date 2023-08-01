using EnsureThat;

namespace WebSockets
{
    public class Beacon : IHostedService
    {
        private System.Timers.Timer Trigger = new System.Timers.Timer(10000);

        private IServer Server { get; }

        public Beacon(IServer server)
        {
            Server = EnsureArg.IsNotNull(server);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Trigger.Elapsed += OnTriggered;

            Trigger.Enabled = true;

            Trigger.Start();

            return Task.CompletedTask;
        }

        private void OnTriggered(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Server.Broadcast(DateTime.Now.ToString()).GetAwaiter().GetResult();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Trigger.Enabled = false;

            Trigger.Stop();

            return Task.CompletedTask;
        }
    }
}