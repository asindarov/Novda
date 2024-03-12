using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Novda.Shared;

namespace Novda.SignalR.Hubs;

public class RegisterNovdaServerAndClientHub : Hub
{
    public static ConcurrentDictionary<string, string> NovdaClientConnectionIdDictionary = new();

    public static string NovdaServerConnectionId = string.Empty;
    
    public async Task RegisterNovdaClient(string appUrl)
    {
        NovdaClientConnectionIdDictionary.TryAdd(appUrl, Context.ConnectionId);

        await Clients.Client(NovdaServerConnectionId).SendAsync(NovdaConstants.RegisterNovdaClient, appUrl);
    }

    public async Task RegisterNovdaServer()
    {
        NovdaServerConnectionId = Context.ConnectionId;
    }
    
    public async Task SendMetadataFromNovdaServer(NovdaMetadata metadata)
    {
        Console.WriteLine($"Metadata, http method: {metadata.HttpMethod}, request payload: {metadata.RequestPayload}");

        NovdaClientConnectionIdDictionary
            .TryGetValue(
                metadata.LocalAppUrl,
                out var clientConnectionId);

        NovdaServerConnectionId = Context.ConnectionId;

        await Clients.Client(clientConnectionId).SendAsync(NovdaConstants.ReceiveMetadataFromNovdaServer, metadata);
    }

    public async Task SendResponseFromNovdaClient(string response)
    {
        await Clients.Client(NovdaServerConnectionId).SendAsync(NovdaConstants.ReceiveResponseFromNovdaClient, response);
    }
}
