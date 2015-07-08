alter table [User] 
	add IsDistrictRegistrator bit null
GO


update 
	[User]
set 
	IsDistrictRegistrator = 0
where 
	[Login] <> 'InformationNow@Chalkable.com'
GO

update [User]
set 
	IsDistrictRegistrator = 1,
	IsSysAdmin = 0
where 
	[Login] = 'InformationNow@Chalkable.com'
GO

alter table [User] 
	alter column IsDistrictRegistrator bit not null
GO

Drop Type [TUser]
GO
CREATE TYPE [TUser] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Login] [nvarchar](256) NOT NULL,
	[Password] [nvarchar](256) NOT NULL,
	[FullName] [nvarchar](1024) NULL,
	[IsSysAdmin] [bit] NOT NULL,
	[IsDeveloper] [bit] NOT NULL,
	[IsAppTester] [bit] NOT NULL,
	IsDistrictRegistrator bit Not NULL,
	[ConfirmationKey] [nvarchar](256) NULL,
	[SisUserName] [nvarchar](256) NULL,
	[SisUserId] [int] NULL,
	[DistrictRef] [uniqueidentifier] NULL	
)
GO


