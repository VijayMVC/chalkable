
Create Table ApplicationBanHistory
(
	Id int not null primary key identity(1,1),
	ApplicationRef uniqueidentifier not null,
	PersonRef int not null constraint FK_ApplicationBanHistory_Person foreign key references Person(Id),
	Banned bit not null,
	[Date] datetime2 not null, 
)
Go


CREATE NONCLUSTERED INDEX IX_ApplicationBanHistory_ApplicationRef
    ON ApplicationBanHistory (ApplicationRef)
Go



Create Procedure spGetApplicationBanHistory @applicationId uniqueidentifier 
as
Select 
	ApplicationBanHistory.*,
	Person.FirstName as PersonFisrtName,
	Person.LastName as PersonLastName
From 
	ApplicationBanHistory
Join 
	Person on Person.Id = ApplicationBanHistory.PersonRef
Where ApplicationRef = @applicationId
Order By ApplicationBanHistory.[Date] Desc
Go

