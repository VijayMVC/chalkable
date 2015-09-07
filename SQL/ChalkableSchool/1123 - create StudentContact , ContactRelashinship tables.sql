create table [dbo].[ContactRelationship](
	[Id] int NOT NULL primary key,
	[Code] nvarchar(5) NOT NULL,
	[Name] nvarchar(50) NOT NULL,
	[Description] nvarchar(255) NOT NULL,
	[ReceivesMailings] [bit] NOT NULL,
	[CanPickUp] [bit] NOT NULL,
	[IsFamilyMember] [bit] NOT NULL,
	[IsCustodian] [bit] NOT NULL,
	[IsEmergencyContact] [bit] NOT NULL,
	[StateCode] nvarchar(10) NOT NULL,
	[SIFCode] nvarchar(10) NOT NULL,
	[NCESCode] nvarchar(10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL,
)
go

alter table [ContactRelationship]
add constraint QU_ContactRelationship_Code unique([Code])
go

alter table [ContactRelationship]
add constraint QU_ContactRelationship_Name unique(Name)
go

CREATE TABLE [StudentContact]
(
	StudentRef [int] NOT NULL constraint FK_StudentContact_Student foreign key references Student(Id),
	ContactRef [int] NOT NULL constraint FK_StudentContact_Person foreign key references Person(Id),
	[ContactRelationshipRef] int NOT NULL constraint FK_StudentContact_ContactRelationship foreign key references ContactRelationship(Id),
	[Description] [nvarchar](255) NOT NULL,
	[ReceivesMailings] [bit] NOT NULL,
	[CanPickUp] [bit] NOT NULL,
	[IsFamilyMember] [bit] NOT NULL,
	[IsCustodian] [bit] NOT NULL,
	[IsEmergencyContact] [bit] NOT NULL,
	[IsResponsibleForBill] [bit] NOT NULL,
	[ReceivesBill] [bit] NOT NULL,
	[StudentVisibleInHome] [bit] NOT NULL
)
GO

alter table [StudentContact]
add CONSTRAINT PK_StudentContact PRIMARY KEY (StudentRef, ContactRef)
go


