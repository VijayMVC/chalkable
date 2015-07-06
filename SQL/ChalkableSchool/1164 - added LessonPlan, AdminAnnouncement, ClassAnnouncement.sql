Create Table AdminAnnouncement
(
	Id int not null primary key constraint FK_AdminAnnouncement_Announcement foreign key references Announcement(Id),
	AdminRef int not null constraint FK_AdminAnnouncement_Person foreign key references Person(Id),
	Expires datetime2 not null
)
Go

insert into AdminAnnouncement
select Id, AdminRef, Expires 
from Announcement
where AdminRef is not null
Go

Create Table ClassAnnouncement
(
	Id int not null primary key constraint FK_ClassAnnouncement_Announcement foreign key references Announcement(Id),
	Expires datetime2 not null,
	ClassRef int not null constraint FK_ClassAnnouncement_Class foreign key references Class(Id),
	ClassAnnouncementTypeRef int null,
	SchoolRef int not null constraint FK_ClassAnnouncement_School foreign key references School(Id),
	MayBeDropped bit not null,
	VisibleForStudent bit not null,
	[Order] int not null,
	[Dropped] bit not null,
	MaxScore decimal(18,0) null,
	WeightAddition decimal(9,6) null,
	WeightMultiplier decimal(9,6) null,
	SisActivityId int null
)
GO

insert into ClassAnnouncement
select Id, Expires, ClassRef, ClassAnnouncementTypeRef, 
	   SchoolRef, MayBeDropped, VisibleForStudent,
	   [Order], Dropped, MaxScore, WeightAddition, 
	   WeightMultiplier, SisActivityId
from Announcement
where ClassRef is not null and SchoolRef is not null
Go


Create Table LessonPlan
(
	Id int not null primary key constraint FK_LessonPlan_Announcement foreign key references Announcement(Id),
	StartDate datetime2 null, 
	EndDate datetime2 null, 
	ClassRef int not null constraint FK_LessonPlan_Class foreign key references Class(Id),
	GalleryCategoryRef int null constraint FK_LessonPlan_LPGalleryCategory foreign key references LPGalleryCategory(Id),
	SchoolRef int not null constraint FK_LessonPlan_School foreign key references School(Id),
	VisibleForStudent bit not null,
)
GO



Alter Table Announcement
Drop Column Expires
Go


Alter Table Announcement
Drop Constraint FK_Announcement_Class
Go

drop Index IX_Announcement_Class on Announcement 
Go

alter table Announcement
drop column ClassRef
go

alter table Announcement
drop column ClassAnnouncementTypeRef
go

alter table Announcement
drop column MayBeDropped
go

alter table Announcement
drop column VisibleForStudent
go

alter table Announcement
drop column [Order]
go

alter table Announcement
drop column [Dropped]
go

alter table Announcement
drop column MaxScore
go

alter table Announcement
drop column WeightAddition
go

alter table Announcement
drop column WeightMultiplier
go

drop index UQ_Announcement_SisActivityId on Announcement 
Go

Drop Index IX_Announcement_SisId on Announcement
Go

Alter Table Announcement
Drop Column SisActivityId
go

Alter Table Announcement
Drop Constraint FK_Announcement_School

Alter Table Announcement
Drop Column SchoolRef
go

Alter Table Announcement
Drop Constraint FK_Announcement_Person

Alter Table Announcement
Drop Column AdminRef
go

Alter Table Announcement
Drop Column GradingStyle
Go

Alter Table Announcement
Drop Column [Subject]
Go


Create Unique NonClustered Index [UQ_ClassAnnouncement_SisActivityId]
On ClassAnnouncement ([SisActivityId])
Where [SisActivityId] IS NOT NULL


create table AnnouncementGroup
(
	AnnouncementRef int not null constraint FK_AnnouncementGroup_AdminAnnouncement foreign key references AdminAnnouncement(Id),
	GroupRef int not null constraint FK_AnnouncementGroup_Group foreign key references [Group](Id)
)
Go

Alter Table AnnouncementGroup
Add Constraint PK_AnnouncementGroup primary key (AnnouncementRef, GroupRef)
Go

insert into AnnouncementGroup
select AnnouncementRef, GroupRef 
from AdminAnnouncementRecipient

drop table AdminAnnouncementRecipient
go

exec sp_rename 'AdminAnnouncementData', 'AnnouncementRecipientData'
Go




