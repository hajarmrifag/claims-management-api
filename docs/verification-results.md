# Local verification — 16 October 2026

.NET SDK 10.0.401, macOS arm64, PostgreSQL 16.15. Restore/build succeeded with zero warnings/errors after the Pkcs 9.0.21 pin. **22 tests passed, zero failed/skipped:** 11 domain/application, 8 HTTP integration, 3 real PostgreSQL tests (unique constraint, foreign key, transaction rollback). The PostgreSQL tests create disposable databases; no existing data was modified.

`dotnet list package --vulnerable --include-transitive --format json` reported no vulnerable packages in the restored graph on this date. This is a package-advisory check, not an assessment of exploitability or a security certification. The SDK/runtime and deployed containers must still be serviced independently.

The PostgreSQL tests use EF EnsureCreated, not a migration rehearsal. HTTP tests remain EF InMemory; concurrent status updates, tenant isolation, full lifecycle rules and production operations are not established. Existing frontend source was unchanged. CI now provisions PostgreSQL for the solution tests; its remote result is distinct from this local run.
