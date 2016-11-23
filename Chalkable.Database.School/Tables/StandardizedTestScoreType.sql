CREATE TABLE [dbo].[StandardizedTestScoreType]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[StandardizedTestRef] INT NOT NULL CONSTRAINT [FK_StandardizedTestScoreType_StandardizedTest] FOREIGN KEY REFERENCES [StandardizedTest] ([Id]),
    [Name] NVARCHAR(64) NOT NULL, 
	[Description] NVARCHAR(MAX) NULL,
	[Code] NVARCHAR(64) NOT NULL,
	[StateCode] NVARCHAR(64) NULL, 
    [SifCode] NVARCHAR(64) NULL, 
    [NcesCode] NVARCHAR(64) NULL
)
