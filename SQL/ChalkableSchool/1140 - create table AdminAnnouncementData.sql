create table AdminAnnouncementData
(
	AnnouncementRef int not null constraint FK_AdminAnnouncementData_Announcement foreign key references Announcement(Id),
	PersonRef int not null constraint FK_AdminAnnouncementData_Person foreign key references Person(Id),
	Complete bit not null
)
Go


alter table AdminAnnouncementData
add constraint PK_AdminAnnouncementData primary key (AnnouncementRef, PersonRef)
Go



