
CREATE TABLE Categories(
Id INT PRIMARY KEY IDENTITY NOT NULL,
CategoryName NVARCHAR(100) NOT NULL,
--check if decimal
DailyRate DECIMAL(15, 2) NOT NULL,
WeeklyRate DECIMAL(15, 2) NOT NULL,
MonthlyRate DECIMAL(15, 2) NOT NULL,
WeekendRate DECIMAL(15, 2) NOT NULL
)

INSERT INTO Categories(CategoryName, DailyRate, WeeklyRate, MonthlyRate, WeekendRate) VALUES
('burzak', 2, 3, 4, 5),
('tro6ka', 6, 6, 6, 6),
('fsdfsdfs', 5, 4, 2, 1)



CREATE TABLE Cars(
Id INT PRIMARY KEY IDENTITY NOT NULL,
PlateNumber NVARCHAR(10) NOT NULL,
Manufacturer NVARCHAR(50) NOT NULL,
Model NVARCHAR(50) NOT NULL,
CarYear INT NOT NULL,
CategoryId INT FOREIGN KEY REFERENCES Categories(Id) NOT NULL,
Doors INT NOT NULL,
Picture VARBINARY(MAX),
Condition NVARCHAR(50) NOT NULL,
Available char(3) NOT NULL CHECK(Available='yes' OR Available='no')
)

INSERT INTO Cars(PlateNumber, Manufacturer, Model, CarYear, CategoryId, Doors, Picture, Condition, Available) VALUES
('CA1536PM', 'Opel', 'Tigra', 2000, 1, 2, NULL, 'mnogo zapazena', 'yes'),
('K5364AP', 'BMW', 'E36', 2005, 2, 4, NULL, 'super', 'no'),
('CT6666CT', 'BMW', 'X6', 2016, 1, 4, NULL, 'super', 'yes')



CREATE TABLE Employees(
Id INT PRIMARY KEY IDENTITY NOT NULL,
FirstName NVARCHAR(50) NOT NULL,
LastName NVARCHAR(50) NOT NULL,
Title NVARCHAR(200) NOT NULL,
Notes NVARCHAR(MAX)
)

INSERT INTO Employees (FirstName, LastName, Title) VALUES
('Strahil', 'Iliev', 'maistora'),
('Ivan', 'Chilev', 'burzaka'),
('Gosho', 'Ivanov', 'fdsgfd')

CREATE TABLE Customers(
Id INT PRIMARY KEY IDENTITY NOT NULL,
DriverLicenseNumber NVARCHAR(10) NOT NULL,
FullName NVARCHAR(100) NOT NULL,
Address NVARCHAR(100) NOT NULL,
City NVARCHAR(100) NOT NULL,
ZIPCode NVARCHAR(20) NOT NULL,
Notes NVARCHAR(MAX)
)

INSERT INTO Customers (DriverLicenseNumber, FullName, Address, City, ZIPCode)
VALUES
('CT6666T', 'Ivan Goshev Pavlov', 'Stara Zagora', 'Stara Zagora', 6000),
('CB5525AA', 'Gosho Georgiev Chernev', 'Sofia', 'ul. Cherna 56', 1000),
('B5356AH', 'Ivan Petrov Georgiev', 'Varna', 'ul. Chernomorska 154', 7000)

CREATE TABLE RentalOrders(
Id INT PRIMARY KEY IDENTITY NOT NULL,
EmployeeId INT FOREIGN KEY REFERENCES Employees(Id) NOT NULL,
CustomerId INT FOREIGN KEY REFERENCES Customers(Id) NOT NULL,
CarId INT FOREIGN KEY REFERENCES Cars(Id) NOT NULL,
TankLevel INT NOT NULL,
KilometrageStart INT NOT NULL,
KilometrageEnd INT NOT NULL,
TotalKilometrage INT NOT NULL,
StartDate DATE NOT NULL,
EndDate DATE NOT NULL,
TotalDays INT NOT NULL,
RateApplied INT NOT NULL,
TaxRate DECIMAL(15, 2) NOT NULL,
OrderStatus NVARCHAR(50) NOT NULL,
Notes NVARCHAR(MAX)
)

INSERT INTO RentalOrders 
(EmployeeId, CustomerId, CarId, TankLevel, KilometrageStart, KilometrageEnd, TotalKilometrage, StartDate, EndDate, TotalDays, RateApplied, TaxRate, OrderStatus, Notes)
VALUES
(1, 1, 1, 22, 165000, 170000, 5000, '2019-11-11', '2019-11-22', 10, 55, 55, 'finished', 'very fast driver'),
(2, 2, 2, 25, 250000, 300000, 50000, '2008-08-08', '2009-01-11', 10, 10, 10, 'in progress', NULL),
(3, 3, 3, 30, 150000, 200000, 50000, '2018-07-08', '2009-01-11', 10, 10, 10, 'crashed', NULL)
