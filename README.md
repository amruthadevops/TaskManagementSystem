
# 🚀 Task Management System

A full-stack role-based task management system built with .NET 8, React, TypeScript, and SQL Server.

## ✨ Features
### 🔐 Authentication & Authorization

- JWT-based authentication
- Role-based access control (Admin, Manager, User)
- Secure password hashing with BCrypt
- Session management with token expiration

### 📊 Dashboard

- Real-time task statistics
- Visual overview of task distribution
- Quick access to important metrics
- Role-based data filtering

### ✅ Task Management

- Create, read, update, and delete tasks
- Task status tracking (To Do, In Progress, Done)
- Priority levels (Low, Medium, High, Critical)
- Due date management
- Task assignment to team members
- Filter tasks by status, priority, and deadline

### 👥 Team Management

- Create and manage teams (Admin/Manager only)
- Add/remove team members
- View team statistics
- Manager assignment

### 💬 Collaboration

- Comment system on tasks
- Real-time notifications (mock)
- Task assignment notifications
- Status change notifications

### 🎨 User Interface

- Modern, responsive design
- Mobile-friendly
- Intuitive navigation
- Clean and professional UI with Tailwind CSS



## 🛠️ Tech Stack
### Backend

- Framework: ASP.NET Core 8 MVC
- ORM: Entity Framework Core 8
- Database: SQL Server 2022
- Authentication: JWT Bearer Tokens
- Architecture: Clean Architecture with Repository - Pattern
- API Documentation: Swagger/OpenAPI

### Frontend

- Framework: React 18.2 with TypeScript
- Build Tool: Vite
- Styling: Tailwind CSS
- HTTP Client: Axios
- Routing: React Router v6
- Icons: Lucide React
- Notifications: React Hot Toast
- Date Handling: date-fns




## 🏗️ Architecture
Backend Architecture (Clean Architecture)
```
┌─────────────────────────────────────────────────────┐
│                   API Layer                         │
│  Controllers, Middleware, Authentication            │
└─────────────────┬───────────────────────────────────┘
  │
┌─────────────────▼───────────────────────────────────┐
│              Application Layer                      │
│  Services, DTOs, Business Logic, Interfaces         │
└─────────────────┬───────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────┐
│                Domain Layer                         │
│  Entities, Value Objects, Domain Interfaces         │
└─────────────────────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────┐
│            Infrastructure Layer                     │
│  EF Core, Repositories, External Services           │
└─────────────────────────────────────────────────────┘
```
## 🚀 Quick Start
Prerequisites

- .NET 8 SDK
- Node.js 18+
- SQL Server

## Local Development

``` 
    # Navigate to API project
    cd TaskManagement.API

    # Restore packages
    dotnet restore

    # Update connection string in appsettings.json
    # Then run migrations
    dotnet ef migrations add InitialCreate --project ../TaskManagement.Infrastructure
    dotnet ef database update --project ../TaskManagement.Infrastructure

    # Run the API
    dotnet run
```

## Frontend Setup

```
# Navigate to frontend
cd task-management-frontend

# Install dependencies
npm install

# Create .env file
echo "VITE_API_URL=https://localhost:5001/api" > .env

# Run the app
npm run dev
```
##### Access the application at http://localhost:3000


## 📁 Project Structure
```
TaskManagementSystem/
│
├── TaskManagement.API/                 # Web API
│   ├── Controllers/                    # API Controllers
│   ├── Program.cs                      # App configuration
│   └── appsettings.json               # Configuration
│
├── TaskManagement.Application/         # Business Logic
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Services/                      # Business Services
│   └── Interfaces/                    # Service Contracts
│
├── TaskManagement.Core/               # Domain Layer
│   ├── Entities/                      # Domain Entities
│   └── Interfaces/                    # Repository Contracts
│
├── TaskManagement.Infrastructure/     # Data Access
│   ├── Data/                         # DbContext
│   └── Repositories/                 # Repository Implementation
│
├── TaskManagement.Tests/             # Unit Tests
│   ├── Services/                     # Service Tests
│   └── Controllers/                  # Controller Tests
│
├── task-management-frontend/         # React Frontend
│   ├── src/
│   │   ├── api/                     # API Services
│   │   ├── components/              # React Components
│   │   ├── pages/                   # Page Components
│   │   ├── context/                 # Context Providers
│   │   ├── types/                   # TypeScript Types
│   │   └── App.tsx                  # Root Component
│   └── package.json
│
└── docker-compose.yml                # Docker Compose Config

```
### Register
<img width="1360" height="675" alt="image" src="https://github.com/user-attachments/assets/ac2d8a36-e0c4-439d-adc8-037bc87443d4" />

### Logiin 
<img width="1364" height="674" alt="image" src="https://github.com/user-attachments/assets/df53d162-64e2-450c-bb9d-42200fe853de" />


###  Dashboard
<img width="1360" height="683" alt="Image" src="https://github.com/user-attachments/assets/11225581-d977-44a3-9deb-a0470a4dce2b" />

### Task Management
<img width="1361" height="678" alt="Image" src="https://github.com/user-attachments/assets/14db8816-c81a-4901-968f-43a6c7f8c9d1" />

### Team Management
<img width="1363" height="675" alt="Image" src="https://github.com/user-attachments/assets/dbb7b07c-cd31-4d9f-8f6e-c8e687721d12" />



## License
This project is licensed under the MIT License - see the LICENSE file for details.


## Authors

- [Amrutha](https://www.github.com/amruthadevops)

