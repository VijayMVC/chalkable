Create Database ChalkableSchoolTemplate
GO

Use ChalkableSchoolTemplate
GO

CREATE TABLE SchoolPerson
(
	[Id] uniqueidentifier NOT NULL primary key,	
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[ConfirmationKey] [nvarchar](1024) NULL,
	[Salutation] [nvarchar](255) NULL,
	[Active] [bit] NOT NULL,
	[LastPasswordReset] [datetime2](7) NULL,
	[FirstLoginDate] [datetime2](7) NULL,
	[SisId] [int] NULL,
	[RoleRef] [int] NOT NULL,
	[LastMailNotification] [datetime] NULL
)

GO
