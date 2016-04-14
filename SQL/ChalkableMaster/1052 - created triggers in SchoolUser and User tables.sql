Create Trigger SchoolUserInsertTrigger
On SchoolUser
After Insert, Update
as 
	if not exists(
			Select * From [User] 
			join inserted on [User].SisUserId = inserted.UserRef and [User].DistrictRef = inserted.DistrictRef
		)
	Begin
		Throw 51000, 'You Can not update or insert SchoolUser. User is not exists with such SisUserId and DistrictRef', 1
	End
Go

Create Trigger UserDeleteTrigger
On [User]
After Delete
as
	If Exists(
			Select * From SchoolUser 
			join deleted on deleted.SisUserId = SchoolUser.UserRef and deleted.DistrictRef = SchoolUser.DistrictRef
		)
	Begin
		Throw 51001, 'User can not be deleted. SchoolUser has record with such SisUserId and DistrictRef', 1
	End

Go
