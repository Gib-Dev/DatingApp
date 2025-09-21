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
- User login with secure password authentication
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
│   │   └── RegistrerDto.cs       # Registration DTO
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

The API will be available at `https://localhost:5001`

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

The application will be available at `https://localhost:4200`

## API Endpoints

### Authentication
- `POST /api/account/register` - Register a new user
- `POST /api/account/login` - Login user

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

- The application uses SQLite for development database
- Password hashing is implemented using HMACSHA512
- CORS is configured to allow requests from Angular development server
- The frontend uses Tailwind CSS with DaisyUI for styling
- SSL certificates are included for HTTPS development

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
