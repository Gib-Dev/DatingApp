# DatingApp

Full-stack social app with a **C# / ASP.NET Core (.NET 9) Web API** and an **Angular 21** front end: JWT authentication, member profiles with photo upload, likes and one-to-one messaging.

**Case study:** https://magib.tech/projects/dating-app

> **Origin:** built by following Neil Cummings' ASP.NET Core and Angular course on Udemy, then reviewed and extended on my own (see [What I added](#what-i-added)). Status: **in progress** (5 of 7 planned phases done, not deployed yet).

## Why this project

I wanted real experience with a typed back-end stack outside JavaScript (C#, ASP.NET Core, Entity Framework Core) and with Angular rather than React, on an application with authentication, relational data, file uploads and messaging.

## Features

- **Accounts**: registration and login, JWT bearer tokens sent by an Angular HTTP interceptor, an auth guard and an error interceptor
- **Members**: member list and profile pages, profile editing
- **Photos**: upload (stored on the API server for now), set a main photo, delete photos
- **Likes**: like a member, list likes
- **Messages**: send a message, inbox, conversation thread, delete

## What I added

On top of the course material:

- **Password hashing**: replaced HMACSHA512 with **PBKDF2** (600,000 iterations, constant-time comparison) behind an `IPasswordService`
- **Performance**: fixed an N+1 query when loading member photos
- **Deployment readiness**: environment-based API configuration instead of hardcoded URLs
- **Code review**: a written review of the codebase listing 34 issues (performance, security, architecture, code quality) that drives the next phases
- **CI**: GitHub Actions build workflow and CodeQL analysis

## Architecture

```
Angular 21 (zoneless, Tailwind CSS + DaisyUI)
  core/        guards, HTTP interceptors (JWT, errors), services
  features/    account, members, lists, messages
        │  HTTPS + JWT
        ▼
ASP.NET Core Web API (.NET 9)
  Controllers ── DTOs ── Services (token, password)
  Exception middleware (consistent error responses)
        │
        ▼
Entity Framework Core ── SQLite (code-first migrations)
Photo files ── API server storage (wwwroot/images)
```

### API endpoints

| Method | Route | Auth | Purpose |
| --- | --- | --- | --- |
| POST | `/api/account/register` | | Create an account |
| POST | `/api/account/login` | | Get a JWT |
| GET | `/api/members` | | List members |
| GET | `/api/members/{id}` | ✓ | Member profile |
| PUT | `/api/members` | ✓ | Update own profile |
| POST | `/api/members/add-photo` | ✓ | Upload a photo |
| PUT | `/api/members/set-main-photo/{photoId}` | ✓ | Set main photo |
| DELETE | `/api/members/delete-photo/{photoId}` | ✓ | Delete a photo |
| POST | `/api/likes/{likedUserId}` | ✓ | Like a member |
| GET | `/api/likes` | ✓ | List likes |
| POST | `/api/messages` | ✓ | Send a message |
| GET | `/api/messages` | ✓ | Inbox |
| GET | `/api/messages/thread/{userId}` | ✓ | Conversation thread |
| DELETE | `/api/messages/{id}` | ✓ | Delete a message |

### Data model

Entity Framework Core entities: `AppUser`, `Member`, `Photo`, `UserLike` (self-referencing many-to-many between members) and `Message`.

## Tech stack

**Back end:** C#, .NET 9, ASP.NET Core Web API, Entity Framework Core 9, SQLite, JWT bearer authentication
**Front end:** Angular 21 (zoneless change detection), TypeScript, Tailwind CSS 4, DaisyUI
**Tooling:** GitHub Actions, CodeQL

## Getting started

Requires the .NET 9 SDK, Node.js 18+, the Angular CLI and the EF Core tools (`dotnet tool install --global dotnet-ef`).

### API

```bash
cd API
cp appsettings.Development.example.json appsettings.Development.json   # set your own TokenKey
dotnet restore
dotnet ef database update
dotnet run          # https://localhost:5001
```

`appsettings.Development.json` is git-ignored: keep real keys out of the repository.

### Client

```bash
cd client
npm install
npm start           # https://localhost:4200
```

## Known limitations

- No automated tests yet (planned for the quality phase)
- Not deployed yet
- Photos are stored on the API server; cloud storage is not wired up yet
- Open items from the code review: missing database indexes, CORS too permissive for production, business logic still in some controllers

## Next steps

- Database indexes and stricter CORS
- Repository pattern to move logic out of controllers
- Unit tests for the API
- Cloud photo storage (Cloudinary)
- Deployment

## License

Educational project based on course material.
