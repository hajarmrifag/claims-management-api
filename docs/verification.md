# Verification and limits

Use .NET 10 and PostgreSQL 16. `CLAIMS_TEST_ADMIN_URL` is an Npgsql connection string for a local/test server with CREATEDB permission. Each relational test creates a fresh `claims_test_<random>` database and drops only that database. It does not reset demo data. A missing variable fails explicitly; relational tests are not silently skipped.

```sh
export CLAIMS_TEST_ADMIN_URL='Host=localhost;Port=5432;Username=YOUR_TEST_USER;Password=YOUR_TEST_PASSWORD;Database=postgres'
dotnet test --configuration Release
dotnet list package --vulnerable --include-transitive
```

For HTTP/domain work without a database, run the specific UnitTests and IntegrationTests projects. The solution and CI additionally run Claims.PostgresTests: unique-email enforcement, claim-parent FK enforcement and explicit transaction rollback after a unique violation. This uses the EF model/EnsureCreated rather than proving the deployment migration path. SQL Server behavior and concurrent status update races remain unverified.

Status inputs must be named, defined enum values (case-insensitive); arbitrary numeric/composite values are rejected before mutation/history. Any defined status can replace another; no lifecycle policy is invented. Repeating the same status creates no history.

The transitive `System.Security.Cryptography.Pkcs` dependency was pinned to 9.0.21, the patched 9.x version listed by [Microsoft's advisory](https://github.com/dotnet/runtime/security/advisories/GHSA-2fmv-rv2c-59x7), also covering the [second reported advisory](https://github.com/dotnet/runtime/security/advisories/GHSA-846f-f764-76mx). This addresses the observed package warnings, not a claim that an exploit was reachable in this API. Keep runtime/container servicing separate from package audits.

Frontend tests/build/Playwright remain part of existing CI; no frontend logic changed in this update. Local execution results are in `docs/verification-results.md` after the implementation run. CI YAML is prepared; it is not a completed remote run until the branch is authorized for publication.
