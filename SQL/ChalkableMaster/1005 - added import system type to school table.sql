Insert Into ImportSystemType
([Type], Name)
values
(3, 'STI')
GO

alter table School
	Add ImportSystemType int
GO