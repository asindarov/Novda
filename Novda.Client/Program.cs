using Microsoft.AspNetCore.SignalR.Client;
using Novda.Shared;

var client = new HttpClient();

var updateNovdaServerConnection = new HubConnectionBuilder()
    .WithUrl($"{NovdaConstants.NovdaSignalRServer}/UpdateNovdaServer")
    .WithAutomaticReconnect()
    .Build();

updateNovdaServerConnection.Closed += async (error) =>
{
    await Task.Delay(new Random().Next(0,5) * 1000);
    await updateNovdaServerConnection.StartAsync();
};

updateNovdaServerConnection.On<NovdaMetadata>(NovdaConstants.ReceiveMetadataFromNovdaServer, Handler);

await updateNovdaServerConnection.StartAsync();

await updateNovdaServerConnection.InvokeAsync(NovdaConstants.RegisterNovdaClient, "http://localhost:8000");

Console.ReadLine();

async Task Handler(NovdaMetadata metadata)
{
    var connection = new HubConnectionBuilder()
        .WithUrl($"{NovdaConstants.NovdaSignalRServer}/UpdateNovdaServer")
        .WithAutomaticReconnect()
        .Build();

    await connection.StartAsync();

    Console.WriteLine($"AppId: {metadata.AppId}");
    Console.WriteLine($"HttpMethod: {metadata.HttpMethod}");
    Console.WriteLine($"LocalUrl: {metadata.LocalAppUrl}");

    if (metadata.HttpMethod.Equals(HttpMethod.Get.ToString()))
    {
        var response = await client.GetStringAsync(metadata.RequestedUrl);
        Console.WriteLine(response);
        await connection.InvokeAsync(NovdaConstants.SendResponseFromNovdaClient, response);
    }
}
