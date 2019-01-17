CREATE DATABASE Movies

CREATE TABLE Directors(
Id INT PRIMARY KEY IDENTITY NOT NULL,
DirectorName NVARCHAR(200) NOT NULL,
Notes NVARCHAR(MAX)
)

CREATE TABLE Genres(
Id INT PRIMARY KEY IDENTITY NOT NULL,
GenreName NVARCHAR(200) NOT NULL,
Notes NVARCHAR(MAX)
)

CREATE TABLE Categories(
Id INT PRIMARY KEY IDENTITY NOT NULL,
CategoryName NVARCHAR(200) NOT NULL,
Notes NVARCHAR(MAX)
)

CREATE TABLE Movies(
Id INT PRIMARY KEY IDENTITY NOT NULL,
Title NVARCHAR(200) NOT NULL,
DirectorId INT FOREIGN KEY REFERENCES Directors(Id) NOT NULL,
CopyrightYear INT NOT NULL,
Length INT NOT NULL,
GenreId INT FOREIGN KEY REFERENCES Genres(Id) NOT NULL,
CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
Rating NUMERIC(2, 1), 
Notes NVARCHAR(max),
)

INSERT INTO Directors(DirectorName, Notes) VALUES
('ßâîð Ãúðäåâ', 'Èçêëþ÷èòåëåí áúëãàðñêè ðåæèñüîð'),
	('Christopher Nolan', 'Best known for his cerebral, often nonlinear story-telling'),
	('Susanne Bier', 'Known for In a Better World (2010), After the Wedding (2006) and Brothers (2004).'),
	('Kathryn Bigelow', 'Director of The Hurt Locker'),
	('Ridley Scott', 'His reputation remains solidly intact.')

INSERT INTO Genres(GenreName) VALUES
('Drama'),('History'),('Thriller'),('Romance'),('Sci-Fi')
INSERT INTO Categories(CategoryName) VALUES
('R'),('PG-13'),('PG-18'),('Avoid at all cost'),('Hmmmm')

INSERT INTO Movies (Title, DirectorId, CopyrightYear, Length, GenreId, CategoryId, Rating, Notes)
VALUES
	('Gladiator', 1, 2000, 155, 2, 1, 8.5, NULL),
	('The Prestige', 2, 2006, 130, 3, 2, 8.5, 'One of my favourite movies'),
	('The Hurt Locker', 3, 2008, 131, 2, 1, 7.6, NULL),
	('After the Wedding', 1, 2006, 155, 1, 1, 7.8, 'Amazing performance from everyone'),
	('Äçèôò', 1, 2008, 120, 1, 1, 7.4, NULL)