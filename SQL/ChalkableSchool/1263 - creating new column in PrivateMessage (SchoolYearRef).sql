
alter table PrivateMessage add [SchoolYearRef] int null

Go

declare @toUpdate table
(
	[Id] int not null,
	[SchoolYearRef] int not null
)

insert into @toUpdate
select
	pm.Id,
	Max(schoolYear.id) as SchoolYearId
From
	PrivateMessage pm
	join PrivateMessageRecipient pmr
		on pm.Id = pmr.PrivateMessageRef
	join vwPerson as sender
		on sender.Id = pm.FromPersonRef
	join vwPerson as recipient
		on recipient.Id = pmr.RecipientRef
	join SchoolYear on schoolYear.SchoolRef = sender.SchoolRef
where
	sender.SchoolRef = recipient.SchoolRef
	and (pm.[Sent] between schoolyear.StartDate and schoolyear.EndDate)
	and schoolYear.ArchiveDate is null
	and pm.SchoolYearRef is null
group by pm.Id

update pm
Set pm.SchoolYearRef = x.SchoolYearRef
From 
	PrivateMessage pm join @toUpdate x
		on pm.id = x.Id
--------------------------------------Second_Step---------------------------------
Go

declare @toUpdate table
(
	[Id] int not null,
	[SchoolYearRef] int not null
)

insert into @toUpdate
Select pm.Id, Max(SchoolYear.Id) 
From 
	PrivateMessage pm join vwPerson
		on pm.FromPersonRef = vwPerson.Id
	join SchoolYear
		on vwPerson.SchoolRef = SchoolYear.SchoolRef
Where pm.SchoolYearRef is null
group by pm.Id

update pm
Set pm.SchoolYearRef = x.SchoolYearRef
From 
	PrivateMessage pm join @toUpdate x
		on pm.id = x.Id

Go


alter table PrivateMessage alter column SchoolYearRef int not null

alter table PrivateMessage add constraint FK_PrivateMessage_SchoolYear Foreign Key (SchoolYearRef)
references SchoolYear(Id)