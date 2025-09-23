# DatingApp – Project Guide

This guide explains the architecture, runtime flow, authentication, environments, testing, and production hardening. It complements the README and does not change any source code.

## 1) Architecture Overview

- Backend: `.NET 9` Web API (`API/`)
  - Controllers: HTTP endpoints (e.g., `AccountController`, `MembersController`).
  - Data: EF Core `AppDbContext`, migrations, DTOs.
  - Services: `TokenService` creates JWT tokens.
  - Entities: `AppUser` domain model.
- Frontend: `Angular 21` app (`client/`)
  - `src/app`: components, routes, bootstrap (`app.ts`, `app.routes.ts`, `app.config.ts`).
  - HTTP calls to the API using Angular HttpClient.
  - Styling with Tailwind + DaisyUI.

Runtime: Angular serves at `https://localhost:4200` and calls the API at `https://localhost:5001`.

## 2) Backend Runtime Flow

1. Startup (`API/Program.cs`): builds the host, registers services (DbContext, CORS, JWT auth, `TokenService`), and configures middleware.
2. Controllers
   - `AccountController`: registration and login. On successful login, a JWT is issued via `TokenService`.
   - `MembersController`: typically protected endpoints for member list/details (requires valid JWT).
3. Data access: EF Core with SQLite in development (`API/dating.db`). Migrations are tracked in `API/Data/Migrations`.

## 3) Frontend Runtime Flow

1. Bootstrap (`client/src/main.ts`) loads the Angular application.
2. Routing (`client/src/app/app.routes.ts`) defines navigation.
3. Components (`client/src/app/app.ts`, etc.) render UI and call the API via HttpClient.
4. Auth: after login, the client stores the JWT (commonly in memory or storage) and sends it in `Authorization: Bearer <token>` headers for protected API routes.

## 4) Authentication and Authorization

- Passwords: validated using HMACSHA512 with per-user salt in development code.
- Tokens: `TokenService` issues a signed JWT using a secret key from configuration.
- Requests: client sends the JWT with subsequent requests to access protected endpoints.
- Validation: API validates token signature and claims before authorizing.

Security notes:
- Do not commit real secrets. Use `API/appsettings.Development.example.json` as a template and keep developer-specific settings local.
- Prefer a modern password hashing algorithm (PBKDF2, bcrypt, scrypt, or Argon2) for production.

## 5) Configuration and Environments

- API configuration: `API/appsettings.json` and environment-specific files (e.g., `appsettings.Development.json`).
- Client configuration: ensure the API base URL is environment-driven in `client/src/app`.
- CORS: development is permissive for the Angular origin; restrict origins, headers, and methods in production.

## 6) Running Locally

Backend:
```
cd API
dotnet restore
dotnet ef database update
dotnet run
```
Frontend:
```
cd client
npm install
npm start
```
Open `https://localhost:4200` (client). The API runs on `https://localhost:5001`.

## 7) Extending the Project

- New API endpoints: add controller methods in `API/Controllers/*`, define DTOs under `API/DTOs`, and update `AppDbContext` if needed.
- Business logic/services: add under `API/Services/` and inject into controllers via DI.
- Data model changes: update `API/Entities/*`, add a migration (`dotnet ef migrations add <Name>`), then `dotnet ef database update`.
- Angular UI: add components and routes in `client/src/app`, and call the API via HttpClient.

## 8) Testing

- Backend: add unit tests for services (e.g., `TokenService`) and controller actions; consider integration tests with an in-memory provider or a test SQLite database.
- Frontend: add unit tests for services and components (`npm test`), and consider end-to-end tests.

## 9) Production Hardening

- Authentication: migrate password hashing to a modern KDF; consider refresh tokens if long-lived sessions are required.
- Authorization: ensure `[Authorize]` is applied to protected controllers/actions and that policies/roles are defined where applicable.
- Observability: add global exception handling, structured logging, and health checks.
- Security headers: enable HTTPS redirection, HSTS (production), and common headers (X-Content-Type-Options, X-Frame-Options, CSP where feasible).
- Rate limiting: apply to authentication endpoints to mitigate brute-force attempts.
- Database: use a production-grade database (e.g., Postgres or SQL Server) with connection resiliency; run migrations in CI/CD.

## 10) Troubleshooting

- CORS errors: confirm API allows the Angular dev origin and required headers.
- Invalid token: ensure the client sends `Authorization: Bearer <token>` and that the server `TokenKey` matches local config.
- Database issues: run migrations and verify the SQLite file path/permissions (development) or connection string (production).

## 11) Glossary

- DTO: Data Transfer Object used between API and client.
- Entity: Persistence model mapped by EF Core to the database.
- Migration: Versioned database schema change.
- JWT: JSON Web Token for stateless authentication.
