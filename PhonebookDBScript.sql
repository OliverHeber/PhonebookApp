CREATE DATABASE PhonebookDB;

USE PhonebookDB;

GO

CREATE TABLE Records(
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Name VARCHAR(100) NOT NULL,
	Email VARCHAR(250) NOT NULL UNIQUE,
	Contact VARCHAR(50) NOT NULL,
	CREATED_BY VARCHAR(200) DEFAULT 'System',
	CREATED_ON DATETIME DEFAULT getdate(),
	UPDATED_BY VARCHAR(200) DEFAULT 'System',
	UPDATED_ON DATETIME DEFAULT getdate(),
	IS_ACTIVE BIT DEFAULT 1
);
GO

--Function with no parameters
ALTER PROCEDURE GetAllRecords
AS
SELECT * FROM Records
WHERE IS_ACTIVE = 1;
GO

--Stored procedure with a parameter
ALTER PROCEDURE GetRecord
	--@RecordId is the paramater name
	@RecordId int
AS
SELECT * FROM Records 
	WHERE Id = @RecordId
	AND IS_ACTIVE = 1
GO

EXECUTE GetRecord 2

CREATE PROCEDURE InsertRecord
	@Name VARCHAR(100),
	@Email VARCHAR(250),
	@Contact VARCHAR(50)
AS
INSERT INTO Records(
	[Name],
	Email, 
	Contact)
	VALUES(
	@Name, 
	@Email,
	@Contact)
GO

ALTER PROCEDURE DeleteRecord
	@Id int
AS
UPDATE Records
SET IS_ACTIVE = 0
WHERE Id = @Id
GO

CREATE PROCEDURE UpdateRecord
	@Id int,
	@Name VARCHAR(100),
	@Email VARCHAR(250),
	@Contact VARCHAR(50)
AS
UPDATE Records
SET [Name] = @Name,
	Email = @Email,
	Contact = @Contact
WHERE Id = @Id
GO
