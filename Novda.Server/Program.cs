using Microsoft.EntityFrameworkCore;
using Novda.Server.BackgroundServices;
using Novda.Server.Infrastructure;
using Novda.Server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<NovdaMiddleware>();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<UpdateNovdaServerService>();

var connectionString = builder.Configuration.GetConnectionString("Novda");
builder.Services.AddDbContext<NovdaDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseMiddleware<NovdaMiddleware>();

app.Run();
