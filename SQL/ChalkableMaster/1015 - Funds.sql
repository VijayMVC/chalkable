CREATE TABLE FundRequest
(
	[Id] uniqueidentifier NOT NULL primary key,
	[SchoolRef] uniqueidentifier Constraint FK_FUND_REQUEST_SCHOOL Foreign Key References School(Id),
	[UserRef] uniqueidentifier Constraint FK_FUND_REQUEST_USER Foreign Key References [User](Id),
	[CreatedByRef] uniqueidentifier NOT NULL Constraint FK_FUND_REQUEST_CREATED_BY Foreign Key References [User](Id),
	[Amount] [money] NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[PurchaseOrder] [nvarchar](255),
	[State] [int],
	[SignatureRef] uniqueidentifier 
)

GO

CREATE TABLE Fund
(
	[Id] uniqueidentifier NOT NULL  primary key,
	[PerformedDateTime] [datetime2] NOT NULL,
	[Amount] [money] NOT NULL,
	[Descrption] [nvarchar](max) NULL,
	[FromSchoolRef] uniqueidentifier Constraint FK_FUND_FROM_SCHOOL Foreign Key References School(Id),
	[ToSchoolRef] uniqueidentifier Constraint FK_FUND_TO_SCHOOL Foreign Key References School(Id),
	[FromUserRef] uniqueidentifier Constraint FK_FUND_FROM_USER Foreign Key References [User](Id),
	[ToUserRef] uniqueidentifier Constraint FK_FUND_TO_USER Foreign Key References [User](Id),
	[AppInstallActionRef] uniqueidentifier NULL,
	[IsPrivate] [bit] NOT NULL,
	[FundRequestRef] [int] NULL,
	[SchoolRef] uniqueidentifier not null Constraint FK_FUND_SCHOOL Foreign Key References School(Id),
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
