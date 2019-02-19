/****** Section 1. DDL (30 pts) ******/
--check not null age
CREATE TABLE Students
(
	Id          INT PRIMARY KEY IDENTITY,
	FirstName	NVARCHAR(30) NOT NULL,
	MiddleName	NVARCHAR(25),
	LastName	NVARCHAR(30) NOT NULL,
	Age			INT CHECK(Age BETWEEN 5 AND 100),
	Address		NVARCHAR(50),
	Phone		NVARCHAR(10)
)

CREATE TABLE Subjects
(
	Id          INT PRIMARY KEY IDENTITY,
	Name		NVARCHAR(20) NOT NULL,
	Lessons		INT CHECK(Lessons > 0) NOT NULL
)

CREATE TABLE StudentsSubjects
(
	Id          INT PRIMARY KEY IDENTITY,
	StudentId	INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	SubjectId	INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL,
	Grade		DECIMAL(15, 2) CHECK (Grade BETWEEN 2 AND 6) NOT NULL
)

CREATE TABLE Exams
(
	Id          INT PRIMARY KEY IDENTITY,
	Date		DATETIME,
	SubjectId	INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL
)

CREATE TABLE StudentsExams
(
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	ExamId	INT FOREIGN KEY REFERENCES Exams(Id) NOT NULL,
	Grade		DECIMAL(15, 2) CHECK (Grade BETWEEN 2 AND 6) NOT NULL
	CONSTRAINT PK_StudentsExams PRIMARY KEY (StudentId, ExamId)
)

CREATE TABLE Teachers
(
	Id          INT PRIMARY KEY IDENTITY,
	FirstName	NVARCHAR(20) NOT NULL,
	LastName	NVARCHAR(20) NOT NULL,
	Address		NVARCHAR(20) NOT NULL,
	Phone		NVARCHAR(10),
	SubjectId	INT FOREIGN KEY REFERENCES Subjects(Id) NOT NULL
)

CREATE TABLE StudentsTeachers
(
	StudentId INT FOREIGN KEY REFERENCES Students(Id) NOT NULL,
	TeacherId INT FOREIGN KEY REFERENCES Teachers(Id) NOT NULL,

	CONSTRAINT PK_StudentsTeachers PRIMARY KEY (StudentId, TeacherId)
)

/****** Section 2. DML (10 pts) ******/
/****** 2. Insert ******/
INSERT INTO Teachers (FirstName, LastName, Address, Phone, SubjectId)
VALUES
('Ruthanne',	'Bamb',	'84948 Mesta Junction',	3105500146,	6),
('Gerrard',	'Lowin',	'370 Talisman Plaza',	3324874824,	2),
('Merrile',	'Lambdin',	'81 Dahle Plaza',	4373065154,	5),
('Bert',	'Ivie',	'2 Gateway Circle',	4409584510,	4)

INSERT INTO Subjects (Name, Lessons)
VALUES
('Geometry',	12),
('Health',	10),
('Drama',	7),
('Sports',	9)

/****** 3. Update ******/
UPDATE StudentsSubjects
SET Grade = 6.00
WHERE SubjectId IN (1, 2) AND Grade >= 5.50

/****** 4. Delete ******/
DELETE FROM StudentsTeachers
WHERE TeacherId IN (7, 12, 15, 18, 24, 26)
DELETE FROM Teachers
WHERE Phone LIKE '%72%'


/****** Section 3. Querying (40 pts) ******/
/****** 5. Teen Students ******/
SELECT FirstName, LastName, Age 
	FROM Students
	WHERE Age >= 12
	ORDER BY FirstName, LastName

/****** 6. Cool Addresses ******/
SELECT 
FirstName + ' ' + ISNULL(MiddleName, '') + ' ' + LastName AS [Full Name],
Address
	FROM Students
	WHERE Address LIKE '%road%'
	ORDER BY FirstName, LastName, Address

/****** 7. 42 Phones ******/
SELECT FirstName, Address, Phone
	FROM Students
	WHERE MiddleName IS NOT NULL AND
		  Phone LIKE '42%'
	ORDER BY FirstName

/****** 8. Students Teachers ******/
SELECT s.FirstName, s.LastName, COUNT(st.TeacherId) AS TeachersCount
	FROM StudentsTeachers AS st
	JOIN Students AS s ON s.Id = st.StudentId
	GROUP BY s.FirstName, s.LastName

/****** 9. Subjects with Students ******/
SELECT t.FirstName + ' ' + t.LastName AS [FullName],
	   CONCAT(s.Name, '-', s.Lessons) AS Subjects,
	   COUNT(st.StudentId) AS Students
	FROM Teachers AS t
	JOIN StudentsTeachers AS st ON st.TeacherId = t.Id
	JOIN Subjects AS s ON t.SubjectId = s.Id
	GROUP BY t.FirstName, t.LastName, s.Name, s.Lessons
	ORDER BY Students DESC

/****** 10. Students to Go ******/
SELECT s.FirstName + ' ' + s.LastName AS [Full Name]
	FROM Students AS s 
	LEFT JOIN StudentsExams AS se ON se.StudentId = s.Id
	LEFT JOIN Exams AS e ON se.ExamId = e.Id
	WHERE se.StudentId IS NULL AND
		  se.ExamId IS NULL
	ORDER BY s.FirstName, s.LastName


/****** 11. Busiest Teachers ******/
SELECT TOP(10) t.FirstName, t.LastName, COUNT(st.StudentId) AS StudentsCount
	FROM StudentsTeachers AS st
	LEFT JOIN Teachers AS t ON st.TeacherId = t.Id
	GROUP BY t.FirstName, t.LastName
	ORDER BY StudentsCount DESC, t.FirstName, t.LastName

/****** 12. Top Students ******/

--add top(10)
--CONVERT(DECIMAL(10,2),YOURCOLUMN)
SELECT TOP(10) k.FirstName, k.LastName, CONVERT(DECIMAL(10,2), MAX(k.Grade)) AS Grade
FROM(
	SELECT s.FirstName, s.LastName, AVG(se.Grade) AS Grade
		FROM StudentsExams AS se
		JOIN Students AS s ON se.StudentId = s.Id
		GROUP BY s.FirstName, s.LastName
		) AS k
		GROUP BY k.FirstName, k.LastName, k.Grade
		ORDER BY k.Grade DESC, k.FirstName, k.LastName

/****** 13. Second Highest Grade ******/
SELECT k.FirstName, k.LastName, k.Grade
	FROM (
	SELECT s.FirstName, s.LastName, ss.Grade,
			ROW_NUMBER() OVER (PARTITION BY s.FirstName, s.LastName ORDER BY ss.Grade DESC) AS TopGrade
			FROM StudentsSubjects AS ss
			JOIN Students AS s ON ss.StudentId = s.Id
	) AS k
	WHERE k.TopGrade = 2
	ORDER BY k.FirstName, k.LastName
/****** 14. Not So In The Studying ******/
SELECT CONCAT(s.FirstName, ' ' + s.MiddleName, ' ', s.LastName) AS [Full Name]
	FROM Students AS s
	LEFT JOIN StudentsSubjects AS ss ON ss.StudentId = s.Id
	WHERE ss.StudentId IS NULL AND ss.SubjectId IS NULL AND ss.Grade IS NULL
	ORDER BY [Full Name]

/****** 15. Top Student per Teacher ******/
SELECT k.TeacherName, k.SubjectName, k.StudentName, MAX(k.AVGGrade) AS Grade
	FROM (
	SELECT t.FirstName + ' ' + t.LastName AS TeacherName,
		   sub.Name AS SubjectName,
		   CONCAT(s.FirstName, ' ' + s.MiddleName, ' ', s.LastName) AS StudentName,
		   AVG(ss.Grade) AS AVGGrade
		FROM StudentsTeachers AS st
		JOIN Teachers AS t ON st.TeacherId = t.Id
		JOIN Students AS s ON st.StudentId = s.Id
		JOIN Subjects AS sub ON t.SubjectId = sub.Id
		JOIN StudentsSubjects AS ss ON ss.StudentId = s.Id
		GROUP BY t.FirstName, t.LastName, sub.Name, s.FirstName, s.MiddleName, s.LastName, ss.Grade
		ORDER BY SubjectName, TeacherName, Grade DESC
		) AS k
		GROUP BY k.TeacherName, k.SubjectName, k.StudentName
		ORDER BY SubjectName, TeacherName, Grade DESC


SELECT t.FirstName + ' ' + t.LastName AS TeacherName,
	   sub.Name,
	   CONCAT(s.FirstName, ' ' + s.MiddleName, ' ', s.LastName) AS StudentName,
	   ss.Grade
	FROM StudentsSubjects AS ss
	LEFT JOIN Students AS s ON ss.StudentId = s.Id
	LEFT JOIN Teachers AS t ON t.SubjectId = ss.SubjectId
	LEFT JOIN Subjects AS sub ON sub.Id = ss.SubjectId
	WHERE t.FirstName = 'Farleigh'
	ORDER BY ss.Grade DESC



	--max(grade)
SELECT k.TeacherName, k.Name, k.StudentName, k.AVGGrade / k.GradeCount
	FROM (
	SELECT t.FirstName + ' ' + t.LastName AS TeacherName,
		   sub.Name,
		   CONCAT(s.FirstName, ' ' + s.MiddleName, ' ', s.LastName) AS StudentName,
		   SUM(Grade) AS AVGGrade,
		   COUNT(ss.Grade) AS GradeCount
			FROM StudentsSubjects AS ss
			JOIN Students AS s ON ss.StudentId = s.Id
			JOIN StudentsTeachers AS st ON st.StudentId = s.Id
			JOIN Teachers AS t ON st.TeacherId = t.Id
			JOIN Subjects AS sub ON t.SubjectId = sub.Id
			GROUP BY t.FirstName, t.LastName, sub.Name, s.FirstName, s.MiddleName, s.LastName
		) AS k
		GROUP BY k.TeacherName, k.Name, k.StudentName, k.AVGGrade, k.GradeCount
		ORDER BY k.Name, k.TeacherName, k.AVGGrade DESC


SELECT MAX(k.grade1)
	FROM (
	SELECT AVG(Grade) AS grade1
			FROM StudentsSubjects AS ss
			JOIN Students AS s ON ss.StudentId = s.Id
			JOIN StudentsTeachers AS st ON st.StudentId = s.Id
			JOIN Teachers AS t ON st.TeacherId = t.Id
			JOIN Subjects AS sub ON t.SubjectId = sub.Id
			WHERE s.FirstName = 'Merrill' AND t.FirstName = 'Ruthanne'
		) as k


		--CONVERT(DECIMAL(10,2),YOURCOLUMN)

SELECT k.TeacherName, k.Name AS [SubjectName], k.StudentName, CONVERT(DECIMAL(10,2), k.AvgGrade) AS Grade
	FROM (
	SELECT t.FirstName + ' ' + t.LastName AS			 TeacherName,
		   sub.Name,
		   s.FirstName + ' ' + s.LastName AS StudentName,
		   AVG(ss.Grade) AS AvgGrade,
		   ROW_NUMBER() OVER (PARTITION BY t.FirstName, t.LastName ORDER BY AVG(ss.Grade) DESC) AS [GradeRank]
		FROM Teachers AS t
		JOIN Subjects AS sub ON t.SubjectId = sub.Id
		JOIN StudentsSubjects AS ss ON ss.SubjectId = sub.Id
		JOIN Students AS s ON ss.StudentId = s.Id
		JOIN StudentsTeachers AS st ON st.TeacherId = t.Id AND st.StudentId = s.Id
		GROUP BY t.FirstName, t.LastName, sub.Name, s.FirstName, s.LastName
	) AS k
		WHERE k.GradeRank = 1
		GROUP BY k.TeacherName, k.Name, k.StudentName, k.AvgGrade
		ORDER BY k.Name, k.TeacherName, k.AvgGrade DESC
		
	




/****** 16. Average Grade per Subject ******/
SELECT s.Name, AVG(ss.Grade) AS AverageGrade
	FROM Subjects AS s
	JOIN StudentsSubjects AS ss ON s.Id = ss.SubjectId
	GROUP BY s.Name, s.Id
	ORDER BY s.Id

/****** 17. Exams Information ******/
SELECT k.Quarter, k.SubjectName, SUM(k.StudentsCount) AS StudentsCount
	FROM (
	SELECT 
		CASE
		WHEN DATEPART(MONTH, e.Date) BETWEEN 1 AND 3 THEN 'Q1'
		WHEN DATEPART(MONTH, e.Date) BETWEEN 4 AND 6 THEN 'Q2'
		WHEN DATEPART(MONTH, e.Date) BETWEEN 7 AND 9 THEN 'Q3'
		WHEN DATEPART(MONTH, e.Date) BETWEEN 10 AND 12 THEN 'Q4'
		ELSE 'TBA'
		END AS [Quarter],
		s.Name AS SubjectName,
		COUNT(se.StudentId) AS StudentsCount
		FROM Exams AS e
		JOIN StudentsExams AS se ON se.ExamId = e.Id
		JOIN Subjects AS s ON e.SubjectId = s.Id
		WHERE se.Grade >= 4.00
		GROUP BY e.Date, s.Name ) AS k
		GROUP BY k.Quarter, k.SubjectName
		ORDER BY k.Quarter, k.SubjectName

/****** Section 4. Programmability (20 pts) ******/
/****** 18. Exam Grades ******/
CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(3,2))
	RETURNS NVARCHAR(MAX)
AS
BEGIN
	IF (@grade > 6.00)
	BEGIN
		RETURN 'Grade cannot be above 6.00!' 
	END

	DECLARE @student INT = (SELECT TOP(1) Id FROM Students WHERE Id = @studentId)
	IF (@student IS NULL)
	BEGIN
		RETURN 'The student with provided id does not exist in the school!'
	END

	

	DECLARE @studentName NVARCHAR(MAX) = (SELECT TOP(1)FirstName FROM Students WHERE Id = @studentId)

	DECLARE @result INT = (
		SELECT TOP(1) COUNT(se.Grade) 
			FROM StudentsExams AS se
			WHERE se.Grade BETWEEN @grade AND @grade + 0.50
			GROUP BY se.StudentId
	)
	RETURN 'You have to update ' + CAST(@result AS NVARCHAR(MAX)) + ' grades for the student ' + @studentName
END


SELECT dbo.udf_ExamGradesToUpdate(12, 6.20)

SELECT dbo.udf_ExamGradesToUpdate(12, 5.50)

SELECT dbo.udf_ExamGradesToUpdate(121, 5.50)

/****** 19. Exclude from school ******/
CREATE PROCEDURE usp_ExcludeFromSchool(@StudentId INT)
AS
BEGIN
	DECLARE @student INT = (SELECT Id FROM Students WHERE Id = @StudentId)
	IF (@student IS NULL)
	BEGIN
		;THROW 51000, 'This school has no student with the provided id!', 1
	END

	DELETE FROM StudentsTeachers
	WHERE StudentId = @StudentId
	DELETE FROM StudentsExams
	WHERE StudentId = @StudentId
	DELETE FROM StudentsSubjects
	WHERE StudentId = @StudentId
	DELETE FROM Students
	WHERE Id = @StudentId
END

EXEC usp_ExcludeFromSchool 1
SELECT COUNT(*) FROM Students
EXEC usp_ExcludeFromSchool 301

/****** 20. Deleted Student ******/
CREATE TABLE ExcludedStudents
(
	StudentId INT,
	StudentName NVARCHAR(100)
)

CREATE TRIGGER t_DeleteStudent
	ON Students FOR DELETE
	AS
	BEGIN
		INSERT INTO ExcludedStudents (StudentId, StudentName)
		SELECT d.Id, d.FirstName + ' ' + d.LastName
			FROM deleted AS d
	END



DELETE FROM StudentsExams
WHERE StudentId = 1

DELETE FROM StudentsTeachers
WHERE StudentId = 1

DELETE FROM StudentsSubjects
WHERE StudentId = 1

DELETE FROM Students
WHERE Id = 1

SELECT * FROM ExcludedStudents
