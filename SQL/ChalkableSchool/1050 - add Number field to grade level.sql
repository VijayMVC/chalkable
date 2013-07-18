Alter Table GradeLevel
	Add Number int
GO

Update GradeLevel set Number = 0

GO

Alter Table GradeLevel
	Alter Column Number int not null

Go
	