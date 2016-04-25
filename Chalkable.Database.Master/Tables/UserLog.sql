Create Table UserLog
(
	Id uniqueidentifier not null primary key,
	SisUserId int not null,
	DistrictRef uniqueidentifier not null,
	Added DateTime2 not null,
	Operation int not null
)
GO