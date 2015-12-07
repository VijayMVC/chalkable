Create Table SisUser
(
	Id int not null primary key,
	UserName nvarchar(127) not null,
	LockedOut bit not null,
	IsDisabled bit not null,
	IsSystem bit not null
)
GO