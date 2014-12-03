Alter Table Person 
	Drop Column LastPasswordReset
GO
Alter Table Person 
	Drop Column HasMedicalAlert
GO
Alter Table Person 
	Drop Column IsAllowedInetAccess
GO
Alter Table Person 
	Drop Column SpecialInstructions
GO
Alter Table Person 
	Drop Column SpEdStatus
GO


drop type TPerson
go

CREATE TYPE [dbo].[TPerson] AS TABLE(
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[Salutation] [nvarchar](255) NULL,
	[Active] [bit] NOT NULL,
	[FirstLoginDate] [datetime2](7) NULL,
	[LastMailNotification] [datetime2](7) NULL,
	[AddressRef] [int] NULL,
	[PhotoModifiedDate] [datetime2](7) NULL,
	[UserId] int null
)
GO
