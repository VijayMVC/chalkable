Create Table District
(
	Id uniqueidentifier not null primary key,
	Name nvarchar(256) not null,
	[SisUrl] [nvarchar](1024) NULL,
	[DbName] [nvarchar](1024) NULL,
	[SisUserName] [nvarchar](1024) NULL,
	[SisPassword] [nvarchar](1024) NULL,
	[Status] int not null,
	TimeZone nvarchar(1024) not null,
	DemoPrefix nvarchar(256),
	LastUseDemo DateTime2,
	ServerUrl nvarchar(256) not null,
	SisDistrictId uniqueidentifier
)
GO

Create Table [User]
(
	Id uniqueidentifier not null primary key,
	LocalId int,
	[Login] nvarchar(256) not null,
	[Password] nvarchar(256) not null,
	IsSysAdmin bit not null,
	IsDeveloper bit not null,
	ConfirmationKey nvarchar(256),
	SisUserName nvarchar(256),
	DistrictRef uniqueidentifier Constraint FK_User_District Foreign Key References District(Id),
	[SisToken] [nvarchar](max) NULL,
	[SisTokenExpires] [datetime2](7) NULL
)
GO

Alter Table [User]
	Add Constraint UQ_Login unique ([Login])	
GO

Create Index IX_USER_LOGIN_PASSWORD
	on [User](Login, Password)
GO

Create Table School
(
	Id uniqueidentifier not null primary key,
	DistrictRef uniqueidentifier not null Constraint FK_School_District Foreign Key References District(Id),
	Name nvarchar(256) not null,
	LocalId int not null
)
GO

Create Table SchoolUser
(
	Id uniqueidentifier not null primary key,
	SchoolRef uniqueidentifier not null Constraint FK_SchoolUser_School Foreign Key References School(Id),
	UserRef uniqueidentifier not null Constraint FK_SchoolUser_User Foreign Key References [User](Id),
	[Role] int not null
)
GO

Create Table BackgroundTask
(
	Id uniqueidentifier not null primary key,
	DistrictRef uniqueidentifier constraint FK_BackgroundTask_District foreign key references District(Id),
	[Type] int not null,
	[State] int not null,
	Created DateTime2 not null,
	Scheduled DateTime2 not null,
	[Started] DateTime2,
	Data nvarchar(max),
	Completed DateTime2
)
GO

Create Procedure spGetBackgroundTaskForProcessing
	@currentTime DateTime2
as
declare @id uniqueidentifier = null;
set @id = 
(select top 1 Id from 
BackgroundTask bt
where 
	Scheduled <= @currentTime
	and [State] = 0
	and not exists (select * from BackgroundTask where (DistrictRef = bt.DistrictRef or (DistrictRef is null and bt.DistrictRef is null))and [State] = 1 )
order by
	Scheduled
);
update BackgroundTask set [State] = 1, [Started] = @currentTime where Id = @id;
select * from BackgroundTask where Id = @id
GO

CREATE TABLE [dbo].[Preference]
(
	[Id] uniqueidentifier primary key NOT NULL,
	[Key] [nvarchar](256) NOT NULL,
	[Value] [nvarchar](2046) NULL,
	[IsPublic] [bit] NOT NULL,
	[Category] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[Hint] [nvarchar](max) NULL
)
GO

CREATE TABLE ChalkableDepartment
(
	[Id] uniqueidentifier NOT NULL primary key,
	[Name] [nvarchar](255) NOT NULL,
	[Keywords] [nvarchar](max) NOT NULL
)
GO

CREATE TABLE Developer
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[Name] [nvarchar](255) NULL,
	[WebSite] [nvarchar](255) NULL,
	DistrictRef uniqueidentifier not null Constraint FK_Developer_District Foreign Key References District(Id)
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
	HasParentMyApps bit not null
) 

GO

CREATE TABLE [ApplicationRating]
(
	[Id] uniqueidentifier NOT NULL Primary Key,
	[ApplicationRef] uniqueidentifier NOT NULL Constraint FK_ApplicationRating_Application Foreign Key References [Application](Id),
	[UserRef] uniqueidentifier NOT NULL Constraint FK_ApplicationRating_User Foreign Key References [User](Id),
	[Rating] [int] NOT NULL,
	[Review] [nvarchar](max) NULL,
)

GO

Create Table ApplicationPicture
(
	Id uniqueidentifier not null,
	ApplicationRef uniqueidentifier not null constraint FK_ApplicationPicture_Application foreign key references [Application](Id)
)
GO

Alter Table ApplicationPicture 
	Add Constraint PK_ApplicationPicture_Id_ApplicationRef primary key (Id, ApplicationRef)
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

Create Table ApplicationGradeLevel
(
	Id uniqueidentifier not null primary key,
	ApplicationRef uniqueidentifier not null Constraint FK_ApplicationGradeLevel_Application Foreign Key references Application(Id),
	GradeLevel int not null
)
GO

Alter Table Developer
	Add Constraint UQ_Developer_DistrctRef unique (DistrictRef)
GO

Alter Table ApplicationGradeLevel
	Add Constraint UQ_ApplicationGradeLevel_ApplicationRef_GradeLevel unique(ApplicationRef, GradeLevel)
GO

Alter Table ApplicationPermission
	Add Constraint UQ_ApplicationPermission_ApplicationRef_Permission unique(ApplicationRef, Permission)
GO

CREATE TABLE FundRequest
(
	[Id] uniqueidentifier NOT NULL primary key,
	[UserRef] uniqueidentifier Constraint FK_FUND_REQUEST_USER Foreign Key References [User](Id),
	[CreatedByRef] uniqueidentifier NOT NULL Constraint FK_FUND_REQUEST_CREATED_BY Foreign Key References [User](Id),
	[Amount] [money] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[PurchaseOrder] [nvarchar](255),
	[State] [int],
	[SignatureRef] uniqueidentifier,
	SchoolRef uniqueidentifier not null Constraint FK_FundRequest_School Foreign Key References School(Id)
)

GO

CREATE TABLE Fund
(
	[Id] uniqueidentifier NOT NULL  primary key,
	[PerformedDateTime] [datetime2] NOT NULL,
	[Amount] [money] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[FromSchoolRef] uniqueidentifier Constraint FK_Fund_FromSchool Foreign Key References School(Id),
	[ToSchoolRef] uniqueidentifier Constraint FK_Fund_ToSchool Foreign Key References School(Id),
	[FromUserRef] uniqueidentifier Constraint FK_Fund_FromUser Foreign Key References [User](Id),
	[ToUserRef] uniqueidentifier Constraint FK_Fund_ToUser Foreign Key References [User](Id),
	[AppInstallActionRef] uniqueidentifier NULL,
	[IsPrivate] [bit] NOT NULL,
	[FundRequestRef] uniqueidentifier null Constraint FK_Fund_FundRequest Foreign Key References FundRequest(Id),
	[SchoolRef] uniqueidentifier not null Constraint FK_Fund_School Foreign Key References School(Id),
)

GO

CREATE TABLE FundRequestRoleDistribution
(
	[Id] uniqueidentifier NOT NULL primary key,
	[RoleRef] [int] NOT NULL,
	[FundRequestRef] uniqueidentifier NOT NULL Constraint FK_FUND_ROLE_DEST_RUND_REQUETS Foreign Key References FundRequest(Id),
	[Amount] [money] NOT NULL
)
GO

Create Procedure spDeleteDistrict @id uniqueidentifier
as
	BEGIN TRY
		BEGIN TRANsACTION
			delete from BackgroundTask where DistrictRef = @id
			delete from Fund where SchoolRef in (select id from School where DistrictRef = @id)
			delete from SchoolUser where SchoolRef in (select id from School where DistrictRef = @id)			
			delete from School where DistrictRef = @id
			delete from FundRequestRoleDistribution where FundRequestRef in (select id from FundRequest where SchoolRef in (select id from School where DistrictRef = @id))
			delete from FundRequest where SchoolRef in (select id from School where DistrictRef = @id)			
			delete from District where Id = @id					
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		if @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRANSACTION
		END
		declare @errorMessage nvarchar(max) = ERROR_MESSAGE()
		declare @errorSeverity int = ERROR_SEVERITY()
		declare @errorState int = ERROR_STATE()	
		RAISERROR(@errorMessage, @errorSeverity, @errorState)
	END CATCH

GO

alter table ApplicationRating
add constraint QU_ApplicationRating_Application_User unique (ApplicationRef, UserRef)
go