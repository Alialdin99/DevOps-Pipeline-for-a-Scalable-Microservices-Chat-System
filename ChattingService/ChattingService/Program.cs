using ChattingService.Hubs;
using ChattingService.Repositories;
using ChattingService.Repositories;
using ChattingService.Services;
using MassTransit;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//MongoDB setup
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


builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<ChatService>();

//SignalR setup
builder.Services.AddSignalR();

//MassTransit setup (RabbitMQ)
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
    });
});

builder.Services.AddMassTransitHostedService();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
