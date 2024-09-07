# Tasker
## Ovarview
Tasker is a simple task manager that allows you to create, edit, and delete tasks. Also you can register and login to save your tasks. The serviceare using JWT for authentication. For development was user .NET 8.0, Entity Framework Core, and SQL Server.

## Setup instructions

### Prerequisites 
- .NET 8.0
- A SQL database (SQL Server, MySQL, PostgreSQL, SQLite)

### Cloning the repository
```bash
git clone https://github.com/Vladipz/Tasker.git 
cd Tasker
```
### Setup the environment
1. Configure Application Settings
Copy the example configuration file and edit it to set up your environment:
```bash
cd .\Tasker.API\
cp appsettings.example.json appsettings.Development.json
cd ..
```
Edit appsettings.json to include your database connection string and other settings.
2. Install dependencies
```bash
dotnet restore
```
3. Apply migrations
```bash
dotnet ef database update
```
4. Run the application
```bash
dotnet run --project .\Tasker.API\
```

## API Documentation

### Endpoints
#### Tasks Management
### `GET /api/tasks`

Retrieve a list of tasks.

#### Query Parameters:

- `priorityQuery`: string (optional)
- `statusQuery`: string (optional)
- `dueDate`: string (date-time) (optional)
- `sortColumn`: string (optional)
- `sortOrder`: string (optional)
- `page`: integer (required)
- `pageSize`: integer (required)

#### Responses:

- **200 OK**: List of tasks returned.
- **400 Bad Request**: Invalid query parameters.

---

### `POST /api/tasks`

Create a new task.

#### Request Body:

- `TaskRequest` (JSON):
  - `title`: string (optional)
  - `description`: string (optional)
  - `dueDate`: string (date-time) (optional)
  - `status`: integer (`TaskStatusType`)
  - `priority`: integer (`TaskPriorityType`)

#### Responses:

- **200 OK**: Task created.
- **400 Bad Request**: Validation error.

---

### `GET /api/tasks/{id}`

Retrieve a task by its `id`.

#### Path Parameters:

- `id`: string (uuid) (required)

#### Responses:

- **200 OK**: Task returned.
- **404 Not Found**: Task not found.
- **403 Forbidden**: Unauthorized. (user try get task that not belongs to him)

---

### `PUT /api/tasks/{id}`

Update an existing task by its `id`.

#### Path Parameters:

- `id`: string (uuid) (required)

#### Request Body:

- `TaskRequest` (JSON)

#### Responses:

- **200 OK**: Task updated.
- **400 Bad Request**: Validation error.
- **404 Not Found**: Task not found.
- **403 Forbidden**: Unauthorized(user try edit task that not belongs to him).

---

### `DELETE /api/tasks/{id}`

Delete a task by its `id`.

#### Path Parameters:

- `id`: string (uuid) (required)

#### Responses:

- **200 OK**: Task deleted.
- **404 Not Found**: Task not found.

---

#### Authentication

### `POST /api/auth/register`

Register a new user.

#### Request Body:

- `RegistrationRequest` (JSON):
  - `username`: string (required)
  - `email`: string (email) (required)
  - `password`: string (required)

#### Password Requirements:
- Password Requirements:
- The password cannot be empty.
- Minimum password length is 8 characters.
- Maximum password length is 16 characters.
- Must contain at least one uppercase letter.
- Must contain at least one lowercase letter.
- Must contain at least one number.
- Must contain at least one special character (e.g., !, ?, *, .).

#### Responses:

- **200 OK**: User registered.
- **409 Conflict**: Username or email already exists.
- **400 Bad Request**: Validation error.(password)

---

### `POST /api/auth/login`

Login a user.

#### Request Body:

- `LoginRequest` (JSON):
  - `username`: string (required)
  - `password`: string (required)

#### Responses:

- **200 OK**: User logged in.
- **404 Not Found**: User not found.
- **401 Unauthorized**: Invalid password.
- **400 Bad Request**: Validation error or missing fields in settings


---

## Components

### Schemas

#### `LoginRequest`

- **Properties**:
  - `username`: string (required, minLength: 1)
  - `password`: string (required, minLength: 1)

#### `RegistrationRequest`

- **Properties**:
  - `username`: string (required, minLength: 1)
  - `email`: string (required, format: email, minLength: 1)
  - `password`: string (required, minLength: 1)

#### `TaskPriorityType`

- **Enum Values**: 0, 1, 2, 3

#### `TaskRequest`

- **Properties**:
  - `title`: string (nullable, optional)
  - `description`: string (nullable, optional)
  - `dueDate`: string (format: date-time, nullable, optional)
  - `status`: integer (`TaskStatusType`)
  - `priority`: integer (`TaskPriorityType`)

#### `TaskStatusType`

- **Enum Values**: 0, 1, 2, 3
