using AuthenticationService.Consumers;
using AuthenticationService.Repositories;
using AuthenticationService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Prometheus;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuration & Services =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== MongoDB =====
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

builder.Services.AddScoped<UserRepository, UserRepository>();
builder.Services.AddScoped<AuthService, AuthenticationService.Services.AuthService>();

// ===== MassTransit (RabbitMQ) =====
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserUpdatedConsumer>();
    x.AddConsumer<UserDeletedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddMassTransitHostedService();

// ===== Authentication (JWT) =====
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var key = Encoding.ASCII.GetBytes(jwtKey ?? "ReplaceWithStrongKey");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
    );
});

var app = builder.Build();

// ===== HTTP pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("AllowAll");

// Prometheus metrics middleware: collects HTTP metrics and exposes /metrics
app.UseHttpMetrics();           // collect default HTTP metrics (latency, request count, etc.)
app.MapMetrics("/metrics");     // expose metrics endpoint

app.MapControllers();

app.Run();
