using StackExchange.Redis;
using StreamsMS.Infrastructure.Data;
using DotNetEnv;
using StreamsMS.Infrastructure.Services;
using StreamsMS.Application.Interfaces;
using StreamsMS.Application.Services;
using StreamsMS.Infrastructure.SignalR;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddNpgsql<StreamsDbContext>(builder.Configuration.GetConnectionString("dbConnectionStreams"));

var connStringRedis = builder.Configuration.GetValue<string>("Redis:ConnectionString");

builder.Services.AddScoped<IConnectionMultiplexer>(cm => ConnectionMultiplexer.Connect(connStringRedis));
builder.Services.AddScoped<RedisContext>();


builder.Services.AddScoped<IStreamViewerService, StreamViewerService>();
builder.Services.AddScoped<IStreamsService, StreamsService>();

builder.Services.AddScoped<RedisViewerService>();
builder.Services.AddSingleton<StreamConnectionManager>();

builder.Services.AddSignalR();

// builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
