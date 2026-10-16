using Claims.Domain.Entities;
using Claims.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Claims.PostgresTests;

// Each test owns a random new database. No existing database is cleared or migrated.
public class RelationalConstraintTests : IAsyncLifetime
{
    private readonly string _name = "claims_test_" + Guid.NewGuid().ToString("N");
    private string _admin = "";
    private string _connection = "";
    private bool _created;
    public async Task InitializeAsync()
    {
        _admin = Environment.GetEnvironmentVariable("CLAIMS_TEST_ADMIN_URL")
            ?? throw new InvalidOperationException("Set CLAIMS_TEST_ADMIN_URL to disposable PostgreSQL with CREATEDB permission.");
        await using var connection = new NpgsqlConnection(_admin);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"CREATE DATABASE {_name}", connection);
        await command.ExecuteNonQueryAsync();
        _created = true;
        _connection = new NpgsqlConnectionStringBuilder(_admin) { Database = _name }.ConnectionString;
        await using var db = Open();
        await db.Database.EnsureCreatedAsync();
    }
    private ClaimsDbContext Open() => new(new DbContextOptionsBuilder<ClaimsDbContext>().UseNpgsql(_connection).Options);
    public async Task DisposeAsync()
    {
        if (!_created) return;
        NpgsqlConnection.ClearAllPools();
        await using var connection = new NpgsqlConnection(_admin);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"DROP DATABASE {_name} WITH (FORCE)", connection);
        await command.ExecuteNonQueryAsync();
    }
    private static Customer Customer(string email) => new("Test", "Customer", email, null);
    [Fact]
    public async Task DuplicateEmail_IsRejectedByPostgresUniqueConstraint()
    {
        await using var db = Open();
        db.Customers.Add(Customer("duplicate@example.test")); await db.SaveChangesAsync();
        db.Customers.Add(Customer("duplicate@example.test"));
        var error = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.Equal(PostgresErrorCodes.UniqueViolation, Assert.IsType<PostgresException>(error.InnerException).SqlState);
    }
    [Fact]
    public async Task MissingClaimParents_AreRejectedByPostgresForeignKey()
    {
        await using var db = Open();
        db.Claims.Add(new Claim("CLM-orphan", Guid.NewGuid(), Guid.NewGuid(), "Test", 1m));
        var error = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, Assert.IsType<PostgresException>(error.InnerException).SqlState);
    }
    [Fact]
    public async Task ExplicitRollback_RemovesEarlierWriteAfterConstraintFailure()
    {
        await using (var db = Open())
        {
            await using var transaction = await db.Database.BeginTransactionAsync();
            db.Customers.Add(Customer("rollback@example.test")); await db.SaveChangesAsync();
            db.Customers.Add(Customer("rollback@example.test"));
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
            await transaction.RollbackAsync();
        }
        await using var check = Open();
        Assert.Equal(0, await check.Customers.CountAsync());
    }
}
