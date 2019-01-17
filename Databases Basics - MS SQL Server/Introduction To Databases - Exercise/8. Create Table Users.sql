	CREATE TABLE Users(
		[Id] INT PRIMARY KEY IDENTITY,
		[Username] NVARCHAR(30) NOT NULL,
		[Password] NVARCHAR(26) NOT NULL,
		[ProfilePicture] VARBINARY(MAX),
		[LastLoginTime] DATE,
		IsDeleted BIT
	)

	INSERT INTO Users (Username, Password, ProfilePicture, LastLoginTime, IsDeleted)
	VALUES ('Mitko', '2356436345324', null, '1999-12-08', 'true'),
	('Jelqzko', '32423423543634', null, '2015-12-08', 'true'),
	('Jivko', 'sdfsdf', null, '2011-12-11', 'false'),
	('Yordan', 'yordan4o153324', null, '2018-12-26', 'false'),
	('Kostadin', '853453324234', null, '2019-01-01', 'true')