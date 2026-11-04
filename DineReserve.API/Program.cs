using DineReserve.API.Middleware;
using DineReserve.Application.Extension;
using DineReserve.Domain.Entities;
using DineReserve.Infrastructure.Authorization;
using DineReserve.Infrastructure.Extensions;
using DineReserve.Infrastructure.Persistance;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application layer
builder.Services.AddApplication();

// Infrastructure + database
builder.Services.AddInfrastructureServices(builder.Configuration);

// Middleware
builder.Services.AddScoped<ErrorHandlingMiddleware>();

// Authentication
builder.Services.AddAuthentication();

builder.Services
    .AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole>()
    .AddClaimsPrincipalFactory<RestaurantUserClaimFactoryPrincipal>()
    .AddEntityFrameworkStores<DineReserveDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/identity")
    .MapIdentityApi<User>();

app.MapControllers();

app.Run();