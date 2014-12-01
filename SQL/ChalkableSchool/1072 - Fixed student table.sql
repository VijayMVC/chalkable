Alter Table [Student] 
	Alter Column [SpecialInstructions] varchar(4096)not null
GO
DROP TYPE [dbo].[TStudent]
GO
CREATE TYPE [dbo].[TStudent] AS TABLE(
	[Id] [int] NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[BirthDate] [datetime2](7) NULL,
	[Gender] [nvarchar](255) NULL,
	[HasMedicalAlert] [bit] NOT NULL,
	[IsAllowedInetAccess] [bit] NOT NULL,
	[SpecialInstructions] varchar(4096)not null,
	[SpEdStatus] [nvarchar](256) NULL,
	[PhotoModifiedDate] [datetime2](7) NULL,
	[UserId] [int] NOT NULL
)
GO


