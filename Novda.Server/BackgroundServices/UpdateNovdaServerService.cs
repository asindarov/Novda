using Microsoft.AspNetCore.SignalR.Client;
using Novda.Server.Infrastructure;
using Novda.Server.Models;
using Novda.Shared;

namespace Novda.Server.BackgroundServices;

public class UpdateNovdaServerService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl($"{NovdaConstants.NovdaSignalRServer}/UpdateNovdaServer")
            .WithAutomaticReconnect()
            .Build();
    
        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0,5) * 1000, cancellationToken);
            await connection.StartAsync(cancellationToken);
        };

        connection.On<string>(NovdaConstants.RegisterNovdaClient, Handler);
        
        await connection.StartAsync(cancellationToken);
        
        await connection.InvokeAsync(NovdaConstants.RegisterNovdaServer, cancellationToken: cancellationToken);
    }
    
    private async Task Handler(string appUrl)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var novdaDbContext = scope.ServiceProvider.GetRequiredService<NovdaDbContext>();
            var appId = Guid.NewGuid();
            await novdaDbContext.NovdaApplications.AddAsync(new NovdaApplication()
            {
                Id = appId,
                LocalAppUrl = appUrl,
            });

            await novdaDbContext.SaveChangesAsync();
        
            Console.WriteLine($"The following local app url is registered: {appUrl} and here is the appId: {appId}");   
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
