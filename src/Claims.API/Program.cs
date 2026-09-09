using Claims.API.Exceptions;
using Claims.Application.Claims;
using Claims.Application.Customers;
using Claims.Application.Policies;
using Claims.Infrastructure.Persistence;
using Claims.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ClaimsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ClaimsDatabase")));

builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<ClaimService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<PolicyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
