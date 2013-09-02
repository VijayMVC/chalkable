Create Table ApplicationGradeLevel
(
	Id uniqueidentifier not null primary key,
	ApplicationRef uniqueidentifier not null Constraint FK_ApplicationGradeLevel_Application Foreign Key references Application(Id),
	GradeLevel int not null
)
GO
	