create table CC_StandardCategory
(
	Id  uniqueidentifier not null primary key,
	ParentCategoryRef uniqueidentifier null constraint FK_CC_StandardCategory_ParentCategory foreign key references CC_StandardCategory(Id),
	Name nvarchar(max)
)
go


create table CommonCoreStandard
(
	[Code] nvarchar(255) not null primary key,
	[Description] nvarchar(max),
	[StandardCategoryRef] uniqueidentifier not null constraint FK_CommonCoreStandard_StandardCategory foreign key references CC_StandardCategory(Id)
)
go

create table ApplicationStandard
(
	ApplicationRef uniqueidentifier not null constraint FK_ApplicationStandard_Application foreign key references [Application](Id),
	StandardCode nvarchar(255) not null constraint FK_ApplicationStandard_CommonCoreStandard foreign key references [CommonCoreStandard]([Code])
)
go

alter table ApplicationStandard
add constraint PK_ApplicationStandard primary key (ApplicationRef, StandardCode)
go


alter table [Application]
add InternalScore int null
go

alter table [Application]
add InternalDescription nvarchar(max) null
go
