using StackExchange.Redis;
using StreamsMS.Infrastructure.Data;
using DotNetEnv;
using StreamsMS.Infrastructure.Services;
using StreamsMS.Application.Interfaces;
using StreamsMS.Application.Services;
using StreamsMS.Infrastructure.SignalR;
using StreamsMS.API.Hubs;
using Microsoft.OpenApi.Models;
using StreamsMS.Infrastructure.EventBus;
using StreamsMS.Infrastructure.Repository;
using StreamsMS.Infrastructure.Auth;
using StreamsMS.Infrastructure.Http;
using StreamsMS.Infrastructure.Swagger;
using System.Text.Json.Serialization;
using Prometheus;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StreamsMS API", Version = "v1" });
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SchemaFilter<EnumSchemaFilter>(); // Enables los enums as string

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT: Bearer {token}"
    });

    c.OperationFilter<AuthHeaderOperationFilter>();
});

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddNpgsql<StreamsDbContext>(builder.Configuration.GetConnectionString("dbConnectionStreams"));

var connStringRedis = builder.Configuration.GetValue<string>("Redis:ConnectionString");

builder.Services.AddScoped<IConnectionMultiplexer>(cm => ConnectionMultiplexer.Connect(connStringRedis));
builder.Services.AddScoped<RedisContext>();

builder.Services.AddScoped<IPlatformRepository, PlatformsRepository>();
builder.Services.AddScoped<IStreamRepository, StreamsRepository>();

builder.Services.AddScoped<IStreamViewerService, StreamViewerService>();
builder.Services.AddScoped<IStreamsService, StreamsService>();
builder.Services.AddScoped<IPlatformsService, PlatformsService>();

builder.Services.AddScoped<RedisViewerService>();
builder.Services.AddSingleton<StreamConnectionManager>();
builder.Services.AddSingleton<IEventBusProducer, EventBusProducer>();

builder.Services.AddHostedService<EventBusProducer>();

builder.Services.AddHttpClient<TicketHttpClient>(client =>
{
    var uriBaseTickets = builder.Configuration.GetValue<string>("URIBASE:TICKETS");
    client.BaseAddress = new Uri(uriBaseTickets);
});


builder.Services.AddSignalR();

// builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

builder.AddAppAuthentication();
builder.Services.AddAuthorization();
Metrics.SuppressDefaultMetrics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseHttpMetrics();

app.UseEndpoints(endpoints => {
    endpoints.MapMetrics();
});

app.MapGet("/", () => Results.Ok("Healthy"));

app.MapControllers();
//signal r connect
app.MapHub<StreamHub>("/streamHub");

app.Run();
