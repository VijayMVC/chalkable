CREATE TABLE [dbo].[StandardizedTest]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Code] NVARCHAR(30) NOT NULL, 
    [Name] NVARCHAR(30) NOT NULL, 
    [DisplayName] NVARCHAR(30) NULL, 
    [Description] NVARCHAR(MAX) NULL,
    [GradeLevelRef] INT NULL CONSTRAINT [FK_StandardizedTest_GradeLevel] FOREIGN KEY REFERENCES [GradeLevel] ([Id]), 
    [StateCode] NVARCHAR(30) NULL, 
    [DisplayOnTranscript] BIT NULL, 
    [SifCode] NVARCHAR(30) NULL, 
    [NcesCode] NVARCHAR(30) NULL
)
