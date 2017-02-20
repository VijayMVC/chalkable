CREATE TYPE [dbo].[THomeroom] AS TABLE
(
	[Id] INT, 
    [SchoolYearRef] INT, 
    [Name] NVARCHAR(64), 
    [TeacherRef] INT, 
    [RoomRef] INT
)
