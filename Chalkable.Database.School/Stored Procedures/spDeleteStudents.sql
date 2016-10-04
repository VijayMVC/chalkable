Create Procedure spDeleteStudents
	@ids TInt32 ReadOnly
As

Delete From StudentAnnouncementApplicationMeta
Where StudentRef in(select * from @ids)

Delete From StudentGroup
Where StudentRef in(select * from @ids)

Delete From Student
Where Id in(select * from @ids)

Go