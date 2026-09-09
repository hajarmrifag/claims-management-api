using Claims.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Claims.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ClaimsDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ClaimsDbContext>>();

            services.AddDbContext<ClaimsDbContext>(options =>
                options.UseInMemoryDatabase(
                    $"ClaimsIntegrationTests-{Guid.NewGuid()}"));
        });
    }
}
