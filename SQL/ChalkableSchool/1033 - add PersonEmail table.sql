create table PersonEmail
(
	Id int not null constraint FK_PersonEmail_Person foreign key references Person(Id),
	EmailAddress nvarchar(128) not null,
	[Description] nvarchar(max) not null,
	IsListed bit,
	IsPrimary bit
)
go

ALTER TABLE PersonEmail
ADD CONSTRAINT PK_Id_EmailAddress PRIMARY KEY (Id, EmailAddress)
go
