CREATE TABLE [dbo].[Homeroom]
(
	[Id] INT NOT NULL, 
    [SchoolYearRef] INT NOT NULL, 
    [Name] NVARCHAR(64) NULL, 
    [TeacherRef] INT NULL, 
    [RoomRef] INT NULL,
	CONSTRAINT PK_Homeroom PRIMARY KEY(Id),
	CONSTRAINT FK_Homeroom_SchoolYear FOREIGN KEY(SchoolYearRef) REFERENCES SchoolYear(Id),
	CONSTRAINT FK_Homeroom_Staff FOREIGN KEY(TeacherRef) REFERENCES Staff(Id),
	CONSTRAINT FK_Homeroom_Room FOREIGN KEY(RoomRef) REFERENCES Room(Id)
)