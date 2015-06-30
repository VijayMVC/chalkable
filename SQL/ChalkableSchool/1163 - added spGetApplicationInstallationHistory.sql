Alter Table ApplicationInstallAction
	Add SchoolYearRef int Constraint FK_ApplicationInstallAction_SchoolYear Foreign Key References SchoolYear(Id)
GO
delete from ApplicationInstallAction 
where 
	Id not in
	(select AppInstallActionRef from ApplicationInstall)

Update 
	ApplicationInstallAction
set
	SchoolYearRef = (Select top 1 SchoolYearRef from ApplicationInstall where ApplicationInstall.AppInstallActionRef = ApplicationInstallAction.Id)
GO

Alter Table ApplicationInstallAction
	Alter Column SchoolYearRef int not null
GO

Create Procedure spGetApplicationInstallHistory 
	@applicationid uniqueidentifier
as
	Select
		School.Id as SchoolId,
		School.Name as SchoolName,
		Person.Id as PersonId,
		Person.FirstName as FirstName,
		Person.LastName as LastName,
		(Select count(*) From ApplicationInstall where ApplicationInstall.AppInstallActionRef = ApplicationInstallAction.Id) as InstalledCount
	From
		ApplicationInstallAction
		join Person on ApplicationInstallAction.OwnerRef = Person.Id
		join SchoolYear on SchoolYear.Id = ApplicationInstallAction.SchoolYearRef
		join School on SchoolYear.SchoolRef = School.Id
	Where 
		ApplicationInstallAction.ApplicationRef = @applicationid
