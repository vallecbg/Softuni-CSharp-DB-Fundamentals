SELECT * FROM Towns
SELECT * FROM Addresses
SELECT * FROM Employees
EXEC dbo.usp_GetEmployeesSalaryAboveNumber 48100

/****** Problem 1. Employees with Salary Above 35000 ******/
CREATE PROC dbo.usp_GetEmployeesSalaryAbove35000
AS
  SELECT FirstName, LastName
  FROM Employees
  WHERE Salary > 35000

/****** Problem 2. Employees with Salary Above Number ******/
CREATE PROC dbo.usp_GetEmployeesSalaryAboveNumber (@salary DECIMAL(18,4)) AS
SELECT FirstName, LastName
FROM Employees
WHERE Salary >= @salary

/****** Problem 3. Town Names Starting With ******/
CREATE PROC dbo.usp_GetTownsStartingWith (@startingCharacter VARCHAR(MAX))
AS
SELECT Name
FROM Towns
WHERE LEFT(Name, LEN(@startingCharacter)) = @startingCharacter

/****** Problem 4. Employees from Town ******/
CREATE PROC dbo.usp_GetEmployeesFromTown (@townName NVARCHAR(MAX))
AS
SELECT FirstName, LastName
	FROM Employees AS e
	JOIN Addresses AS a ON a.AddressID = e.AddressID
	JOIN Towns AS t ON t.TownID = a.TownID
	WHERE t.Name = @townName

/****** Problem 5. Salary Level Function ******/
CREATE FUNCTION
ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS NVARCHAR(7)
AS
BEGIN
	DECLARE @salaryLevel VARCHAR(7)

	IF @salary < 30000
		SET @salaryLevel = 'Low'
	ELSE IF @salary BETWEEN 30000 AND 50000
		SET @salaryLevel = 'Average'
	ELSE IF @salary > 50000
		SET @salaryLevel = 'High'

	RETURN @salaryLevel
END;

/****** Problem 6. Employees by Salary Level ******/
CREATE PROC dbo.usp_EmployeesBySalaryLevel (@salaryParameter NVARCHAR(10))
AS
	SELECT FirstName, LastName 
	FROM(
		SELECT FirstName, LastName, dbo.ufn_GetSalaryLevel(Salary) AS SalaryLevel
		FROM Employees
	) as c
	WHERE c.SalaryLevel = 'High'
		
/****** Problem 7. Define Function ******/
CREATE FUNCTION
ufn_IsWordComprised(@setOfLetters NVARCHAR(MAX), @word NVARCHAR(MAX))
RETURNS BIT
AS
     BEGIN
         DECLARE @currentIndex INT = 1;
         WHILE(@currentIndex <= LEN(@word))
             BEGIN
                 DECLARE @currentLetter CHAR= SUBSTRING(@word, @currentIndex, 1);
                 IF(CHARINDEX(@currentLetter, @setOfLetters) <= 0)
                     BEGIN
                         RETURN 0;
                 END;
                 SET @currentIndex+=1;
             END;
         RETURN 1;
     END;
		
/****** Problem 8. * Delete Employees and Departments ******/
CREATE PROCEDURE usp_DeleteEmployeesFromDepartment
(
                 @departmentId INT
)
AS
     BEGIN
         ALTER TABLE Employees ALTER COLUMN ManagerID INT;

         ALTER TABLE Employees ALTER COLUMN DepartmentID INT;

         UPDATE Employees
           SET
               DepartmentID = NULL
         WHERE EmployeeID IN
         (
         (
             SELECT EmployeeID
             FROM Employees
             WHERE DepartmentID = @departmentId
         )
         );

         UPDATE Employees
           SET
               ManagerID = NULL
         WHERE ManagerID IN
         (
         (
             SELECT EmployeeID
             FROM Employees
             WHERE DepartmentID = @departmentId
         )
         );

         ALTER TABLE Departments ALTER COLUMN ManagerID INT;

         UPDATE Departments
           SET
               ManagerID = NULL
         WHERE DepartmentID = @departmentId;

         DELETE FROM Departments
         WHERE DepartmentID = @departmentId;

         DELETE FROM EmployeesProjects
         WHERE EmployeeID IN
         (
         (
             SELECT EmployeeID
             FROM Employees
             WHERE DepartmentID = @departmentId
         )
         );

         DELETE FROM Employees
         WHERE DepartmentID = @departmentId;

         SELECT COUNT(*)
         FROM Employees
         WHERE DepartmentID = @departmentId;
     END;