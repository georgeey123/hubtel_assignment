# Policy Premium - Hubtel Interview Assignment

## Project Overview
This API provides a flexible insurance policy premium calculation system. It allows for creating, managing, and calculating premiums for insurance policies with multiple components and dynamic calculation rules.

## Technologies Used
- .NET 8.0
- ASP.NET Core Web API
- xUnit for testing
- Moq for mocking in tests
- Swagger/OpenAPI for API documentation

## Features
- CRUD operations for insurance policies
- Dynamic premium calculation based on policy components
- Support for both flat-rate and percentage-based premium components
- Comprehensive test coverage (unit and integration tests)
- In-memory data persistence (easily extensible to other storage solutions)
- RESTful API design
- Swagger documentation

## Project Structure
```bash
├── application/        # Application services and interfaces
├── domain/             # Domain entities and business rules 
├── infrastructure/     # Data persistence and external services 
├── tests/              # Unit and integration tests
└── web/                # API controllers and configuration 
```


## Getting Started

### Prerequisites
- .NET 8.0 SDK
- An IDE (Visual Studio, Rider, or VS Code)

### Installation
1. Clone the repository
```bash
git clone https://github.com/georgeey123/hubtel_assignment.git
```

2. Navigate to the project directory
```bash
cd policy
```
3. Build the solution
```bash
cd web/
```
then
```bash
dotnet build
```

4. Runing the tests
Navigate to the test project
```bash
cd policy/tests
```
Run the command below to run all tests
```bash
dotnet run tests
```

6. Run the application
```bash
cd policy/web
dotnet run
```

The API will be available at `https://localhost:5229` (or your configured port)


## API Documentation

### Endpoints

#### Policies
- `GET /api/Policy` - Get all policies
- `GET /api/Policy/{id}` - Get policy by ID
- `POST /api/Policy` - Create new policy
- `PATCH /api/Policy/{id}` - Update existing policy
- `DELETE /api/Policy/{id}` - Delete policy

#### Premium Calculation
- `POST /api/Quote/request-quote` - Calculate premium for a policy

### Sample Requests

#### Create Policy

POST /api/Policy 

```bash
{
  "policyName": "string",
  "components": [
    {
      "sequence": 0,
      "name": "string",
      "operation": 1,
      "flatValue": 0,
      "percentageValue": 0
    }
  ]
}
```
### OperationType Enum Specification
1. add : 1
2. subtract : 2

#### Calculate Premium

POST /api/Quote/request-quote 
```bash
 {
  "policyId": 1,
  "marketValue": 10000
 }
```

## Future Improvements
1. Implement persistent storage (e.g., SQL Server)
2. Add authentication and authorization
3. Implement caching for frequently accessed policies
4. Add validation rules for policy components
5. Implement audit logging
6. Add rate limiting
7. Include policy versioning

## Security Measures
- Input validation
- Model validation
- Exception handling
- Safe error messages
- HTTPS enforcement

## Development Decisions and Trade-offs

### Why In-Memory Repository?
For demonstration purposes, an in-memory repository was chosen for simplicity and ease of testing. In a production environment, this would be replaced with a proper database implementation.

### Why PATCH for Updates?
PATCH was chosen over PUT for updates to support partial updates of policies, following REST best practices and reducing unnecessary data transfer.

### Calculation Strategy
The premium calculation strategy was designed to be extensible, allowing for easy addition of new calculation components

