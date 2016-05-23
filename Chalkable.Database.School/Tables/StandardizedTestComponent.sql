CREATE TABLE [dbo].[StandardizedTestComponent]
(
	[Id] INT NOT NULL PRIMARY KEY, 
	[StandardizedTestRef] INT NOT NULL CONSTRAINT [FK_StandardizedTestComponent_StandardizedTest] FOREIGN KEY REFERENCES [StandardizedTest] ([Id]),
    [Name] NVARCHAR(30) NOT NULL, 
	[DisplayOnTranscript] BIT NULL, 
	[Code] NVARCHAR(30) NOT NULL,
	[StateCode] NVARCHAR(30) NULL, 
    [SifCode] NVARCHAR(30) NULL, 
    [NcesCode] NVARCHAR(30) NULL
)
