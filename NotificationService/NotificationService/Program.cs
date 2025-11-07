using MassTransit;
using NotificationService.Consumers; // where your consumers live
using NotificationService.Hubs;
using NotificationService.Services; // your EmailService + NotificationHub

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Enable CORS for frontend connections
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin() //  restrict in production
    );
});

// Add SignalR for real-time notifications
builder.Services.AddSignalR();

// Register your EmailService
builder.Services.AddSingleton<EmailService>();

// ✅ Define this BEFORE configuring MassTransit
var rabbitMqConnection = builder.Configuration.GetConnectionString("RabbitMq"); 

//Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();
    x.AddConsumer<MessageSentConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(new Uri(rabbitMqConnection), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("notification-service", e =>
        {
            e.ConfigureConsumer<UserRegisteredConsumer>(context);
            e.ConfigureConsumer<MessageSentConsumer>(context);
        });

        cfg.UseRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
    });
});

var app = builder.Build();

// Use Swagger in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

// Map your SignalR hub
app.MapHub<NotificationHub>("/notificationHub");

// Map controllers (if you ever expose APIs)
app.MapControllers();

app.Run();
