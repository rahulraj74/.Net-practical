# Experiment 10: Student Management REST API using ASP.NET Core

**Course:** 01CE1523 - .NET Technologies  
**Department:** Department of Computer Engineering, Faculty of Engineering and Technology, Marwadi University  
**Project Name:** `StudentManagementAPI`

---

## 🎯 Practical Definition & Aim
> **AIM:** Build Student Management REST API using ASP.NET Core (Middleware, API, Project Structure)

The objective of this practical is to create a RESTful Web API using ASP.NET Core that performs CRUD (Create, Read, Update, Delete) operations on a Student entity interacting with a SQL Server database via ADO.NET (`Microsoft.Data.SqlClient`). It also configures Swagger/OpenAPI documentation and middleware.

---

## 📂 Project Structure

```text
StudentManagementAPI/
├── Controllers/
│   ├── StudentsController.cs          # REST API endpoints for Student CRUD operations
│   └── WeatherForecastController.cs   # Default WeatherForecast GET endpoint
├── Models/
│   └── Student.cs                     # Student model class
├── Properties/
│   └── launchSettings.json            # Launch configuration with Swagger UI URL
├── appsettings.json                   # Connection strings and configuration
├── appsettings.Development.json       # Development configuration
├── create_table.sql                   # SQL script to create database and table
├── Program.cs                         # Application startup, DI services, and middleware pipeline
├── StudentManagementAPI.csproj        # Project configuration with Swashbuckle & SqlClient
├── WeatherForecast.cs                 # WeatherForecast model
└── README.md                          # Documentation
```

---

## 💻 Source Code (Identical to Practical PDF)

### 1. `WeatherForecast.cs`
Represents the default weather forecast model:
```csharp
namespace StudentManagementAPI
{
    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string? Summary { get; set; }
    }
}
```

### 2. `Program.cs`
Configures services, Swagger generator, and the HTTP request middleware pipeline:
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 
app.UseAuthorization(); 
app.MapControllers();

app.Run();
```

### 3. `Controllers/WeatherForecastController.cs`
Sample controller providing weather data:
```csharp
using Microsoft.AspNetCore.Mvc;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", 
            "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
```

### 4. `Models/Student.cs`
Defines the `Student` entity with default field values:
```csharp
namespace StudentManagementAPI.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Vrushti";
        public string Email { get; set; } = "vrushti@gmail.com";
        public string Course { get; set; } = "B. tech CE";
    }
}
```

### 5. `Controllers/StudentsController.cs`
Implements complete CRUD operations using ADO.NET (`Microsoft.Data.SqlClient`):
- `GET /api/Students` — Fetch all students
- `GET /api/Students/{id}` — Fetch student by Id
- `POST /api/Students` — Insert a new student (`SCOPE_IDENTITY()`)
- `PUT /api/Students/{id}` — Update existing student by Id
- `DELETE /api/Students/{id}` — Remove student by Id

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StudentManagementAPI.Models;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StudentsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        // GET: api/students
        [HttpGet]
        public IActionResult GetStudents()
        {
            List<Student> students = new List<Student>();
            using SqlConnection con = GetConnection();
            string query = "SELECT * FROM Students";
            using SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new Student
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString()!,
                    Email = reader["Email"].ToString()!,
                    Course = reader["Course"].ToString()!
                });
            }
            return Ok(students);
        }

        // GET: api/students/1
        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            using SqlConnection con = GetConnection();
            string query = "SELECT * FROM Students WHERE Id = @Id";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Student student = new Student
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString()!,
                    Email = reader["Email"].ToString()!,
                    Course = reader["Course"].ToString()!
                };
                return Ok(student);
            }
            return NotFound();
        }

        // POST: api/students
        [HttpPost]
        public IActionResult AddStudent(Student student)
        {
            using SqlConnection con = GetConnection();
            string query = @"INSERT INTO Students (Name, Email, Course)
VALUES (@Name, @Email, @Course);
SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Name", student.Name);
            cmd.Parameters.AddWithValue("@Email", student.Email);
            cmd.Parameters.AddWithValue("@Course", student.Course);
            con.Open();
            student.Id = Convert.ToInt32(cmd.ExecuteScalar());
            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        // PUT: api/students/1
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, Student student)
        {
            using SqlConnection con = GetConnection();
            string query = @"UPDATE Students
SET Name = @Name,
    Email = @Email,
    Course = @Course
WHERE Id = @Id";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", student.Name);
            cmd.Parameters.AddWithValue("@Email", student.Email);
            cmd.Parameters.AddWithValue("@Course", student.Course);
            con.Open();
            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                return NotFound();
            return Ok("Student updated successfully.");
        }

        // DELETE: api/students/1
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            using SqlConnection con = GetConnection();
            string query = "DELETE FROM Students WHERE Id = @Id";
            using SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();
            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                return NotFound();
            return Ok("Student deleted successfully.");
        }
    }
}
```

---

## 🗄️ Database Setup (SQL Server)
Execute the following script in SQL Server Management Studio (SSMS) or Azure Data Studio:

```sql
CREATE DATABASE StudentDB;
GO

USE StudentDB;
GO

CREATE TABLE Students (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Course NVARCHAR(100) NOT NULL
);
GO

-- Sample student matching the practical
INSERT INTO Students (Name, Email, Course)
VALUES ('Vrushti', 'vrushti@gmail.com', 'B. tech CE');
GO
```

Ensure your `appsettings.json` points to your SQL Server instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=StudentDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## 🚀 How to Run the Application

1. Open a terminal and navigate to the project directory:
   ```bash
   cd /Users/rahulraj/.gemini/antigravity/scratch/StudentManagementAPI
   ```

2. Run the project:
   ```bash
   dotnet run
   ```

3. Open your browser and navigate to the Swagger UI:
   ```
   http://localhost:5167/swagger
   ```

---

## 📸 Output Verification Matching PDF Screenshots

| Operation | HTTP Method & Path | Description | Screenshot Page in PDF |
|---|---|---|---|
| **Get All Students** | `GET /api/Students` | Fetches all students from the database | Page 5 |
| **Add Student** | `POST /api/Students` | Accepts JSON `{"id":0,"name":"...","email":"...","course":"..."}` and returns Created student with generated `Id` | Page 5 |
| **Get Student By Id** | `GET /api/Students/{id}` | Fetches student details for given `id` | Page 6 |
| **Update Student** | `PUT /api/Students/{id}` | Updates existing student details | Page 6 |
| **Delete Student** | `DELETE /api/Students/{id}` | Removes student by `id` | Page 5 & 6 |
| **WeatherForecast** | `GET /WeatherForecast` | Returns sample array of WeatherForecast records | Page 6 |
| **Schemas** | `Student`, `WeatherForecast` | JSON Schema models | Page 6 |
