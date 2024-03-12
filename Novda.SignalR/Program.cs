using Novda.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHub<RegisterNovdaServerAndClientHub>("UpdateNovdaServer");

app.UseAuthorization();

app.MapControllers();

app.Run();
