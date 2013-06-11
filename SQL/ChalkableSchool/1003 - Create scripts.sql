Create Table ClassPerson
(
	Id uniqueidentifier not null primary key,
	ClassRef uniqueidentifier not null constraint FK_ClassPerson_Class foreign key references Class(Id),
	PersonRef uniqueidentifier not null constraint FK_ClassPerson_Person foreign key references Person(Id),
)
GO