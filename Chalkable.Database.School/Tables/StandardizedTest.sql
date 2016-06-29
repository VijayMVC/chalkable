CREATE TABLE [dbo].[StandardizedTest]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Code] NVARCHAR(64) NOT NULL, 
    [Name] NVARCHAR(64) NOT NULL, 
    [DisplayName] NVARCHAR(64) NULL, 
    [Description] NVARCHAR(MAX) NULL,
    [GradeLevelRef] INT NULL CONSTRAINT [FK_StandardizedTest_GradeLevel] FOREIGN KEY REFERENCES [GradeLevel] ([Id]), 
    [StateCode] NVARCHAR(64) NULL, 
    [DisplayOnTranscript] BIT NULL, 
    [SifCode] NVARCHAR(64) NULL, 
    [NcesCode] NVARCHAR(64) NULL
)
