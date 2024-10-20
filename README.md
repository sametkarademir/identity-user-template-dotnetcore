# Identity User Service

This project is a .NET Core API for user and role management, built using Microsoft Identity. It provides a comprehensive system for handling user authentication, authorization, and user roles. PostgreSQL is used as the database, and Entity Framework Core is utilized for database operations. The application is containerized using Docker and orchestrated via Docker Compose.

## Features

- **User Authentication**: Secure login with JWT-based authentication.
- **User Role Management**: Manage user roles and permissions using Microsoft Identity.
- **Email Confirmation**: Built-in support for email confirmation.
- **Password Management**: Password reset functionality via email.
- **RabbitMQ Integration**: Message broker for handling background tasks and notifications.
- **Feature Toggles**: Easily enable or disable features such as registration and email confirmation.

## Technologies

- **.NET Core 8**
- **Microsoft Identity**
- **Entity Framework Core**
- **PostgreSQL**
- **Docker & Docker Compose**
- **RabbitMQ**

## Prerequisites

- Docker
- Docker Compose

## Getting Started

To run this project locally, follow the steps below.

### 1. Clone the repository

```bash
git clone https://github.com/sametkarademir/identity-user-template-dotnetcore.git
cd identity-user-template-dotnetcoreg
```

### 2. Build and run the Docker containers

Ensure that Docker and Docker Compose are installed on your machine. Then, use the following command to build and start the containers:

```bash
docker-compose up --build
```

### 3. Access the application

Once the containers are running, you can access the API on the following ports:

* API: http://localhost:5000
* Swagger UI: http://localhost:5001/swagger

## 4. Environment Variables

The following environment variables are used in the project and are pre-configured in the docker-compose.yml file:

* ConnectionStrings__DefaultConnection: Database connection string.
* JWT__Key: Secret key for JWT authentication.
* Email__Configuration: SMTP server details for email notifications.
* RabbitMq__Host: RabbitMQ settings for message brokering.

# Docker Compose Configuration

The application uses two services in Docker Compose:

* Web API: Hosts the .NET Core API service.
* PostgreSQL: The database service, storing user and role information.

```yaml
services:
  s_identity_user_webapi:
    image: i_identity_user_webapi
    container_name: c_identity_user_webapi
    depends_on:
      - c_identity_user_postgresqldb
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=c_identity_user_postgresqldb;Port=5432;Database=identity-user-service-db;User Id=admin;Password=P@ssw0rd;
      - Identity__AccessFailedCount=5
      - JWT__Key=your_jwt_secret_key
      - RabbitMq__Host=localhost
    ports:
      - "5000:8080"
      - "5001:8081"
    build:
      context: .
      dockerfile: src/WebApi/Dockerfile 
  s_identity_user_postgresqldb:
    container_name: c_identity_user_postgresqldb
    image: postgres:14.1-alpine
    restart: always
    environment:
      - POSTGRES_PASSWORD=P@ssw0rd
    ports:
      - '5432:5432'
    volumes:
      - data:/var/lib/postgresql/data

volumes:
  data:
```

