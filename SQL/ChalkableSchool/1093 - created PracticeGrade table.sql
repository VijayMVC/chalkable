create table PracticeGrade
(	
	Id int not null primary key identity,
	StandardId int not null constraint FK_PracticeGrade_Standard foreign key references [Standard](Id),
	StudentId int not null constraint FK_PracticeGrade_Student foreign key references Student(Id),
	ApplicationRef uniqueidentifier not null,
	Score nvarchar(256),
	[Date] datetime2
)
go

