-- CTRL+ENTER For whole script
-- CTRL+SHIFT+ENTER run selected code
-- CREATE SCHEMA Phonebook;

CREATE TABLE Records(
	Id INT auto_increment PRIMARY KEY,
	Name VARCHAR(100) NOT NULL,
	Email VARCHAR(250) NOT NULL UNIQUE,
	Contact VARCHAR(50) NOT NULL,
	CREATED_BY VARCHAR(200) DEFAULT 'System',
	CREATED_ON DATETIME DEFAULT now(),
	UPDATED_BY VARCHAR(200) DEFAULT 'System',
	UPDATED_ON DATETIME DEFAULT now(),
	IS_ACTIVE BIT DEFAULT 1
);

select * from Records

delimiter //
CREATE PROCEDURE GetAllRecords()
BEGIN
SELECT * FROM Records
WHERE IS_ACTIVE = 1;
END //
delimiter ;

delimiter //
CREATE PROCEDURE GetRecord(RecordId int)
BEGIN
SELECT * FROM Records 
	WHERE Id = RecordId
	AND IS_ACTIVE = 1;
END //
delimiter ;

delimiter //
CREATE PROCEDURE InsertRecord(NameVal VARCHAR(100), EmailVal VARCHAR(250), ContactVal VARCHAR(50))
BEGIN
INSERT INTO Records(
	Name,
	Email, 
	Contact)
	VALUES(
	NameVal, 
	EmailVal,
	ContactVal);
END //
delimiter ;

delimiter //
CREATE PROCEDURE DeleteRecord(IdVal int)
BEGIN
UPDATE Records
SET IS_ACTIVE = 0
WHERE Id = IdVal;
END //
delimiter ;

delimiter //
CREATE PROCEDURE UpdateRecord(
	IdVal int,
	NameVal VARCHAR(100),
	EmailVal VARCHAR(250),
	ContactVal VARCHAR(50))
BEGIN
UPDATE Records
SET Name = NameVal,
	Email = EmailVal,
	Contact = ContactVal
WHERE Id = Id;
END //
delimiter ;






