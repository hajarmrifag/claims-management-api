# Application security assessment

Assessment date: 14 September 2026  
Scope: source-assisted review of the ASP.NET Core API, React client, document
handling, authentication flow, and GitHub Actions configuration

## Method

This initial assessment combined manual data-flow review, abuse-case analysis,
and inspection of security-sensitive code. Findings are mapped to CWE and the
OWASP Top 10 where a useful mapping exists. CodeQL, Trivy, and OWASP ZAP workflows
have been added; their generated reports are CI artifacts and GitHub code-scanning
results, not fabricated in this document. The workflow is intentionally honest
about which evidence has and has not run.

Severity considers likely impact and exploitability in this demonstration's
documented single-organization, synthetic-data scope.

## Findings summary

| ID | Severity | Finding | CWE / OWASP | Status |
|---|---|---|---|---|
| APPSEC-001 | High | Uploaded content trusted the client-declared MIME type | CWE-434 / A04 Insecure Design | Remediated |
| APPSEC-002 | Medium | Authentication endpoints lacked abuse throttling | CWE-307 / A07 Identification and Authentication Failures | Remediated |
| APPSEC-003 | Medium | JWT is readable by browser JavaScript | CWE-922 / A07 Identification and Authentication Failures | Accepted for demo; production change required |
| APPSEC-004 | Context-dependent | API uses shared-workspace authorization rather than resource ownership | CWE-639 / A01 Broken Access Control | Accepted within current scope |
| APPSEC-005 | Low | Browser defence-in-depth headers were incomplete | CWE-693 / A05 Security Misconfiguration | Remediated |
| APPSEC-006 | Medium | CI did not perform static, dependency, secret, container, or dynamic scanning | A06 Vulnerable and Outdated Components | Remediated in configuration; first CI results pending |
| APPSEC-007 | Medium | Document uploads are not malware scanned | CWE-434 / A04 Insecure Design | Planned before real-data use |
| APPSEC-008 | High | Production container executed as the root user | CWE-250 / A05 Security Misconfiguration | Remediated after first Trivy run |

## Detailed findings

### APPSEC-001: Content-type spoofing in document uploads

**Observation.** The upload endpoint allowed PDF, JPEG, and PNG MIME values, but
the MIME value came from the multipart request. A non-image or non-PDF body could
therefore be stored with an allowed declaration.

**Impact.** The risk depends on downstream processing. Serving attacker-controlled
content, parsing it with a vulnerable library, or passing it to another user could
turn a weak validation decision into code execution or content-sniffing exposure.

**Remediation.** The endpoint now checks the file's magic-byte signature against
its declared type, normalizes the original filename, and retains the size and MIME
allowlists. A negative integration test submits fake PDF content and verifies a
400 response.

**Residual risk.** Signatures establish format, not safety. Production handling
would also require malware scanning, protected object storage, authorization on
every retrieval, and safe rendering/download behavior.

### APPSEC-002: Authentication abuse was not throttled

**Observation.** Registration and login could be called repeatedly without an
application-level limit.

**Impact.** Attackers could perform password guessing or consume application and
database resources.

**Remediation.** Both endpoints now share a fixed-window limit of five requests
per source address per minute and return HTTP 429 when exhausted. An integration
test verifies the control.

**Residual risk.** A real deployment should combine application limits with
platform-level controls, alerting, adaptive rules, and carefully designed account
lockout.

### APPSEC-003: JWT available to JavaScript

**Observation.** The React client stores its short-lived access token in session
storage. The existing engineering decision record already acknowledges that an
XSS flaw could expose it.

**Decision.** Accepted for the disposable portfolio demo. Before handling real
data, use a Secure, HttpOnly, SameSite cookie strategy, implement CSRF protection,
and test login, logout, expiry, and refresh behavior.

### APPSEC-004: Shared-workspace authorization model

**Observation.** Authentication protects claims and document routes, while role
authorization protects status changes. Authenticated Adjusters share the same
queue; claims do not carry a tenant or owner authorization attribute.

**Decision.** This matches the documented internal single-organization scenario.
It would become a broken object-level authorization risk if customers, external
partners, or multiple organizations used the API.

**Required future remediation.** Add an organization identifier and explicit
resource-authorization policies, scope every query server-side, and add negative
tests proving cross-tenant identifiers cannot retrieve or mutate resources.

### APPSEC-005: Missing defence-in-depth response headers

**Observation.** The API redirected to HTTPS but did not consistently send a CSP,
anti-sniffing, referrer, permissions, and production HSTS policies.

**Remediation.** Responses now include a restrictive CSP, `X-Content-Type-Options`,
`Referrer-Policy`, and `Permissions-Policy`; production enables HSTS. An integration
test verifies representative headers.

### APPSEC-006: Security analysis absent from CI

**Observation.** CI compiled, linted, and tested the application but did not run
security-specific analysis.

**Remediation.** The repository now configures:

- CodeQL extended security queries for C# and JavaScript/TypeScript
- Trivy repository scanning for dependencies, secrets, and misconfiguration
- Trivy scanning of the production container image
- OWASP ZAP passive baseline scanning against an ephemeral deployment
- Dependabot updates for NuGet, npm, and GitHub Actions

**Verification.** Review the first GitHub workflow run and triage every result.
ZAP is initially non-blocking so its baseline can be reviewed without disguising
alerts as build failures; the intended next step is to make agreed high-confidence
rules blocking.

### APPSEC-007: No malware inspection of valid document formats

**Observation.** A file with a valid allowed signature can still contain malicious
content.

**Decision.** Do not accept real documents in this portfolio deployment. Before
production use, quarantine uploads, scan them asynchronously, use private storage,
serve safe derivatives where appropriate, and release only clean objects.

### APPSEC-008: Production container executed as root

**Observation.** The first Trivy repository scan failed rule DS-0002 because the
runtime Docker stage had no `USER` instruction and therefore executed as root.

**Impact.** Root execution unnecessarily increases the impact of an application
or container-runtime compromise.

**Remediation.** The image now prepares its writable upload directory during the
build, assigns it to the built-in unprivileged .NET account, and switches to
`USER $APP_UID` before starting the API. This finding was fixed rather than
ignored, preserving the failed scan as evidence of the remediation workflow.

## Verification checklist

- [x] Unit and integration tests pass after remediation
- [x] Spoofed document content has a negative regression test
- [x] Authentication throttling has a regression test
- [x] Security headers have a regression test
- [x] NuGet advisory check reports no vulnerable direct or transitive packages
- [x] npm production dependency audit reports zero vulnerabilities
- [x] First Trivy run identified root container execution and the Dockerfile was remediated
- [ ] Review the first CodeQL result set
- [ ] Review and classify Trivy dependency, secret, configuration, and image results
- [ ] Review the first ZAP HTML and JSON artifacts
- [ ] Convert agreed high-confidence ZAP findings into a blocking policy
- [ ] Reassess after any authentication or authorization redesign

## Limitations

This is a portfolio assessment, not a guarantee of security. It did not include
destructive testing, authenticated ZAP crawling, infrastructure penetration
testing, source-host configuration review, or third-party cloud configuration.
