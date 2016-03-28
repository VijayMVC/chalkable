--CREATE TYPE MyTableType AS TABLE 


CREATE TYPE [dbo].[TApplication] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDateTime] [datetime2](7) NOT NULL,
	[Url] [nvarchar](2048) NULL,
	[SecretKey] [nvarchar](2048) NULL,
	[ShortDescription] [nvarchar](max) NULL,
	[AdditionalInfo] [nvarchar](max) NULL,
	[Price] [money] NOT NULL,
	[VideoUrl] [nvarchar](255) NULL,
	[SmallPictureRef] [uniqueidentifier] NULL,
	[BigPictureRef] [uniqueidentifier] NULL,
	[DeveloperRef] [uniqueidentifier] NOT NULL,
	[State] [int] NOT NULL,
	[CanAttach] [bit] NOT NULL,
	[ShowInGradeView] [bit] NOT NULL,
	[HasStudentMyApps] [bit] NOT NULL,
	[HasTeacherMyApps] [bit] NOT NULL,
	[HasAdminMyApps] [bit] NOT NULL,
	[PricePerClass] [money] NULL,
	[PricePerSchool] [money] NULL,
	[OriginalRef] [uniqueidentifier] NULL,
	[IsInternal] [bit] NOT NULL,
	[HasParentMyApps] [bit] NOT NULL
) 

CREATE TYPE [dbo].[TApplicationCategory] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[CategoryRef] [uniqueidentifier] NOT NULL
) 

CREATE TYPE [dbo].[TApplicationGradeLevel] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[GradeLevel] [int] NOT NULL
) 

CREATE TYPE [dbo].[TApplicationPermission] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[Permission] [int] NOT NULL
) 

CREATE TYPE [dbo].[TApplicationPicture] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL
) 

CREATE TYPE [dbo].[TApplicationRating] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[UserRef] [uniqueidentifier] NOT NULL,
	[Rating] [int] NOT NULL,
	[Review] [nvarchar](max) NULL
)  

CREATE TYPE [dbo].[TBackgroundTask] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[DistrictRef] [uniqueidentifier] NULL,
	[Type] [int] NOT NULL,
	[State] [int] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Scheduled] [datetime2](7) NOT NULL,
	[Started] [datetime2](7) NULL,
	[Data] [nvarchar](max) NULL,
	[Completed] [datetime2](7) NULL
)  

CREATE TYPE [dbo].[TCategory] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL
) 

CREATE TYPE [dbo].[TChalkableDepartment] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Keywords] [nvarchar](max) NOT NULL
)  

CREATE TYPE [dbo].[TDeveloper] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[WebSite] [nvarchar](255) NULL,
	[DistrictRef] [uniqueidentifier] NOT NULL
) 

CREATE TYPE [dbo].[TDistrict] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[SisUrl] [nvarchar](1024) NULL,
	[DbName] [nvarchar](1024) NULL,
	[SisUserName] [nvarchar](1024) NULL,
	[SisPassword] [nvarchar](1024) NULL,
	[Status] [int] NOT NULL,
	[TimeZone] [nvarchar](1024) NOT NULL,
	[ServerUrl] [nvarchar](256) NOT NULL,
	[SisDistrictId] [uniqueidentifier] NULL,
	[SisRedirectUrl] [nvarchar](1024) NULL,
	[LastSync] [datetime2](7) NULL
) 

CREATE TYPE [dbo].[TFund] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[PerformedDateTime] [datetime2](7) NOT NULL,
	[Amount] [money] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[FromSchoolRef] [uniqueidentifier] NULL,
	[ToSchoolRef] [uniqueidentifier] NULL,
	[FromUserRef] [uniqueidentifier] NULL,
	[ToUserRef] [uniqueidentifier] NULL,
	[AppInstallActionRef] [uniqueidentifier] NULL,
	[IsPrivate] [bit] NOT NULL,
	[FundRequestRef] [uniqueidentifier] NULL,
	[SchoolRef] [uniqueidentifier] NOT NULL
)  

CREATE TYPE [dbo].[TFundRequest] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[UserRef] [uniqueidentifier] NULL,
	[CreatedByRef] [uniqueidentifier] NOT NULL,
	[Amount] [money] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[PurchaseOrder] [nvarchar](255) NULL,
	[State] [int] NULL,
	[SignatureRef] [uniqueidentifier] NULL,
	[SchoolRef] [uniqueidentifier] NOT NULL
) 

CREATE TYPE [dbo].[TFundRequestRoleDistribution] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[RoleRef] [int] NOT NULL,
	[FundRequestRef] [uniqueidentifier] NOT NULL,
	[Amount] [money] NOT NULL
) 

CREATE TYPE [dbo].[TPreference] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[Key] [nvarchar](256) NOT NULL,
	[Value] [nvarchar](2046) NULL,
	[IsPublic] [bit] NOT NULL,
	[Category] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Hint] [nvarchar](max) NULL
)  

CREATE TYPE [dbo].[TSchool] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[DistrictRef] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
	[LocalId] [int] NOT NULL
) 

CREATE TYPE [dbo].[TSchoolUser] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[SchoolRef] [uniqueidentifier] NOT NULL,
	[UserRef] [uniqueidentifier] NOT NULL,
	[Role] [int] NOT NULL
) 

CREATE TYPE [dbo].[TUser] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[LocalId] [int] NULL,
	[Login] [nvarchar](256) NOT NULL,
	[Password] [nvarchar](256) NOT NULL,
	[IsSysAdmin] [bit] NOT NULL,
	[IsDeveloper] [bit] NOT NULL,
	[ConfirmationKey] [nvarchar](256) NULL,
	[SisUserName] [nvarchar](256) NULL,
	[DistrictRef] [uniqueidentifier] NULL,
	[SisToken] [nvarchar](max) NULL,
	[SisTokenExpires] [datetime2](7) NULL
)  

