# TM_API

## Description

REST API for managing tasks with user registration and authentication.  
Implements CRUD operations for tasks with secure, user-specific access control and persistent storage using a relational database.  
Focus on clean backend architecture, maintainable code, and structured development practices.

---

## Tech Stack

- C#
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- xUnit
- JWT (JSON Web Tokens)

---

## Features

- User registration and login
- Secure authentication using JWT
- Stateless user identification via Bearer tokens
- Task management (CRUD)
- Authorization: users can only access their own tasks
- Input validation
- Prevention of duplicate users
- Data persistence with PostgreSQL
- Unit tests for business logic

---

## Authentication

This API uses JWT-based authentication.

### Login

POST /api/users/login

Response:

{
  "id": "user-id",
  "email": "user@example.com",
  "token": "JWT_TOKEN"
}

---

### Using the Token

Include the token in requests:

Authorization: Bearer YOUR_TOKEN

---

## Getting Started

### Prerequisites

- .NET SDK
- PostgreSQL

---

### Setup

1. Configure your database connection in `appsettings.json`:

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=tmapi_db;Username=postgres;Password=YOUR_PASSWORD"
}

2. Apply migrations:

dotnet ef database update

3. Run the application:

dotnet run

---

## Testing

Run unit tests:

dotnet test

---

## Project Structure

- Controllers → API endpoints  
- Services → business logic  
- Data → database context  
- DTOs → request/response models  
- Models → domain entities  
