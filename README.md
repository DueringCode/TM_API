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
- Docker
- Docker Compose

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
- Containerized application setup with Docker

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

## Run with Docker (Recommended)

### Prerequisites

- Docker Desktop

### Start application

```bash
docker compose up --build
