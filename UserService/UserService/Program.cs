using MassTransit;
using MongoDB.Driver;
using System.Reflection;
using UserService.Consumers;
using UserService.Repositories;
using UserService.Services.Interface;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase("UserDb"));


builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IUserService, UserService.Services.ConcreteServices.UserService>();



builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserRegisteredConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", 5672, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("user-service-user-registered", e =>
        {
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            e.ConfigureConsumer<UserRegisteredConsumer>(context);
        });
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
   
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
