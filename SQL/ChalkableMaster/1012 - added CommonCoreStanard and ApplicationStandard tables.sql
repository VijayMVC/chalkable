create table CommonCoreStandard
(
	[Code] nvarchar(255) not null primary key,
	[Description] nvarchar(max)
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
