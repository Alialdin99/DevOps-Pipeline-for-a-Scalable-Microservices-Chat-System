using MassTransit;
using MongoDB.Driver;
using NotificationService.Consumers;
using NotificationService.Hubs;
using NotificationService.Repositories;
using NotificationService.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB setup
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var conn = builder.Configuration.GetConnectionString("MongoDb");
    return new MongoClient(conn);
});

builder.Services.AddSingleton(sp =>
{
    var conn = builder.Configuration.GetConnectionString("MongoDb");
    var mongoUrl = new MongoUrl(conn);
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoUrl.DatabaseName);
});

builder.Services.AddScoped<NotificationRepository>();

// Enable CORS for frontend connections
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin() // restrict in production
    );
});

// Add SignalR for real-time notifications
builder.Services.AddSignalR();

// MassTransit setup
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SomeNotificationConsumer>(); // replace with actual consumers if present
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddMassTransitHostedService();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();
app.UseCors("AllowAll");

// Prometheus metrics middleware
app.UseHttpMetrics();
app.MapMetrics("/metrics");

app.MapControllers();
app.MapHub<NotificationHub>("/notificationhub");

app.Run();
