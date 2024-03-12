using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Novda.Server.Infrastructure;
using Novda.Shared;

namespace Novda.Server.Middlewares;

public class NovdaMiddleware(NovdaDbContext novdaDbContext) : IMiddleware
{
    private string Response = string.Empty;
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var requestPath = context.Request.Path.Value;

        var appUrl = context.Request.Host.Value.Split('.')[0];

        if (!appUrl.Contains("localhost"))
        {
            await InvokeHubConnection(context, requestPath, appUrl);
        }
        else
        {
            context.Response.StatusCode = 200;

            await context.Response.WriteAsJsonAsync(new
            {
                RequestPath = requestPath,
                AppUrl = appUrl
            });
        }
    }

    private async Task InvokeHubConnection(HttpContext context, string requestPath, string appUrl)
    {
        var appId = Guid.Parse(appUrl);
        var novdaApplication = await novdaDbContext.NovdaApplications.FirstOrDefaultAsync(na => na.Id.Equals(appId));
        
        var connection = new HubConnectionBuilder()
            .WithUrl($"{NovdaConstants.NovdaSignalRServer}/UpdateNovdaServer")
            .WithAutomaticReconnect()
            .Build();
        
        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0,5) * 1000);
            await connection.StartAsync();
        };

        connection.On<string>(NovdaConstants.ReceiveResponseFromNovdaClient, ReceiveResponseFromNovdaClientHandler);
        
        await connection.StartAsync();

        var metadata = new NovdaMetadata()
        {
            AppId = novdaApplication.Id,
            ConnectionId = connection.ConnectionId,
            HttpMethod = context.Request.Method,
            LocalAppUrl = novdaApplication.LocalAppUrl,
            RequestedUrl = $"{novdaApplication.LocalAppUrl}{requestPath}",
        };
        
        await connection.InvokeAsync(
            NovdaConstants.SendMetadataFromNovdaServer,
            metadata);

        for (int i = 0; i < 5; i++)
        {
            await Task.Delay(i * 1000);
            if (Response != string.Empty)
            {
                await context.Response.WriteAsync(Response);
                return;
            }
        }
        
        await context.Response.WriteAsync($"Error occured");
    }
    
    private async Task ReceiveResponseFromNovdaClientHandler(string response)
    {
        Response = response;
    }
}
