Alter Table ClassPerson 
	add IsEnrolled bit
GO
Update ClassPerson set IsEnrolled = 1
GO		
Alter Table ClassPerson 
	Alter Column IsEnrolled bit not null