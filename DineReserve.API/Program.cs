using DineReserve.Infrastructure.Extensions;
using DineReserve.Infrastructure.Seeders;
using DineReserve.Application.Extension;
using Serilog;
using DineReserve.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Serilog from appsettings.json
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<RequestTimeLoggingMiddleware>();

builder.Services.AddApplication();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Seed database safely (dispose scope)
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IRestaurantSeeder>();
    await seeder.SeedAsync();
}

// Middleware pipeline
app.UseSerilogRequestLogging();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
