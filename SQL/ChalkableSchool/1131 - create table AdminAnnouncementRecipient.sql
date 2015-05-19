create table AdminAnnouncementRecipient
(
	Id int primary key not null identity(1,1),
	AnnouncementRef int not null constraint FK_AnnouncementRecipient_Announcement foreign key references Announcement(Id),
	PersonRef int null constraint FK_AnnouncementRecipient_Person foreign key references Person(Id),
	[Role] int null,
	GradeLevelRef int null constraint FK_AnnouncementRecipient_GradeLevel foreign key references GradeLevel(Id),
	SchoolRef int null constraint FK_AnnouncementRecipient_School foreign key references School(Id)
	ToAll bit not null
)
go
