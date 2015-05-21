alter table Announcement
add AdminRef int null constraint FK_Announcement_Person foreign key references Person(Id)
go


alter table Announcement
alter column ClassRef int null
go

alter table Announcement
alter column SchoolRef int null
go 

alter table Announcement
alter column ClassAnnouncementTypeRef int null 
go 
alter table Announcement
alter column GradingStyle int null
go 