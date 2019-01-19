--2
SELECT * FROM Departments
--3
SELECT Name FROM Departments
--4
SELECT FirstName, LastName, Salary FROM Employees
--5
SELECT FirstName, MiddleName, LastName FROM Employees
--6
SELECT FirstName + '.' + LastName + '@softuni.bg' AS [Full Email Address]
	FROM Employees
--7
SELECT DISTINCT Salary
	FROM Employees
--8
SELECT *
	FROM Employees
	WHERE JobTitle = 'Sales Representative'
--9
SELECT FirstName, LastName, JobTitle
	FROM Employees
	WHERE Salary BETWEEN 20000 AND 30000
--10
SELECT FirstName + ' ' + MiddleName + ' ' + LastName AS [Full Name]
	FROM Employees
	WHERE Salary IN (25000, 14000, 12500, 23600)
--11
SELECT FirstName, LastName
	FROM Employees
	WHERE ManagerID IS NULL
--12
SELECT FirstName, LastName, Salary
	FROM Employees
	WHERE Salary > 50000
	ORDER BY Salary DESC
--13
SELECT TOP(5) FirstName, LastName
	FROM Employees
	ORDER BY Salary DESC
--14
SELECT FirstName, LastName
	FROM Employees
	WHERE DepartmentID != 4

SELECT FirstName, LastName
	FROM Employees
	WHERE DepartmentID NOT IN (4)
--15
SELECT EmployeeID, FirstName, LastName, MiddleName, JobTitle, DepartmentID, ManagerID, HireDate, Salary, AddressID
	FROM Employees
	ORDER BY 
	Salary DESC,
	FirstName ASC,
	LastName DESC,
	MiddleName ASC
--16 - you need to submit only this part in judge
CREATE VIEW V_EmployeesSalaries  
    AS
	SELECT FirstName,LastName,Salary 
	FROM Employees 
--------------------------------------------------
SELECT * 
	FROM V_EmployeesSalaries

--17-----------------
CREATE VIEW V_EmployeeNameJobTitle
	AS
	SELECT FirstName + ' ' + ISNULL(MiddleName, '') + ' '  + LastName AS [Full Name],
	JobTitle
	FROM Employees
-----------------------------
SELECT *
	FROM V_EmployeeNameJobTitle

--18
SELECT DISTINCT JobTitle
	FROM Employees

--19
SELECT TOP(10) ProjectID, Name, Description, StartDate, EndDate
	FROM Projects
	ORDER BY StartDate, Name

--20
SELECT TOP(7) FirstName, LastName, HireDate
	FROM Employees
	ORDER BY HireDate DESC, FirstName DESC, LastName DESC

--21
/* Not for Judge - Making Backup
BACKUP DATABASE SoftUni TO DISK = 'D:\softuni-backup.bak';
*/

UPDATE Employees
   SET Salary = Salary * 1.12
 WHERE DepartmentID IN (1, 2, 4, 11)

SELECT Salary
	FROM Employees

 /* Not for Judge - Restore from Backup
USE [Geography];
GO
ALTER DATABASE SoftUni SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE SoftUni;
GO
RESTORE DATABASE SoftUni FROM DISK = 'D:\softuni-backup.bak';
*/


--22
SELECT PeakName
	FROM Peaks
	ORDER BY PeakName

--23
SELECT TOP(30) CountryName, Population
	FROM Countries
	WHERE ContinentCode = 'EU'
	ORDER BY Population DESC, CountryName ASC

--24
SELECT CountryName,
       CountryCode,
       CASE CurrencyCode
           WHEN 'EUR'
           THEN 'Euro'
           ELSE 'Not Euro'
       END AS 'Currency'
FROM Countries
ORDER BY CountryName;

--25
SELECT Name
	FROM Characters
	ORDER BY Name