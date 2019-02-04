
/****** Problem 1.	Employee Address ******/
SELECT TOP(5) e.EmployeeID, e.JobTitle, e.AddressID, a.AddressText
	FROM Employees AS e
	JOIN Addresses AS a
	ON e.AddressID = a.AddressID
	ORDER BY a.AddressID
/****** Problem 2.	Addresses with Towns ******/

SELECT TOP(50) e.FirstName, e.LastName, t.Name, a.AddressText
	FROM Employees AS e
	JOIN Addresses AS a
	ON e.AddressID = a.AddressID
	JOIN Towns AS t
	ON t.TownID = a.TownID
	ORDER BY e.FirstName, 
			 e.LastName
	
/****** Problem 3.	Sales Employee ******/

SELECT e.EmployeeID, e.FirstName, e.LastName, d.Name
	FROM Employees AS e
	JOIN Departments AS d
	ON d.DepartmentID = e.DepartmentID
	WHERE d.DepartmentID = 3
	ORDER BY e.EmployeeID

/****** Problem 4.	Employee Departments ******/
SELECT TOP(5) e.EmployeeID, e.FirstName, e.Salary, d.Name
	FROM Employees AS e
	JOIN Departments AS d
	ON d.DepartmentID = e.DepartmentID
	WHERE e.Salary > 15000
	ORDER BY e.DepartmentID

/****** Problem 5.	Employees Without Project ******/
SELECT TOP(3) EmployeeID, FirstName
	FROM Employees
	WHERE EmployeeID NOT IN (SELECT EmployeeID FROM EmployeesProjects)
	ORDER BY EmployeeID

/****** Problem 6.	Employees Hired After ******/

SELECT e.FirstName, e.LastName, e.HireDate, d.Name
	FROM Employees AS e
	JOIN Departments AS d
	ON e.DepartmentID = d.DepartmentID
	WHERE e.HireDate > '1-1-1999'
	AND d.Name IN ('Sales', 'Finance')
	ORDER BY e.HireDate

/****** Problem 7.	Employees with Project ******/
SELECT TOP(5) e.EmployeeID, e.FirstName, p.Name
	FROM Employees AS e
	JOIN EmployeesProjects AS ep
	ON e.EmployeeID = ep.EmployeeID
	JOIN Projects AS p
	ON ep.ProjectID = p.ProjectID
	WHERE p.StartDate > '08-13-2002'
	AND p.EndDate IS NULL
	ORDER BY e.EmployeeID

/****** Problem 8.	Employee 24 ******/
SELECT e.EmployeeID, e.FirstName, 
	CASE
	WHEN p.StartDate >= '01-01-2005' THEN NULL
	WHEN p.StartDate < '01-01-2005' THEN p.Name
	END AS [ProjectName]
	FROM Employees AS e
	JOIN EmployeesProjects AS ep
	ON e.EmployeeID = ep.EmployeeID
	JOIN Projects AS p
	ON ep.ProjectID = p.ProjectID
	WHERE e.EmployeeID = 24

/****** Problem 9.	Employee Manager ******/
SELECT e.EmployeeID, e.FirstName, e.ManagerID, m.FirstName AS ManagerName
	FROM Employees AS e
    JOIN Employees AS m ON e.ManagerID = m.EmployeeID
	WHERE e.ManagerID IN(3, 7)
	ORDER BY e.EmployeeID

/****** Problem 10.	Employee Summary ******/
SELECT TOP(50) 
		e.EmployeeID, 
		e.FirstName + ' ' + e.LastName AS EmployeeName, 
		m.FirstName + ' ' + m.LastName AS ManagerName, 
		d.Name
	FROM Employees AS e
    JOIN Employees AS m ON e.ManagerID = m.EmployeeID
	JOIN Departments AS d ON e.DepartmentID = d.DepartmentID
	ORDER BY e.EmployeeID

/****** Problem 11.	Min Average Salary ******/
SELECT MIN(asd.AverageSalary)
	FROM (
		SELECT AVG(Salary) AS AverageSalary
		FROM Employees
		GROUP BY DepartmentID
	) AS asd

/****** Problem 12.	Highest Peaks in Bulgaria ******/
SELECT mc.CountryCode, m.MountainRange, p.PeakName, p.Elevation
	FROM MountainsCountries AS mc
	JOIN Mountains AS m
	ON m.Id = mc.MountainId
	JOIN Peaks AS p
	ON p.MountainId = mc.MountainId
	WHERE p.Elevation > 2835
	AND mc.CountryCode = 'BG'
	ORDER BY p.Elevation DESC

/****** Problem 13.	Count Mountain Ranges ******/
SELECT 
	mc.CountryCode,
	COUNT(mc.CountryCode) AS MountaininRanges
	FROM MountainsCountries AS mc
	JOIN Mountains AS m
	ON m.Id = mc.MountainId
	WHERE mc.CountryCode IN ('US', 'RU', 'BG')
	GROUP BY mc.CountryCode

/****** Problem 14.	Countries with Rivers ******/
SELECT TOP(5) c.CountryName, r.RiverName
	FROM Countries AS c
	LEFT JOIN CountriesRivers AS cr
	ON c.CountryCode = cr.CountryCode
	LEFT JOIN Rivers as r
	ON r.Id = cr.RiverId 
	WHERE c.ContinentCode = 'AF'
	ORDER BY c.CountryName

/****** Problem 15.	*Continents and Currencies ******/
SELECT ranked.ContinentCode,
       ranked.CurrencyCode,
       ranked.CurrencyUsage
FROM
(
    SELECT gbc.ContinentCode,
           gbc.CurrencyCode,
           gbc.CurrencyUsage,
           DENSE_RANK() OVER(PARTITION BY gbc.ContinentCode ORDER BY gbc.CurrencyUsage DESC) AS UsageRank
    FROM
    (
        SELECT ContinentCode,
               CurrencyCode,
               COUNT(CurrencyCode) AS CurrencyUsage
        FROM Countries
        GROUP BY ContinentCode,
                 CurrencyCode
        HAVING COUNT(CurrencyCode) > 1
    ) AS gbc
) AS ranked
WHERE ranked.UsageRank = 1
ORDER BY ranked.ContinentCode

/****** Problem 16.	Countries without any Mountains ******/
SELECT COUNT(c.CountryCode) AS CountryCode
FROM Countries AS c
     LEFT OUTER JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
WHERE mc.CountryCode IS NULL; 

/****** Problem 17.	Highest Peak and Longest River by Country ******/
SELECT TOP (5) peaks.CountryName,
               peaks.Elevation AS HighestPeakElevation,
               rivers.Length AS LongestRiverLength
FROM
(
    SELECT c.CountryName,
           c.CountryCode,
           DENSE_RANK() OVER(PARTITION BY c.CountryName ORDER BY p.Elevation DESC) AS DescendingElevationRank,
           p.Elevation
    FROM Countries AS c
         FULL OUTER JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
         FULL OUTER JOIN Mountains AS m ON mc.MountainId = m.Id
         FULL OUTER JOIN Peaks AS p ON m.Id = p.MountainId
) AS peaks
FULL OUTER JOIN
(
    SELECT c.CountryName,
           c.CountryCode,
           DENSE_RANK() OVER(PARTITION BY c.CountryCode ORDER BY r.Length DESC) AS DescendingRiversLenghRank,
           r.Length
    FROM Countries AS c
         FULL OUTER JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
         FULL OUTER JOIN Rivers AS r ON cr.RiverId = r.Id
) AS rivers ON peaks.CountryCode = rivers.CountryCode
WHERE peaks.DescendingElevationRank = 1
      AND rivers.DescendingRiversLenghRank = 1
      AND (peaks.Elevation IS NOT NULL
           OR rivers.Length IS NOT NULL)
ORDER BY HighestPeakElevation DESC,
         LongestRiverLength DESC,
         CountryName; 

/****** Problem 18.	* Highest Peak Name and Elevation by Country ******/
SELECT TOP (5) jt.CountryName AS Country,
               CASE
                   WHEN jt.PeakName IS NULL
                   THEN '(no highest peak)'
                   ELSE jt.PeakName
               END AS HighestPeakName,
               CASE
                   WHEN jt.Elevation IS NULL
                   THEN 0
                   ELSE jt.Elevation
               END AS HighestPeakElevation,
               CASE
                   WHEN jt.MountainRange IS NULL
                   THEN '(no mountain)'
                   ELSE jt.MountainRange
               END AS Mountain
FROM
(
    SELECT c.CountryName,
           DENSE_RANK() OVER(PARTITION BY c.CountryName ORDER BY p.Elevation DESC) AS PeakRank,
           p.PeakName,
           p.Elevation,
           m.MountainRange
    FROM Countries AS c
         LEFT JOIN MountainsCountries AS mc ON c.CountryCode = mc.CountryCode
         LEFT JOIN Mountains AS m ON mc.MountainId = m.Id
         LEFT JOIN Peaks AS p ON m.Id = p.MountainId
) AS jt
WHERE jt.PeakRank = 1
ORDER BY jt.CountryName,
         jt.PeakName;