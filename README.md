# DatingApp

A full-stack dating application built with .NET 9 Web API backend and Angular 21 frontend.

## Project Overview

This is a modern dating application featuring user registration, authentication, and member management. The application uses a clean architecture with separate backend and frontend projects.

## Architecture

### Backend (.NET 9 Web API)
- **Framework**: .NET 9.0
- **Database**: SQLite with Entity Framework Core
- **Authentication**: Custom password hashing with HMACSHA512
- **CORS**: Configured for Angular development server

### Frontend (Angular 21)
- **Framework**: Angular 21 (next version)
- **Styling**: Tailwind CSS with DaisyUI components
- **Build Tool**: Angular CLI with zoneless change detection
- **HTTP Client**: Built-in Angular HttpClient for API communication

## Features

### Backend Features
- User registration with email validation
- User log in with secure password authentication
- Member listing and individual member retrieval
- SQLite database with Entity Framework Core migrations
- CORS configuration for frontend integration

### Frontend Features
- Modern UI with Tailwind CSS and DaisyUI
- Member listing display
- Responsive design
- HTTPS support for development

## Project Structure

```
DatingApp/
├── API/                          # .NET 9 Web API Backend
│   ├── Controllers/              # API Controllers
│   │   ├── AccountController.cs  # Authentication endpoints
│   │   ├── MembersController.cs  # Member management
│   │   └── BaseApiController.cs  # Base controller
│   ├── Data/                     # Data layer
│   │   ├── AppDbContext.cs       # Entity Framework context
│   │   ├── LoginDto.cs           # Login data transfer object
│   │   └── Migrations/           # Database migrations
│   ├── DTOs/                     # Data transfer objects
│   │   └── RegistrerDto.cs       # Registration DTO (filename may be corrected to RegisterDto.cs)
│   ├── Entities/                 # Domain entities
│   │   └── AppUser.cs            # User entity
│   ├── dating.db                 # SQLite database
│   └── Program.cs                # Application entry point
├── client/                       # Angular 21 Frontend
│   ├── src/
│   │   ├── app/                  # Angular application
│   │   │   ├── app.ts            # Main component
│   │   │   ├── app.html          # Main template
│   │   │   ├── app.css           # Component styles
│   │   │   ├── app.routes.ts     # Routing configuration
│   │   │   └── app.config.ts     # Application configuration
│   │   ├── styles.css            # Global styles
│   │   └── main.ts               # Application bootstrap
│   ├── public/                   # Static assets
│   └── ssl/                      # SSL certificates for development
└── DatingApp.sln                 # Visual Studio solution file
```

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js (v18 or higher)
- Angular CLI (v21)
- EF Core CLI tool (for migrations):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### Backend Setup

1. Navigate to the API directory:
   ```bash
   cd API
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Update the database:
   ```bash
   dotnet ef database update
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:5001`.

### Frontend Setup

1. Navigate to the client directory:
   ```bash
   cd client
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Run the development server:
   ```bash
   npm start
   ```

The application will be available at `https://localhost:4200`.

## API Endpoints

### Authentication
- `POST /api/account/register` - Register a new user
- `POST /api/account/login` - Log in user

### Members
- `GET /api/members` - Get all members
- `GET /api/members/{id}` - Get member by ID

## Database Schema

### AppUser Entity
- `Id` (string) - Unique identifier
- `DisplayName` (string) - User's display name
- `Email` (string) - User's email address
- `PasswordHash` (byte[]) - Hashed password
- `PasswordSalt` (byte[]) - Password salt for hashing

## Development Notes

- The application uses SQLite for the development database
- Password hashing is implemented using HMACSHA512
- CORS is configured to allow requests from Angular development server
- The frontend uses Tailwind CSS with DaisyUI for styling
- SSL certificates are included for HTTPS development

## Security Notes

- Secrets: never commit real secrets. Keep `appsettings.Development.json` local; use `appsettings.Development.example.json` as a template.
- JWT signing key: store securely via environment variables or a secrets provider in production.
- Password hashing: for production, prefer PBKDF2, bcrypt, scrypt, or Argon2 instead of raw HMACSHA512.
- CORS: restrict origins, headers, and methods in production.
- HTTPS: enforce HTTPS redirection and enable HSTS in production.
- Rate limiting: apply on authentication endpoints to mitigate brute-force attacks.

## Technologies Used

### Backend
- .NET 9.0
- Entity Framework Core 9.0.8
- SQLite
- ASP.NET Core Web API

### Frontend
- Angular 21.0.0-next.0
- Tailwind CSS 4.1.12
- DaisyUI 5.0.54
- TypeScript 5.9.2

## License

This project is for educational purposes.

## Demo

- Live demo: add link here when deployed
- API base URL (local): `https://localhost:5001`
- Client URL (local): `https://localhost:4200`

## Screenshots

Place screenshots in `client/public/` and reference them here:

- Landing page: `client/public/landing.png`
- Members list: `client/public/members.png`
- Messaging: `client/public/messages.png`

## Environment Configuration

- Backend (`API/appsettings.Development.json`): set connection strings and JWT config.
- Frontend (`client/src/app`): update API base URL used by the HTTP client if applicable.
- Secrets: never commit real secrets. Use user secrets or environment variables for production.

## Development Workflow

1. Start API: `cd API && dotnet run`
2. Start Client: `cd client && npm start`
3. Browse client at `https://localhost:4200` (API must be running for data).

### Common Tasks

- Run Angular unit tests: `cd client && npm test`
- Add EF Core migration: `cd API && dotnet ef migrations add <Name>`
- Update database: `cd API && dotnet ef database update`

## Testing

- Backend: add unit tests for services (e.g., token issuance) and controller actions; consider integration tests against a test database.
- Frontend: run unit tests with `npm test` and consider end-to-end tests for critical flows.

## Deployment

### API (Azure App Service)

- Publish profile from Visual Studio or `dotnet publish -c Release` and deploy artifact.
- Set environment variables (e.g., connection strings, JWT options) in Azure.

### Client (Static Hosting)

- Build: `cd client && npm run build` (outputs to `client/dist`).
- Deploy `dist` output to a static host (Azure Static Web Apps, Netlify, Vercel, S3+CloudFront).
- Ensure the client points to the public API URL.

## Configuration and Environments

- Backend configuration lives in `API/appsettings*.json`. Use environment variables or managed secrets in production.
- Frontend should reference the API base URL from environment configuration and avoid hardcoding URLs.

## Roadmap

- Photo upload and gallery
- Private messaging
- Filtering, sorting, and paging
- JWT authentication and guards
- Real-time presence and notifications with SignalR

## How to Use This in a Portfolio

- Summarize the problem: modern full-stack app with auth, data, and realtime.
- Highlight your role: API design, Angular UI, security, and deployment.
- Link to the live demo and include 2–3 screenshots.
- Brief bullet points of technical highlights (see Roadmap and Technologies Used).

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
