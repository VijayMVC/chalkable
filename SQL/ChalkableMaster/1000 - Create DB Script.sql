Create Table [User]
(
	Id uniqueidentifier not null primary key,
	[Login] nvarchar(256) not null,
	[Password] nvarchar(256) not null
)
Create Table District
(
	Id uniqueidentifier not null primary key,
	Name nvarchar(256) not null
)
GO
Create Table School
(
	Id uniqueidentifier not null primary key,
	Name nvarchar(256) not null,
	ServerUrl nvarchar(256) not null,
	DistrictRef uniqueidentifier not null Constraint FK_School_District Foreign Key References District(Id)
)
GO
Create Table SchoolUser
(
	Id uniqueidentifier not null primary key,
	UserRef uniqueidentifier not null Constraint FK_SchoolUser_User Foreign Key References [User](Id),
	SchoolRef uniqueidentifier not null Constraint FK_SchoolUser_School Foreign Key References School(Id),
	[Role] int not null 
)
GO
Alter Table [User]
	Add Constraint UQ_Login unique ([Login])
GO
Alter Table [User]
	Add IsSysAdmin bit not null
GO
Alter Table [User]
	Add IsDeveloper bit not null
GO
Create Index IX_USER_LOGIN_PASSWORD
	on [User](Login, Password)
GO