CREATE TABLE Developer
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[Name] [nvarchar](255) NULL,
	[WebSite] [nvarchar](255) NULL
)

GO

Alter Table Developer
	Add Constraint FK_Developer_User Foreign Key (Id) References [User](Id)
	
GO

CREATE TABLE [Application]
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[CreateDateTime] [datetime2] NOT NULL,
	[Url] [nvarchar](2048) NULL,
	[SecretKey] [nvarchar](2048) NULL,
	[ShortDescription] [nvarchar](max) NULL,
	[AdditionalInfo] [nvarchar](max) NULL,
	[Price] [money] NOT NULL,
	[VideoUrl] [nvarchar](255) NULL,
	[SmallPictureRef] uniqueidentifier NULL,
	[BigPictureRef] uniqueidentifier NULL,
	[DeveloperRef] uniqueidentifier NOT NULL Constraint FK_Application_Developer Foreign Key References Developer(Id),
	[State] [int] NOT NULL,
	[CanAttach] [bit] NOT NULL,
	[ShowInGradeView] [bit] NOT NULL,
	[HasStudentMyApps] [bit] NOT NULL,
	[HasTeacherMyApps] [bit] NOT NULL,
	[HasAdminMyApps] [bit] NOT NULL,
	[PricePerClass] [money] NULL,
	[PricePerSchool] [money] NULL,
	[OriginalRef] uniqueidentifier NULL Constraint FK_Application_Original Foreign Key References [Application](Id),
	[IsInternal] [bit] NOT NULL,
) 

GO

CREATE TABLE [ApplicationRating]
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[ApplicationRef] uniqueidentifier NOT NULL Constraint FK_ApplicationRating_Application Foreign Key References [Application](Id),
	[UserRef] uniqueidentifier NOT NULL Constraint FK_ApplicationRating_User Foreign Key References [User](Id),
	[Rating] [int] NOT NULL,
	[Reveiw] [nvarchar](max) NULL,
)

GO

CREATE TABLE [ApplicationPicture]
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[ApplicationRef] uniqueidentifier NOT NULL Constraint FK_ApplicationPicture_Application Foreign Key References [Application](Id)
)

GO

CREATE TABLE [Category]
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](1024) NULL,
)

GO

CREATE TABLE [ApplicationCategory]
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[ApplicationRef] uniqueidentifier NOT NULL Constraint FK_ApplicationCategory_Application Foreign Key References [Application](Id),
	[CategoryRef] uniqueidentifier NOT NULL Constraint FK_ApplicationCategory_Category Foreign Key References [Category](Id),
)

GO

CREATE TABLE [ApplicationPermission]
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[ApplicationRef] uniqueidentifier NOT NULL Constraint FK_ApplicationPermission_Application Foreign Key References [Application](Id),
	[Permission] [int] NOT NULL,
)

GO




