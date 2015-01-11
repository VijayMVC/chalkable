Alter Table [dbo].[User]
	Add IsAppTester bit
GO
Update 
	[User]
Set 
	IsAppTester = 0
GO
Alter Table [dbo].[User]
	Alter Column IsAppTester bit not null
GO
DROP TYPE [dbo].[TUser] 
GO
CREATE TYPE [dbo].[TUser] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Login] [nvarchar](256) NOT NULL,
	[Password] [nvarchar](256) NOT NULL,
	[FullName] [nvarchar](1024) NULL,
	[IsSysAdmin] [bit] NOT NULL,
	[IsDeveloper] [bit] NOT NULL,
	[IsAppTester] [bit] NOT NULL,
	[ConfirmationKey] [nvarchar](256) NULL,
	[SisUserName] [nvarchar](256) NULL,
	[SisUserId] [int] NULL,
	[DistrictRef] [uniqueidentifier] NULL
)
GO