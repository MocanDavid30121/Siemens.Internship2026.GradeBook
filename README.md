# Siemens Internship 2026 - GradeBook API

This project is an ASP.NET Core Web API for managing and filtering grades.

## Technologies

- ASP.NET Core
- .NET 10
- C#
- HttpClient
- Dependency Injection

## Main changes implemented

### 1. Domain refactoring

The original project used generic names such as `Item`, `ItemController`, `IItemReader` and `ItemRepository`.

These were renamed to match the business domain:

- `Item` -> `Grade`
- `ItemController` -> `GradesController`
- `IItemReader` -> `IGradeRepository`
- `ItemRepository` -> `ExternalGradeRepository`

### 2. Framework upgrade

The project was upgraded from .NET 8 to .NET 10 by updating the target framework:

```xml
<TargetFramework>net10.0</TargetFramework>
3. Service layer

A service layer was added in order to keep business logic outside the controller and repository layers.

Added files:

Interfaces/IGradeService.cs
Services/GradeService.cs

The service layer contains the required filtering logic:

grade.IsActive && grade.Value >= 5
4. External repository

The original in-memory repository was replaced with an external data source.

ExternalGradeRepository retrieves grade data from the endpoint configured in appsettings.json.

5. SOLID review

A SOLID review document was added:

SOLID_REVIEW.md

It explains:

which SOLID principles were violated
where the issues were located
why they were violations
how they were fixed
API Endpoints
Get all grades
GET /api/grades
Get grade by id
GET /api/grades/{id}

Example:

GET /api/grades/2
Get first N active passing grades
GET /api/grades/passing-active?count={count}

Example:

GET /api/grades/passing-active?count=3

This endpoint returns only the first N grades that satisfy both conditions:

isActive == true
value >= 5
How to run the project

Restore dependencies:

dotnet restore

Build the project:

dotnet build

Run the project:

dotnet run

Then test the API in the browser or in Postman:

https://localhost:7069/api/grades
https://localhost:7069/api/grades/passing-active?count=3

The port may differ depending on the local launch settings.