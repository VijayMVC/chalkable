create table CourseType
(
	Id int  NOT NULL primary key,
	[Code] [nvarchar](5) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[StateCode] [nvarchar](10) NOT NULL,
	[SIFCode] [nvarchar](10) NOT NULL,
	[NCESCode] [nvarchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL,
)
Go


alter table Class
add CourseTypeRef int null constraint FK_Class_CourseType foreign key references CourseType(Id)
Go

declare @courseTypeId int = 20000000   
insert into CourseType
values (@courseTypeId, 'All', 'All', 'All course types', '', '', '', 1, 0)

update Class 
set CourseTypeRef = @courseTypeId
go

alter table Class
alter column CourseTypeRef int not null
GO
