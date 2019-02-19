/****** Section 1. DDL (30 pts) ******/
CREATE TABLE Planets
(
	Id          INT PRIMARY KEY IDENTITY,
	Name		VARCHAR(30) NOT NULL
)

CREATE TABLE Spaceports
(
	Id          INT PRIMARY KEY IDENTITY,
	Name		VARCHAR(50) NOT NULL,
	PlanetId	INT FOREIGN KEY REFERENCES Planets(Id) NOT NULL
)

CREATE TABLE Spaceships
(
	Id             INT PRIMARY KEY IDENTITY,
	Name		   VARCHAR(50) NOT NULL,
	Manufacturer   VARCHAR(30) NOT NULL,
	LightSpeedRate INT DEFAULT 0
)

CREATE TABLE Colonists
(
	Id              INT PRIMARY KEY IDENTITY,
	FirstName		VARCHAR(20) NOT NULL,
	LastName		VARCHAR(20) NOT NULL,
	Ucn				VARCHAR(10) UNIQUE NOT NULL,
	BirthDate		DATE NOT NULL
)

CREATE TABLE Journeys
(
	Id              INT PRIMARY KEY IDENTITY,
	JourneyStart	DATETIME NOT NULL,
	JourneyEnd		DATETIME NOT NULL,
	Purpose			VARCHAR(11) CHECK (Purpose IN ('Medical', 'Technical', 'Educational', 'Military')),
	DestinationSpaceportId INT FOREIGN KEY REFERENCES Spaceports(Id) NOT NULL,
	SpaceshipId INT FOREIGN KEY REFERENCES Spaceships(Id) NOT NULL
)

CREATE TABLE TravelCards
(
	Id               INT PRIMARY KEY IDENTITY,
	CardNumber		 VARCHAR(10) UNIQUE NOT NULL,
	JobDuringJourney VARCHAR(8) CHECK (JobDuringJourney IN ('Pilot', 'Engineer', 'Trooper', 'Cleaner', 'Cook')),
	ColonistId		 INT FOREIGN KEY REFERENCES Colonists(Id) NOT NULL,
	JourneyId		 INT FOREIGN KEY REFERENCES Journeys(Id) NOT NULL
)
/****** Section 2. DML (10 pts) ******/
--2.insert
INSERT INTO Planets (Name) VALUES
 ('Mars'),
 ('Earth'),
 ('Jupiter'),
 ('Saturn')

INSERT INTO Spaceships (Name, Manufacturer, LightSpeedRate)
VALUES
('Golf',	'VW',	3),
('WakaWaka',	'Wakanda',	4),
('Falcon9',	'SpaceX',	1),
('Bed',	'Vidolov',	6)

--3.update
UPDATE Spaceships
SET LightSpeedRate += 1
WHERE Id BETWEEN 8 AND 12

--4.delete
DELETE FROM TravelCards
WHERE JourneyId IN (1, 2, 3)
DELETE TOP(3) FROM Journeys

/****** Section 3. Querying (40 pts) ******/
--5.	Select all travel cards
SELECT CardNumber, JobDuringJourney
  FROM TravelCards
  ORDER BY CardNumber
 
--6.	Select all colonists
SELECT Id, FirstName + ' ' + LastName AS FullName, Ucn
  FROM Colonists
  ORDER BY FirstName, LastName, Id

--7.	Select all military journeys
SELECT  Id, 
		CONVERT(VARCHAR(10), JourneyStart, 103) AS JourneyStart, 
		CONVERT(VARCHAR(10), JourneyEnd, 103) AS JourneyEnd
	FROM Journeys
	WHERE Purpose = 'Military'
	ORDER BY JourneyStart

--8.	Select all pilots
SELECT c.Id, FirstName + ' ' + LastName AS FullName
	FROM Colonists AS c
	JOIN TravelCards AS tc ON tc.ColonistId = c.Id
	WHERE tc.JobDuringJourney = 'Pilot'
	ORDER BY c.Id

--9.	Count colonists
SELECT Count(c.Id) AS count
	FROM Colonists AS c
	JOIN TravelCards AS tc ON tc.ColonistId = c.Id
	JOIN Journeys AS j ON j.Id = tc.JourneyId
	WHERE j.Purpose = 'Technical'

--10.	Select the fastest spaceship
SELECT TOP(1) ss.Name AS SpaceshipName, sp.Name AS SpaceportName
	FROM Spaceships AS ss
	JOIN Journeys AS j ON j.SpaceshipId = ss.Id
	JOIN Spaceports AS sp ON j.DestinationSpaceportId = sp.Id
	ORDER BY ss.LightSpeedRate DESC

--11.	Select spaceships with pilots younger than 30 years
SELECT DISTINCT ss.Name, ss.Manufacturer
	FROM Colonists AS c
	JOIN TravelCards AS tc ON tc.ColonistId = c.Id
	JOIN Journeys AS j ON tc.JourneyId = j.Id
	JOIN Spaceships AS ss ON j.SpaceshipId = ss.Id
	WHERE tc.JobDuringJourney = 'Pilot' AND 
		  DATEDIFF(YEAR, c.BirthDate, '01/01/2019') < 30
	ORDER BY ss.Name

--12.	Select all educational mission planets and spaceports
SELECT p.Name AS PlanetName, sp.Name AS SpaceportName
	FROM Planets AS p
	JOIN Spaceports AS sp ON sp.PlanetId = p.Id
	JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id
	WHERE j.Purpose = 'Educational'
	ORDER BY sp.Name DESC

--13.	Select all planets and their journey count
SELECT p.Name AS PlanetName, COUNT(*) AS JourneysCount
	FROM Planets AS p
	JOIN Spaceports AS sp ON sp.PlanetId = p.Id
	JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id
	GROUP BY p.Name
	ORDER BY JourneysCount DESC, p.Name

--14.	Select the shortest journey
SELECT TOP(1) k.Id, k.PlanetName, k.SpaceportName, k.JourneyPurpose 
FROM (
	SELECT j.Id, p.Name AS PlanetName, sp.Name AS SpaceportName, j.Purpose AS JourneyPurpose, DATEDIFF(MONTH, j.JourneyStart, j.JourneyEnd) AS Months
		FROM Planets AS p
		JOIN Spaceports AS sp ON sp.PlanetId = p.Id
		JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id) AS k
		ORDER BY Months ASC

--15.	Select the less popular job
SELECT TOP(1) k.Id, k.JobDuringJourney AS JobName 
FROM (
	SELECT j.Id, p.Name AS PlanetName, sp.Name AS SpaceportName, j.Purpose AS JourneyPurpose, DATEDIFF(MONTH, j.JourneyStart, j.JourneyEnd) AS Months, tc.JobDuringJourney
		FROM Planets AS p
		JOIN Spaceports AS sp ON sp.PlanetId = p.Id
		JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id
		JOIN TravelCards AS tc ON tc.JourneyId = j.Id) AS k
		ORDER BY Months DESC

--16.	Select Second Oldest Important Colonist
SELECT k.JobDuringJourney, k.FullName, k.JobRank
FROM (
	SELECT JobDuringJourney, 
		   c.FirstName + ' ' + c.LastName AS FullName, 
		   ROW_NUMBER() OVER (PARTITION BY JobDuringJourney ORDER BY BirthDate) AS JobRank
			FROM Planets AS p
			JOIN Spaceports AS sp ON sp.PlanetId = p.Id
			JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id
			JOIN TravelCards AS tc ON tc.JourneyId = j.Id
			JOIN Colonists AS c ON tc.ColonistId = c.Id) AS k
			WHERE k.JobRank = 2

--17.	Planets and Spaceports
SELECT p.Name, Count(sp.PlanetId) AS [Count]
	FROM Planets AS p
	LEFT JOIN Spaceports AS sp ON sp.PlanetId = p.Id
	GROUP BY p.Name
	ORDER BY Count DESC, p.Name

--18.	Get Colonists Count
CREATE FUNCTION dbo.udf_GetColonistsCount(@PlanetName VARCHAR(30))
	RETURNS INT
AS
	BEGIN
		DECLARE @Result INT = (SELECT COUNT(c.Id)
		FROM Planets AS p
		JOIN Spaceports AS sp ON sp.PlanetId = p.Id
		JOIN Journeys AS j ON j.DestinationSpaceportId = sp.Id
		JOIN TravelCards AS tc ON tc.JourneyId = j.Id
		JOIN Colonists AS c ON tc.ColonistId = c.Id
		WHERE p.Name = @PlanetName)

		RETURN @Result
	END


--19.	Change Journey Purpose
CREATE PROCEDURE usp_ChangeJourneyPurpose(@JourneyId INT, @NewPurpose VARCHAR(11))
AS
BEGIN
	DECLARE @journey INT = (SELECT Id FROM Journeys WHERE Id = @JourneyId)

	IF (@journey IS NULL)
	BEGIN
		;THROW 51000, 'The journey does not exist!', 1
	END

	DECLARE @purpose VARCHAR(11) = (SELECT Purpose FROM Journeys WHERE Id = @JourneyId)
	IF (@purpose = @NewPurpose)
	BEGIN
		;THROW 51000, 'You cannot change the purpose!', 1
	END

	UPDATE Journeys
	SET Purpose = @NewPurpose
	WHERE Id = @JourneyId
END

EXEC usp_ChangeJourneyPurpose 1, 'Technical'
SELECT * FROM Journeys

EXEC usp_ChangeJourneyPurpose 2, 'Educational'

EXEC usp_ChangeJourneyPurpose 196, 'Technical'

--20.	Deleted Journeys
CREATE TABLE DeletedJourneys
(
Id INT, JourneyStart DATETIME, JourneyEnd DATETIME, Purpose VARCHAR(11), DestinationSpaceportId INT, SpaceshipId INT
)

CREATE TRIGGER t_DeleteJourneys
	ON Journeys AFTER DELETE
	AS
	BEGIN
		INSERT INTO DeletedJourneys (Id, JourneyStart, JourneyEnd, Purpose, DestinationSpaceportId, SpaceshipId)
		SELECT d.Id, d.JourneyStart, d.JourneyEnd, d.Purpose, d.DestinationSpaceportId, d.SpaceshipId
			FROM deleted AS d
	END

DELETE FROM TravelCards
WHERE JourneyId =  1

DELETE FROM Journeys
WHERE Id =  1
