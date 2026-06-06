# HospitalApi

ASP.NET Core Web API application using Entity Framework Core in the Database First approach.

The project was implemented based on a hospital database. It provides endpoints for retrieving patient data and assigning an available bed to a patient for a selected time period.

## Technologies

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server LocalDB
- Database First
- JetBrains Rider
- Git / GitHub

## Database

The application uses a SQL Server LocalDB database created from the provided `create.sql` script.

Database name used in the project:

```text
HospitalDb
```

Connection string is stored in `appsettings.json`:

```text
"ConnectionStrings" : {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HospitalDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Entity classes and `HospitalDbContext` were generated from the existing database using EF Core Database First scaffold.

Example scaffold command:

```bash
dotnet ef dbcontext scaffold "Server=(localdb)\MSSQLLocalDB;Database=HospitalDb;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o Models -c HospitalDbContext --context-dir Data --force
```

## Project structure

```text
Controllers
Data
DTOs
Exceptions
Models
Services
```

### Main folders

- `Controllers` - API controllers
- `Data` - database context
- `DTOs` - request and response DTO classes
- `Exceptions` - custom exceptions used for API error handling
- `Models` - entity classes generated from the database
- `Services` - application logic and database operations

## Endpoints

### Get patients

```http
GET /api/patients
```

Returns all patients with their admissions and bed assignments.

The response includes:

- patient data,
- admissions with ward data,
- bed assignments with bed, bed type, room and ward data.

Optional query parameter:

```http
GET /api/patients?search=an
```

The `search` parameter filters patients by first name and last name.

Filtering is performed using SQL `LIKE` with `%` wildcards.

### Assign bed to patient

```http
POST /api/patients/{pesel}/bedassignments
```

Assigns an available bed to a patient for the selected time period.

Example request body:

```json
{
  "from": "2026-06-01T09:00:00",
  "to": "2026-06-10T10:00:00",
  "bedType": "Standard",
  "ward": "Kardiologia"
}
```

The `to` field is optional.

The endpoint searches for a bed that:

- has the selected bed type,
- is located in the selected ward,
- is not occupied during the selected time period.

## HTTP status codes

The API returns the following HTTP status codes:

```text
200 OK - patients returned successfully
201 Created - bed assignment created successfully
400 Bad Request - invalid request data
404 Not Found - patient, ward, bed type or available bed was not found
```

## Validation and error handling

The API handles the following cases:

- patient with the provided PESEL does not exist,
- bed type does not exist,
- ward does not exist,
- no available bed exists for the selected period,
- assignment start date is not earlier than assignment end date,
- required request fields are missing or empty.

Error messages are clear and specific to the detected problem.

## Running the application

1. Create the database using the provided `create.sql` script.
2. Make sure the connection string in `appsettings.json` points to the correct LocalDB database.
3. Run the application from Rider or with:

```bash
dotnet run
```

Default local address used during development:

```text
http://localhost:5214
```

The port may be different depending on local launch settings.

## Testing

Example HTTP requests are available in:

```text
HospitalApi.http
```

The API can also be tested with Postman.

Tested scenarios include:

- getting all patients,
- filtering patients by search parameter,
- assigning a bed to a patient,
- assigning a bed with invalid dates,
- assigning a bed to a non-existing patient,
- assigning a non-existing bed type,
- assigning a bed in a non-existing ward,
- assigning a bed when no available bed exists.

## Author

#### Grzegorz Wojewódzki