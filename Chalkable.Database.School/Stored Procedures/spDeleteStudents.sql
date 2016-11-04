Create Procedure spDeleteStudents
	@ids TInt32 ReadOnly
As

Delete From SupplementalAnnouncementRecipient
Where StudentRef in (Select * From @ids)

Delete From StudentAnnouncementApplicationMeta
Where StudentRef in (Select * From @ids)

Delete From AdminAnnouncementStudent
Where StudentRef in (Select * From @ids)

Delete From StudentGroup
Where StudentRef in (Select * From @ids)

Delete From StudentCustomAlertDetail
Where StudentRef in (Select * From @ids)

Delete From AutoGrade
Where StudentRef in (Select * From @ids)

Delete From Student
Where Id in (Select * From @ids)

Go