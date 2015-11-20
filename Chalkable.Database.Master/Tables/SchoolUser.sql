CREATE TABLE [dbo].[SchoolUser] (
    [SchoolRef]   INT              NOT NULL,
    [UserRef]     INT              NOT NULL,
    [DistrictRef] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_SchoolUser] PRIMARY KEY CLUSTERED ([SchoolRef] ASC, [UserRef] ASC, [DistrictRef] ASC),
    CONSTRAINT [FK_SchoolUser_District] FOREIGN KEY ([DistrictRef]) REFERENCES [dbo].[District] ([Id]),
    CONSTRAINT [FK_SchoolUser_School] FOREIGN KEY ([SchoolRef], [DistrictRef]) REFERENCES [dbo].[School] ([LocalId], [DistrictRef])
);




GO





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