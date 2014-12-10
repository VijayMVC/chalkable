Alter Table [User]
	Drop Column LocalId
GO
Alter Table [User]
	Drop Column SisToken
GO
Alter Table [User]
	Drop Column SisTokenExpires
GO
Alter Table [User]
	Add SisUserId int
GO
Alter Table [User]
	Add FullName nvarchar(1024)
GO
Drop type TUser
GO
CREATE TYPE [dbo].[TUser] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Login] [nvarchar](256) NOT NULL,
	[Password] [nvarchar](256) NOT NULL,
	FullName [nvarchar](1024),
	[IsSysAdmin] [bit] NOT NULL,
	[IsDeveloper] [bit] NOT NULL,
	[ConfirmationKey] [nvarchar](256) NULL,
	[SisUserName] [nvarchar](256) NULL,
	SisUserId int,
	[DistrictRef] [uniqueidentifier] NULL
)  
GO