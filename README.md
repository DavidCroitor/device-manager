# Device Management System

Device Management System is a full-stack web application designed for managing devices, featuring built-in user authentication and AI-powered device description generation. The project is structured with a Clean Architecture backend and an Angular frontend.

## Tech Stack

**Backend (app/)**
- .NET / C#
- ASP.NET Core 
- JWT Authentication
- Clean Architecture (Domain, Application, Infrastructure, Presentation)

**Frontend (client/)**
- Angular
- TypeScript
- HTML / CSS

## Features

- **User Authentication:** Secure user registration, login, and token-based authentication (JWT).
- **Device Management:** Create, read, update, and manage devices within the system.
- **AI-Powered Descriptions:** Automatically generate descriptive details for devices using AI integrations.
- **Automated Testing:** Integration tests configured for user and device endpoints.

## Installation and Running Guide

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (latest version supported by the project)
- [Node.js and npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli)

### Setting up the Backend
1. Open a terminal and navigate to the backend directory:
   ```bash
   cd app
   ```
2. Restore .NET dependencies:
   ```bash
   dotnet restore
   ```
3. Configure your JWT settings and AI API key using the .NET Secret Manager from the `Presentation` directory:
   ```bash
   cd Presentation
   dotnet user-secrets set "Jwt:Key" "Your_Super_Secret_Key"
   dotnet user-secrets set "Jwt:Audience" "Audience"
   dotnet user-secrets set "Jwt:Issuer" "Issuer"
   dotnet user-secrets set "Jwt:ExpirationInMinutes" "120"
   dotnet user-secrets set "GroqApiKey" "Your_AI_API_Key"
   cd ..
   ```
4. Run the API:
   ```bash
   dotnet run --project Presentation/Presentation.csproj
   ```
   The backend will start and typically listen on ports `http://localhost:5000` or `https://localhost:5001`.

### Setting up the Frontend
1. Open a new terminal instance and navigate to the client directory:
   ```bash
   cd client
   ```
2. Install npm dependencies:
   ```bash
   npm install
   ```
3. Start the Angular development server:
   ```bash
   npm start
   ```
4. Access the application by opening your browser to `http://localhost:4200/`.
