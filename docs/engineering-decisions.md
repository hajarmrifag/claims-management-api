# Engineering decisions

This document records the reasoning behind the project's main technical choices and is candid about their trade-offs.

## Server state: TanStack Query

Claims and authentication requests are remote state, not shared UI state. TanStack Query provides request deduplication, caching, mutation states, and explicit loading/error handling without a general-purpose global store. Local filter and pagination values stay in component state because unrelated features do not consume them.

**Trade-off:** query keys and invalidation need discipline as the application grows. A larger product would centralize key factories and cache policies.

## Forms: React Hook Form and Zod

React Hook Form limits unnecessary rerenders while Zod keeps validation typed and close to the client contract. Server validation remains authoritative; client validation provides fast, understandable feedback.

**Trade-off:** validation exists in both TypeScript and .NET. A generated API client or shared schema would reduce drift in a larger system.

## Authentication: JWT plus session storage

The API returns a short-lived JWT with role claims. The client keeps it in session storage so a tab refresh does not sign the user out, while closing the browser session clears it. Route protection improves navigation; the API remains the authorization boundary.

**Trade-off:** JavaScript-readable storage is exposed if an XSS vulnerability exists. A production browser application should consider secure `HttpOnly`, `SameSite` cookies, CSRF protection, and a refresh-token strategy.

## Layered backend

Domain rules, use cases, persistence, and HTTP concerns live in separate projects. Workflow rules can be tested without hosting the API, and storage implementations can change behind interfaces.

**Trade-off:** this has more ceremony than a small CRUD service needs. It is justified here by role-based workflows, audit history, and multiple infrastructure providers.

## One deployable container

The Docker build compiles the Vite application and places its static output in ASP.NET Core's `wwwroot`. Serving UI and API from one origin avoids production CORS configuration and keeps the demo simple to operate.

**Trade-off:** frontend and backend cannot scale or deploy independently. A higher-traffic system could use a CDN for static assets and operate the API separately.

## PostgreSQL on Render, SQL Server locally

EF Core uses PostgreSQL for the live free-tier deployment while retaining SQL Server locally. Domain and application layers do not depend on either provider.

**Trade-off:** providers can differ in SQL behavior. Provider-specific integration tests should be added before treating both as production targets.

## Testing strategy

Domain unit tests cover workflow rules. API integration tests cover authentication, authorization, customer-to-policy-to-claim flows, and documents. Frontend component tests cover critical rendering and failure states. CI also performs linting and production compilation.

**Trade-off:** the suite does not yet exercise the full system in a real browser. Playwright end-to-end tests are the next highest-value addition.
