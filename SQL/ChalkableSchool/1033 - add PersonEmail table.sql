create table PersonEmail
(
	PersonRef int not null constraint FK_PersonEmail_Person foreign key references Person(Id),
	EmailAddress nvarchar(128) not null,
	[Description] nvarchar(max) not null,
	IsListed bit,
	IsPrimary bit
)
go

ALTER TABLE PersonEmail
ADD CONSTRAINT PK_PersonRef_EmailAddress PRIMARY KEY (PersonRef, EmailAddress)
go
