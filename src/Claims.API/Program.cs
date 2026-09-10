using Azure.Storage.Blobs;
using System.Text;
using Claims.API.Auth;
using Claims.API.Exceptions;
using Claims.API.Middleware;
using Claims.Application.Auth;
using Claims.Application.Claims;
using Claims.Application.Customers;
using Claims.Application.Documents;
using Claims.Application.Policies;
using Claims.Infrastructure.Auth;
using Claims.Infrastructure.Documents;
using Claims.Infrastructure.Persistence;
using Claims.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ClaimsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ClaimsDatabase")));

builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<ClaimService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CustomerService>();

builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<PolicyService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<AuthService>();
var storageProvider =
    builder.Configuration["Storage:Provider"] ?? "Local";

if (storageProvider.Equals(
        "AzureBlob",
        StringComparison.OrdinalIgnoreCase))
{
    var blobConnectionString =
        builder.Configuration["Storage:ConnectionString"]
        ?? throw new InvalidOperationException(
            "Azure Blob Storage connection string is not configured.");

    var containerName =
        builder.Configuration["Storage:ContainerName"]
        ?? "claim-documents";

    builder.Services.AddSingleton(
        new BlobContainerClient(
            blobConnectionString,
            containerName));

    builder.Services.AddScoped<IFileStorage, AzureBlobFileStorage>();
}
else
{
    builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
}

builder.Services.AddScoped<ClaimDocumentService>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT signing key is not configured.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? "ClaimsManagementApi";

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? "ClaimsManagementClient";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
