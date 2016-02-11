create table AnnouncementAttribute
(
	Id int not null primary key,
	Code nvarchar(5) not null,
	Name nvarchar(50) not null,
	[Description] nvarchar(255) not null,
	StateCode nvarchar(10) not null,
	SIFCode nvarchar(10) not null,
	NCESCode nvarchar(10) not null,
	IsActive bit not null,
	IsSystem bit not null
)
Go

alter table AnnouncementAttribute
add constraint QA_AnnouncementAttribute_Code unique([Code])
go

alter table AnnouncementAttribute
add constraint QA_AnnouncementAttribute_Name unique([Name])
go
