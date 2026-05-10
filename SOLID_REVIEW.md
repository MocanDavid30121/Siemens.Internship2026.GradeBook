# SOLID Review

## 1. Controller with too many responsibilities

### SOLID principle violated
Single Responsibility Principle

### Location
`Controllers/ItemController.cs`, methods `GetAll` and `GetById`

### Why this was a violation
The original controller was responsible for handling HTTP requests, writing log messages with `Console.WriteLine`, validating input, calculating response statistics such as total count and average value, and creating the HTTP response.

A controller should mainly coordinate the HTTP request/response flow. Business rules and data processing should be placed in a dedicated service layer.

### Fix applied
The controller was renamed from `ItemController` to `GradesController`.

The controller was simplified so it only:
- receives HTTP requests
- validates simple route/query parameters
- calls `IGradeService`
- returns HTTP responses

The business logic was moved to `GradeService`.

---

## 2. Repository containing business filtering

### SOLID principle violated
Single Responsibility Principle

### Location
`Repositories/ItemRepository.cs`, methods `GetAllAsync` and `GetByIdAsync`

### Why this was a violation
The original repository filtered items by `IsActive`.

This means the data access layer was also applying a business rule. A repository should be responsible for retrieving data, not deciding which grades are valid for a specific business use case.

### Fix applied
The repository was refactored into `ExternalGradeRepository`.

The repository now retrieves grades from the external endpoint. The filtering logic was moved to `GradeService`, specifically to the method `GetFirstPassingActiveGradesAsync`.

---

## 3. Hardcoded in-memory data source

### SOLID principle violated
Open/Closed Principle and Dependency Inversion Principle

### Location
`Repositories/ItemRepository.cs`, fields `_items` and `_nextId`

### Why this was a violation
The original repository was directly tied to an in-memory `List<Item>` data source.

Changing the data source from in-memory data to an external endpoint required modifying the repository implementation. This makes the code less extensible.

### Fix applied
The in-memory repository was replaced with `ExternalGradeRepository`.

The new repository retrieves data from the external endpoint using `HttpClient`.

The rest of the application depends on the abstraction `IGradeRepository`, not directly on a concrete data source.

---

## 4. Missing dependency injection registrations

### SOLID principle violated
Dependency Inversion Principle

### Location
`Program.cs`

### Why this was a violation
The original controller depended on `IItemReader`, but no implementation was registered in the dependency injection container.

This means the application could not create the controller correctly at runtime.

### Fix applied
The dependency injection configuration was updated in `Program.cs`.

The following services were registered:

```csharp
builder.Services.AddHttpClient<IGradeRepository, ExternalGradeRepository>();
builder.Services.AddScoped<IGradeService, GradeService>();
. Missing service layer
SOLID principle violated
Single Responsibility Principle
Location
Original project structure
Why this was a violation
The original project did not have a service layer. Without a service layer, business rules can easily end up in the controller or repository.
The assignment specifically requires a service layer for business logic.
Fix applied
A service layer was added:


IGradeService


GradeService


The method GetFirstPassingActiveGradesAsync implements the required business rule:


the grade must be active


the grade value must be greater than or equal to 5


only the first N matching grades are returned



6. Generic domain naming
SOLID principle violated
This is not a direct SOLID violation, but it is a domain modeling issue.
Location
Item, IItemReader, ItemRepository, ItemController
Why this was a problem
The assignment is about grades, but the original code used generic names such as Item.
Generic names make the code harder to understand and less aligned with the business domain.
Fix applied
The domain was renamed:


Item became Grade


IItemReader became IGradeRepository


ItemRepository became ExternalGradeRepository


ItemController became GradesController
