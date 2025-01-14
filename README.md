# Tinker POS - Pharmacy Management System

A modern, full-stack Blazor WebAssembly application for pharmacy point-of-sale and inventory management.

## Project Structure
```
Tinker/
├── Tinker.Server         # Backend API and server-side logic
├── Tinker.Client         # Blazor WebAssembly frontend
├── Tinker.Shared         # Shared models and interfaces
└── Tinker.Infrastructure # Infrastructure services and data access
```

## Architecture Overview

The system follows Clean Architecture principles with distinct layers:

### Core Layer
- Domain entities and business logic
- Use cases and domain services
- Domain events and validation

### Infrastructure Layer
- Data access and persistence
- Identity and security
- Caching and monitoring
- Error handling

### API Layer
- GraphQL endpoints
- Security middleware
- Performance metrics

### Client Layer
- Blazor WASM interface
- State management
- PWA support
- SEO optimization

## Features

- 🏪 **Point of Sale**
  - Process sales transactions
  - Handle payments
  - Generate invoices
  - Manage orders

- 📦 **Inventory Management**
  - Stock tracking
  - Expiry date monitoring
  - Low stock alerts
  - Batch management

- 👥 **Customer Management**
  - Customer profiles
  - Loyalty program
  - Purchase history
  - Contact management

- 💊 **Prescription Management**
  - RX validation
  - Refill tracking
  - Compliance logging
  - Patient records

- 📊 **Reporting**
  - Sales analytics
  - Inventory reports
  - Compliance reports
  - Customer insights

## Key Features

- Enhanced security measures
- Comprehensive error handling
- Performance monitoring
- Offline support
- SEO optimization

## Technology Stack

- **.NET 9.0**
  - Blazor WebAssembly
  - ASP.NET Core
  - Entity Framework Core
  - Identity Framework

- **Database**
  - SQL Server
  - Redis Cache

- **Monitoring**
  - Application Insights
  - Serilog
  - Health Checks

- **API Documentation**
  - Swagger/OpenAPI
  - GraphQL

## Getting Started

1. **Prerequisites**
   - .NET 9.0 SDK
   - SQL Server
   - Redis (optional)

2. **Configuration**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "your_connection_string",
       "Redis": "your_redis_connection"
     }
   }

### Running the Application

1. **Clone the repository**:
   ```sh
   git clone https://github.com/bilalobe/Tinker.git
   cd Tinker
   ```

2. **Build the solution**:
   ```sh
   dotnet build
   ```

3. **Run the server**:
   ```sh
   cd Tinker.Server
   dotnet run
   ```

4. **Run the client**:
   ```sh
   cd Tinker.Client
   dotnet run
   ```

### API Documentation

- **Swagger**: Access the API documentation at `https://localhost:/api/docs`
- **GraphQL**: Access the GraphQL Playground at `https://localhost:/graphql`

### Health Checks

- Access health checks at `https://localhost/health`



### License

This project is licensed under the Mozilla Public License Version 2.0. See the LICENSE file for details.

### Contact

For any inquiries or support, please contact us at [support@tinkerpos.com](mailto:support@tinkerpos.com).

---
