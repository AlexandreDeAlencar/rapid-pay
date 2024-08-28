# RapidPay

Welcome to the RapidPay project! This guide will help you set up the project, configure your environment, and understand the project structure and requirements.

## Project Overview

RapidPay is a payment provider that requires the development of a new Authorization system. The project is divided into two main modules:

- **Card Management Module
- **Payment Fees Module

### Project Requirements

1. **Card Management Module:**
   - **Create Card**: API endpoint to create a card with a 15-digit card number.
   - **Pay**: API endpoint to process payments using the created card and update the balance.
   - **Get Card Balance**: API endpoint to retrieve the balance of a card.

2. **Payment Fees Module:**
   - The payment fee is calculated randomly and changes every hour. The fee is applied to each payment.

3. **Bonus (30K points):**
   - Improve API performance using multithreading.
   - Enhance the authentication system for better security.
   - Ensure shared resources are thread-safe.

## Project Structure

The project is structured into several projects, each with its own configuration and services:

- **RapidPay.CardManagement.Api**: The main web API project.
- **RapidPay.CardManagement.App**: Contains application logic for managing cards and payments.
- **RapidPay.CardManagement.Domain**: Defines domain entities and interfaces.
- **RapidPay.CardManagement.EntityFramework**: Handles database access and ORM configuration.
- **RapidPay.PaymentFees.BackgroundService**: Contains the background service for updating payment fees.

## Setup Instructions

### 1. Configure `appsettings.json`

The `appsettings.json` file contains the general configuration for the application, including logging and database connection strings.

Create an `appsettings.json` file in the root of the `RapidPay.CardManagement.Api` project with the following content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=your_database_name;Username=your_username;Password=your_password"
  },
  "AllowedHosts": "*"
}
```
      
### 2. Configure `appsettings.Development.json`

The `appsettings.Development.json` file is used for development-specific configurations, including JWT settings.

Create an `appsettings.Development.json` file in the root of the `RapidPay.CardManagement.Api` project with the following content:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "Issuer": "RapidPay",
    "Audience": "RapidPayServices",
    "SecretKey": "bRjk29nD8asH7diEfi0GsEdpGmP78sjR3h5Pjd93SlwW"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password;"
  }
}
```

### 3. Docker Setup

To containerize the application, you can use the provided Dockerfile. This Dockerfile is configured to build and run the application using Docker.

Here’s the Dockerfile setup:

```dockerfile
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RapidPay.CardManagement.Api/RapidPay.CardManagement.Api.csproj", "RapidPay.CardManagement.Api/"]
COPY ["RapidPay.CardManagement.App/RapidPay.CardManagement.App.csproj", "RapidPay.CardManagement.App/"]
COPY ["RapidPay.CardManagement.Domain/RapidPay.CardManagement.Domain.csproj", "RapidPay.CardManagement.Domain/"]
COPY ["RapidPay.CardManagement.EntityFramework/RapidPay.CardManagement.EntityFramework.csproj", "RapidPay.CardManagement.EntityFramework/"]
COPY ["RapidPay.PaymentFees.BackgroundService/RapidPay.PaymentFees.BackgroundService.csproj", "RapidPay.PaymentFees.BackgroundService/"]
RUN dotnet restore "./RapidPay.CardManagement.Api/RapidPay.CardManagement.Api.csproj"
COPY . .
WORKDIR "/src/RapidPay.CardManagement.Api"
RUN dotnet build "./RapidPay.CardManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RapidPay.CardManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RapidPay.CardManagement.Api.dll"]
```

### 4. Project Configuration

Each project within the solution has a static `ConfigureServices` method to register its dependencies.

#### Example: Registering the Background Service

For the `PaymentFeesBackgroundService`, the services are registered as follows:

```csharp
using Microsoft.Extensions.DependencyInjection;

namespace RapidPay.PaymentFees.BackgroundService;

public static class ServiceRegistration
{
    public static IServiceCollection AddPaymentFeesBackgroundService(this IServiceCollection services)
    {
        services.AddHostedService<FeeUpdateHostedService>();

        return services;
    }
}
```
This method ensures that the FeeUpdateHostedService is registered and ready to run as a background service when the application starts. Each project should have a similar static ConfigureServices method to manage its own dependencies.

### 5. Running the Project

Once you've configured the appsettings files and Docker, you can run the project by following these steps:

1. **Build the Docker Image**: Use the Dockerfile to build the image.
   - Run the following command in the terminal from the root of your project directory:
     ```bash
     docker build -t rapidpay-app .
     ```

2. **Run the Docker Container**: Start the container and ensure the necessary ports (8080 and 8081) are exposed.
   - Use the following command to run the container:
     ```bash
     docker run -d -p 8080:8080 -p 8081:8081 --name rapidpay-container rapidpay-app
     ```

3. **Access the API**: The API will be available at `http://localhost:8080` or `http://localhost:8081`.

   - You can test the API endpoints using tools like Postman or curl.

4. **Environment Variables**: If needed, you can override configuration values by setting environment variables when running the Docker container.

5. **Logging**: Logs will be available in the container’s console output. You can view the logs by running:
   ```bash
   docker logs rapidpay-container

### 6. API Endpoints

The API includes the following endpoints:

- **Create Card**: `/api/cards/create`
  - **Description**: Creates a new card with a 15-digit card number for a user.
  - **Method**: `POST`
  - **Request Body**: 
    ```json
    {
      "userName": "string",
      "userId": "string"
    }
    ```
  - **Response**: 
    - `201 Created`: When the card is successfully created.
    - `400 Bad Request`: If the request data is invalid.

- **Pay**: `/api/cards/payment`
  - **Description**: Processes a payment using the created card and updates the card balance.
  - **Method**: `POST`
  - **Request Body**: 
    ```json
    {
      "cardId": "string (GUID)",
      "value": "decimal"
    }
    ```
  - **Response**: 
    - `200 OK`: When the payment is successfully processed.
    - `400 Bad Request`: If the request data is invalid.
    - `404 Not Found`: If the card is not found.

- **Get Card Balance**: `/api/cards/{cardId}/balance`
  - **Description**: Retrieves the current balance of a specified card.
  - **Method**: `GET`
  - **Route Parameter**:
    - `cardId`: The GUID of the card whose balance is to be retrieved.
  - **Response**: 
    - `200 OK`: Returns the balance of the card.
    - `404 Not Found`: If the card is not found.

### Example API Requests

Here are some example requests using `curl`:

1. **Create Card**:
   
   ```bash
   curl -X POST http://localhost:8080/api/cards/create \
   -H "Content-Type: application/json" \
   -d '{"userName":"JohnDoe", "userId":"User123"}'
    ```
   
2. **Pay**:
 ```bash
   curl -X POST http://localhost:8080/api/cards/create \
   -H "Content-Type: application/json" \
   -d '{"userName":"JohnDoe", "userId":"User123"}'
  ```

3. **Get Card Balance**:
 ```bash
   curl http://localhost:8080/api/cards/<your-card-id>/balance
  ```

### 7. Testing and Validation
### 7. Testing and Validation

You can test the API using tools like Postman or curl by sending requests to the above endpoints. Ensure your appsettings are correctly configured and that the Docker container is running.

#### Default Seed Data and Authentication

The project includes default seed data for testing authentication. The `UserAuthContext` is configured to seed the database with some initial users and roles:

- **Roles:**
  - `Admin`
  - `User`

- **Default Users:**
  - **Username:** `testuser1`
    - **Password:** `Password123!`
    - **Email:** `testuser1@example.com`
  - **Username:** `testuser2`
    - **Password:** `Password123!`
    - **Email:** `testuser2@example.com`

The seeded users can be used to test the authentication system.

#### Login Endpoint

The `/api/login` endpoint is used to authenticate users and generate JWT tokens.

**Login Example:**

```bash
curl -X POST http://localhost:8080/api/login \
-H "Content-Type: application/json" \
-d '{"username":"testuser1", "password":"Password123!"}'
```
# Response:

On a successful login, you will receive a JWT token in the response:

```json
{
  "token": "your_jwt_token_here"
}
```
This token can be used in the Authorization header for subsequent requests to secured endpoints.

## Example of Using the Token:

```bash
curl -H "Authorization: Bearer your_jwt_token_here" \
http://localhost:8080/api/cards/create \
-H "Content-Type: application/json" \
-d '{"userName":"JohnDoe", "userId":"User123"}'
```
Make sure to replace your_jwt_token_here with the actual token received from the login endpoint.


