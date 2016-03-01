create table AutoGrade
(
	AnnouncementApplicationRef int not null constraint FK_AutoGrade_AnnouncementApplication foreign key references AnnouncementApplication(Id),
	StudentRef int not null constraint FK_AutoGrade_Student foreign key references Student(Id),
	Grade nvarchar(255),
	[Date] datetime2,
	Posted bit
)
go

alter table AutoGrade
add constraint PK_AutoGrade primary key (StudentRef, AnnouncementApplicationRef)
go
