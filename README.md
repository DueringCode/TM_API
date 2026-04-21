# TM_API

## Description

REST API for managing tasks with user registration and login functionality.  
Includes CRUD operations for tasks and persistent data storage using a relational database.  
Focus on clean backend architecture, maintainable code, and structured development practices.

---

## Tech Stack

- C#
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- xUnit

---

## Features

- User registration
- Basic login logic (in progress / planned)
- Task management (CRUD)
- Input validation
- Prevention of duplicate users
- Data persistence with PostgreSQL

---

## Getting Started

### Prerequisites

- .NET SDK
- PostgreSQL

### Setup

1. Configure your database connection in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=tmapi_db;Username=postgres;Password=YOUR_PASSWORD"
}
