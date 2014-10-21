create table UserLoginInfo
(
	Id uniqueidentifier not null primary key constraint FK_UserLoginInfo_User foreign key references [User](Id),
	SisToken nvarchar(max),
	SisTokenExpires datetime2 null,
    LastPasswordReset datetime2 null
)
go
create type TUserLoginInfo as table
(
	Id uniqueidentifier not null,
	SisToken nvarchar(max),
	SisTokenExpires datetime2 null,
    LastPasswordReset datetime2 null
)
go
