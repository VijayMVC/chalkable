drop table ApplicationStandard
go
drop table CommonCoreStandard
go
drop table CC_StandardCategory
go
create table CommonCoreStandardCategory
(
	Id uniqueidentifier not null primary key,
	Name nvarchar(255) not null
)
go
create table CommonCoreStandard
(
	Id uniqueidentifier not null primary key,
	ParentStandardRef uniqueidentifier null constraint FK_CommonCoreStandard_ParentStandard foreign key references CommonCoreStandard(Id),
	StandardCategoryRef uniqueidentifier not null constraint FK_CommonCoreStandard_StandardCategory foreign key references CommonCoreStandardCategory(Id),
	Code nvarchar(255) not null,
	[Description] nvarchar(max) null,
	 
)
go
create table ApplicationStandard
(
	ApplicationRef uniqueidentifier not null constraint FK_ApplicationStandard_Application foreign key references [Application](Id),
	StandardRef uniqueidentifier not null constraint FK_ApplicationStandard_CommonCoreStandard foreign key references [CommonCoreStandard](Id)
)
go

ALTER TABLE ApplicationStandard
ADD CONSTRAINT PK_ApplicationStandardID PRIMARY KEY (ApplicationRef,StandardRef)
go