# XpressShip

XpressShip is a comprehensive shipping management solution built using **ASP.NET Core Minimal API**. It follows a **layered architecture** to ensure a clear separation of concerns and a scalable codebase.

## Features

- **Minimal API with ASP.NET Core**
- **Layered Architecture** for better separation of concerns
- **Stripe Payment Integration** with Webhook Support
- **In-Memory Caching** for performance optimization
- **Background Services** for efficient task management
- **Secure Authentication Mechanisms**
- **Robust Data Access Layer** using Entity Framework Core
- **Unit Testing and Common Testing Utilities** to ensure reliability

## Project Structure

```
XpressShip (Solution)
│
├── src
│   ├── XpressShip.API            # Entry point for the API
│   ├── XpressShip.Application    # Business logic layer
│   ├── XpressShip.Domain         # Core domain models and logic
│   ├── XpressShip.Infrastructure # Data access, repositories, external integrations
│
├── tests
│   ├── Unit                      # Unit tests
│   ├── XpressShip.Tests.Common   # Common utilities for testing
```

## Technologies Used

- **ASP.NET Core Minimal API**
- **Entity Framework Core**
- **Stripe API for Payments**
- **In-Memory Caching**
- **Background Services in .NET**
- **JWT-based Authentication**
- **xUnit for Unit Testing**

## Getting Started

### Prerequisites
- .NET 9 or later
- SQL Server or an alternative database supported by Entity Framework Core
- Stripe API Keys (for payment integration)

### Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/XpressShip.git
   cd XpressShip
   ```

2. Restore dependencies:
   ```sh
   dotnet restore
   ```

3. Apply database migrations:
   ```sh
   dotnet ef database update
   ```

4. Run the application:
   ```sh
   dotnet run --project src/XpressShip.API
   ```

### Running Tests
To execute unit tests, run:
```sh
cd tests/Unit
 dotnet test
```

## Required Secrets and Configuration
Users contributing to this project must set up the following secrets and environment variables:

- **Database Connection String** (`ConnectionStrings:Default`)
- **JWT Security Key** (`Token:Access:SecurityKey`)
- **GeoCode API Key** (`API:GeoCodeAPI:ApiKey`)
- **Email Configuration (SMTP Credentials)**
  - `EmailConfiguration:From`
  - `EmailConfiguration:SmtpServer`
  - `EmailConfiguration:Port`
  - `EmailConfiguration:Username`
  - `EmailConfiguration:Password`
- **Stripe Payment Secrets**
  - `PaymentGateways:Stripe:SecretKey`
  - `PaymentGateways:Stripe:WebhookSecret`

### Setting Up Secrets (For Development)
To securely store these values, use **.NET User Secrets**:
```sh
 dotnet user-secrets init
 dotnet user-secrets set "ConnectionStrings:Default" "your-secure-connection-string"
 dotnet user-secrets set "Token:Access:SecurityKey" "your-secure-access-key"
 dotnet user-secrets set "API:GeoCodeAPI:ApiKey" "your-secure-api-key"
 dotnet user-secrets set "EmailConfiguration:Password" "your-email-password"
 dotnet user-secrets set "PaymentGateways:Stripe:SecretKey" "your-stripe-secret-key"
 dotnet user-secrets set "PaymentGateways:Stripe:WebhookSecret" "your-stripe-webhook-secret"
```

For **production**, use environment variables instead.

## Contributing
Contributions are welcome! However, **feature additions are not allowed**. If you find any issues or bugs, feel free to open an issue or submit a fix.

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.

## Contact
For any inquiries, reach out via [your-email@example.com](mailto:your-email@example.com) or open an issue in the repository.

