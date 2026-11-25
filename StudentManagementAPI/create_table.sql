-- SQL Server script for Experiment 10: Student Management REST API

-- 1. Create the Database
CREATE DATABASE StudentDB;
GO

USE StudentDB;
GO

-- 2. Create the Students Table
CREATE TABLE Students (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Course NVARCHAR(100) NOT NULL
);
GO

-- 3. Insert initial sample data (matching the default values in Student.cs)
INSERT INTO Students (Name, Email, Course)
VALUES ('Vrushti', 'vrushti@gmail.com', 'B. tech CE');
GO

-- 4. Verify table contents
SELECT * FROM Students;
GO
