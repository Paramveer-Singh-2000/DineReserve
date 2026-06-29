using DineReserve.API.Extensions;
using DineReserve.API.Middleware;
using DineReserve.Application.Extension;
using DineReserve.Domain.Entities;
using DineReserve.Infrastructure.Authorization;
using DineReserve.Infrastructure.Extensions;
using DineReserve.Infrastructure.Persistance;
using DineReserve.Infrastructure.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddApplication();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.AddPresentationServices();
builder.Services.AddAuthentication();
builder.Services
    .AddIdentityApiEndpoints<User>().AddRoles<IdentityRole>()
    .AddClaimsPrincipalFactory<RestaurantUserClaimFactoryPrincipal>()
    .AddEntityFrameworkStores<DineReserveDbContext>();

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
app.MapGroup("api/identity").WithTags("Identity").MapIdentityApi<User>(); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
