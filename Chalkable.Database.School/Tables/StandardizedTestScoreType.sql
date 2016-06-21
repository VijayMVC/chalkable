CREATE TABLE [dbo].[StandardizedTestScoreType]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[StandardizedTestRef] INT NOT NULL CONSTRAINT [FK_StandardizedTestScoreType_StandardizedTest] FOREIGN KEY REFERENCES [StandardizedTest] ([Id]),
    [Name] NVARCHAR(30) NOT NULL, 
	[Description] NVARCHAR(MAX) NULL,
	[Code] NVARCHAR(30) NOT NULL,
	[StateCode] NVARCHAR(30) NULL, 
    [SifCode] NVARCHAR(30) NULL, 
    [NcesCode] NVARCHAR(30) NULL
)
