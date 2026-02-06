AssignmentProject
Technology Stack

Backend: ASP.NET Core (.NET 8)
Frontend: AngularJS
Authentication: JWT (JSON Web Tokens)
Database Approach: DB First with Stored Procedures
Password Security: BCrypt hashing

The backend is built using ASP.NET Core .NET 8 to ensure high performance, improved security, and long-term support.

Database Design
ER Diagram (ASCII Representation)
+-------------------+          +-------------------+
|       Users       |          |       Tasks       |
+-------------------+          +-------------------+
| UserId (PK)       |<------┐  | TaskId (PK)       |
| FullName          |       └--| TaskTitle         |
| Username (UNQ)    |          | TaskDescription   |
| PasswordHash      |          | TaskDueDate       |
| UniqueId          |          | TaskStatus        |
| CreatedOn         |          | TaskRemarks       |
+-------------------+          | CreatedOn         |
                               | LastUpdatedOn     |
                               | CreatedById (FK)  |
                               | CreatedByName     |
                               | LastUpdatedById   |
                               | LastUpdatedByName |
                               +-------------------+

Relationship

One User can create multiple Tasks (One-to-Many relationship).

Data Dictionary (Summary)
Users Table

UserId – Primary key identifying each user

FullName – User’s full name

Username – Unique username used for login

PasswordHash – Encrypted password using BCrypt

UniqueId – Public unique identifier for user tracking

CreatedOn – User creation timestamp

Tasks Table

TaskId – Primary key identifying each task

TaskTitle – Title of the task

TaskDescription – Detailed task description

TaskDueDate – Due date of the task

TaskStatus – Current status of the task

TaskRemarks – Additional notes

CreatedOn – Task creation timestamp

LastUpdatedOn – Last update timestamp

CreatedById – Foreign key referencing Users.UniqueId

CreatedByName – Name of the user who created the task

LastUpdatedById – User who last updated the task

LastUpdatedByName – Name of the last updating user

Indexes Used

Unique index on Username for fast authentication

Index on TaskId for quick task updates and deletes

Index on CreatedById for user-specific task filtering

Index on TaskTitle for optimized search functionality

Database Approach

A Database First approach is used.
All database operations and validations are handled through stored procedures, ensuring better control, security, and consistency.

Project Folder Structure
AssignmentProject/
│
├── Controllers/
│   ├── AuthController.cs        // Signup, Login, JWT handling
│   ├── TaskApiController.cs     // Task CRUD APIs
│
├── Models/
│   ├── LoginModel.cs            // Login request model
│   ├── RegisterModel.cs         // Signup request model
│   ├── TaskModel.cs             // Task entity model
│
├── wwwroot/
│   ├── task.js                  // AngularJS application logic
│
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml         // Login & Signup UI
│   │   ├── Privacy.cshtml       // Task Management UI
│
├── appsettings.json             // Database and JWT configuration
├── Program.cs                   // .NET 8 application startup
├── README.md                    // Project documentation
└── AssignmentProject.csproj     // Project configuration file

Frontend–Backend Communication

The AngularJS frontend communicates with the ASP.NET Core Web API via HTTP requests.

All secured API endpoints require a valid JWT token.

JWT tokens are issued during login and attached to subsequent requests for authorization.
