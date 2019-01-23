--1
SELECT FirstName, LastName
	FROM Employees
	WHERE FirstName LIKE 'Sa%'
--2
SELECT FirstName, LastName
	FROM Employees
	WHERE LastName LIKE '%ei%'
--3
SELECT FirstName
	FROM Employees
	WHERE DepartmentID IN (3, 10) 
	AND DATEPART(YEAR, HireDate) BETWEEN 1995 AND 2005
--4
SELECT FirstName, LastName
	FROM Employees
	WHERE JobTitle NOT LIKE '%engineer%'
--5
SELECT Name
	FROM Towns
	WHERE LEN(Name) IN (5, 6)
	ORDER BY Name
--6
SELECT TownID, Name
	FROM Towns
	WHERE Name LIKE '[MKBE]%'
	ORDER BY Name
--7
SELECT TownID, Name
	FROM Towns
	WHERE Name NOT LIKE '[RBD]%'
	ORDER BY Name
--8
CREATE VIEW V_EmployeesHiredAfter2000
	AS
	SELECT FirstName, LastName
	FROM Employees
	WHERE DATEPART(YEAR, HireDate) > 2000
SELECT *
	FROM V_EmployeesHiredAfter2000
--9
SELECT FirstName, LastName
	FROM Employees
	WHERE LEN(LastName) = 5
--10
SELECT EmployeeID, FirstName, LastName, Salary, 
	DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeID) AS [Rank]
	FROM Employees
	WHERE Salary BETWEEN 10000 AND 50000
	ORDER BY Salary DESC
--11
--1st method which doesn't work in judge but in local works
CREATE VIEW V_EmployeesSalary
	AS
SELECT EmployeeID, FirstName, LastName, Salary, 
	DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeID) AS [Rank]
	FROM Employees
	WHERE Salary BETWEEN 10000 AND 50000

	SELECT *
	FROM V_EmployeesSalary
	WHERE [Rank] = 2
	ORDER BY Salary DESC
-------
--2nd method
SELECT * FROM(
	SELECT EmployeeID, FirstName, LastName, Salary, 
		DENSE_RANK() OVER (PARTITION BY Salary ORDER BY EmployeeID) AS [Rank]
		FROM Employees AS R
	) Employees
	WHERE Rank = 2 AND Salary BETWEEN 10000 AND 50000
	ORDER BY Salary DESC
--12
SELECT CountryName, IsoCode
	FROM Countries
	WHERE CountryName LIKE '%a%a%a%'
	ORDER BY IsoCode
--13
SELECT Peaks.PeakName, Rivers.RiverName, LOWER(CONCAT(LEFT(Peaks.PeakName, LEN(Peaks.PeakName) - 1), Rivers.RiverName)) AS [Mix]
	FROM Peaks, Rivers
	WHERE RIGHT(Peaks.PeakName, 1) = LEFT(Rivers.RiverName, 1)
	ORDER BY Mix
--14
SELECT TOP(50) Name, FORMAT( Start, 'yyyy-MM-dd' ) AS [Start]
	FROM Games
	WHERE DATEPART(YEAR, Start) IN (2011, 2012)
	ORDER BY Start, Name
--15
SELECT Username, RIGHT(Email, LEN(Email) - CHARINDEX('@', Email)) AS [Email Provider]
	FROM Users
	ORDER BY [Email Provider], Username
--16
SELECT Username, IpAddress
	FROM Users
	WHERE IpAddress LIKE '___.1%.%.___'
	ORDER BY Username
--17
SELECT Name AS [Game], 
	CASE
		WHEN DATEPART(HOUR, Start) BETWEEN 0 AND 11 THEN 'Morning'
		WHEN DATEPART(HOUR, Start) BETWEEN 12 AND 17 THEN 'Afternoon'
		WHEN DATEPART(HOUR, Start) BETWEEN 18 AND 23 THEN 'Evening'
	END AS [Part of the Day],
	CASE
		WHEN Duration <= 3 THEN 'Extra Short'
		WHEN Duration BETWEEN 4 AND 6 THEN 'Short'
		WHEN Duration > 6 THEN 'Long'
		WHEN Duration IS NULL THEN 'Extra Long'
	END AS [Duration]
	FROM Games
	ORDER BY Name, [Duration], [Part of the Day]
--18
SELECT ProductName, OrderDate,
	DATEADD(DAY, 3, OrderDate) AS [Pay Due],
	DATEADD(MONTH, 1, OrderDate) AS [Deliver Due]
	FROM Orders