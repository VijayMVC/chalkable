create table LessonPlanCategory
(
	Id int not null primary key identity(1,1),
	Name nvarchar(255) not null,
)
Go

alter table LessonPlanCategory
add constraint UQ_Name unique (Name)
Go

