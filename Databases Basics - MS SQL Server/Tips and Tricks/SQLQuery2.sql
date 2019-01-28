/* ********************************************************
	SIMPLE JOIN QUERY
***********************************************************/
SELECT m.MountainRange, p.PeakName, p.Elevation
FROM Mountains AS m
JOIN Peaks AS p
ON p.MountainId = m.Id
WHERE MountainId = 17
ORDER BY p.Elevation DESC

/* ********************************************************
	MANY TO MANY RELATIONSHIP
***********************************************************/
CREATE TABLE Employees(
	EmployeeID INT NOT NULL,
	Name NVARCHAR(50) NOT NULL

	CONSTRAINT PK_Employees
	PRIMARY KEY (EmployeeID)
)

CREATE TABLE Projects(
	ProjectID INT NOT NULL,
	Name NVARCHAR(50) NOT NULL,

	CONSTRAINT PK_Projects
	PRIMARY KEY (ProjectID)
)

CREATE TABLE EmployeesProjects(
	EmployeeID INT NOT NULL,
	ProjectID INT NOT NULL,

	CONSTRAINT PK_EmployeesProjects
	PRIMARY KEY (EmployeeID, ProjectID),

	CONSTRAINT FK_EmployeesProjects_Employees
	FOREIGN KEY (EmployeeID)
	REFERENCES Employees(EmployeeID),

	CONSTRAINT FK_EmployeesProjects_Projects
	FOREIGN KEY (ProjectID)
	REFERENCES Projects(ProjectID)
)
SELECT * FROM EmployeesProjects

INSERT INTO EmployeesProjects
VALUES
(1, 2),
(1, 3),
(2, 2)