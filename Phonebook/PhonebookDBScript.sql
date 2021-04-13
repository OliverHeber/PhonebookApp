CREATE DATABASE [PhonebookDB]
USE [PhonebookDB]
GO
/****** Object:  Table [dbo].[Records]    Script Date: 13/04/2021 6:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Records](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Email] [varchar](250) NOT NULL,
	[Contact] [varchar](50) NOT NULL,
	[CREATED_BY] [varchar](200) NULL,
	[CREATED_ON] [datetime] NULL,
	[UPDATED_BY] [varchar](200) NULL,
	[UPDATED_ON] [datetime] NULL,
	[IS_ACTIVE] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Contact] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Records] ADD  DEFAULT ('System') FOR [CREATED_BY]
GO
ALTER TABLE [dbo].[Records] ADD  DEFAULT (getdate()) FOR [CREATED_ON]
GO
ALTER TABLE [dbo].[Records] ADD  DEFAULT ('System') FOR [UPDATED_BY]
GO
ALTER TABLE [dbo].[Records] ADD  DEFAULT (getdate()) FOR [UPDATED_ON]
GO
ALTER TABLE [dbo].[Records] ADD  DEFAULT ((1)) FOR [IS_ACTIVE]
GO
/****** Object:  StoredProcedure [dbo].[DeleteRecord]    Script Date: 13/04/2021 6:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteRecord]
	@Id int
AS
UPDATE Records
SET IS_ACTIVE = 0
WHERE Id = @Id
GO
/****** Object:  StoredProcedure [dbo].[GetAllRecords]    Script Date: 13/04/2021 6:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetAllRecords]
AS
SELECT * FROM Records
WHERE IS_ACTIVE = 1;
GO
/****** Object:  StoredProcedure [dbo].[GetRecord]    Script Date: 13/04/2021 6:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetRecord]
	--@RecordId is the paramater name
	@RecordId int
AS
SELECT * FROM Records 
	WHERE Id = @RecordId
	AND IS_ACTIVE = 1
GO
/****** Object:  StoredProcedure [dbo].[InsertRecord]    Script Date: 13/04/2021 6:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertRecord]
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
/****** Object:  StoredProcedure [dbo].[UpdateRecord]    Script Date: 13/04/2021 6:37:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UpdateRecord]
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
