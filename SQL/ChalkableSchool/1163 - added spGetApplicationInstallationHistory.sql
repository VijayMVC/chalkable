Alter Table ApplicationInstallAction
	Add SchoolYearRef int Constraint FK_ApplicationInstallAction_SchoolYear Foreign Key References SchoolYear(Id)
GO

Alter Table 
	ApplicationInstallAction
Add InstallDate DateTime2

Alter Table 
	ApplicationInstallAction
Add OwnerRoleId int

Delete From ApplicationInstallAction 
Where 
	Id not in
	(select AppInstallActionRef from ApplicationInstall)
GO

Update 
	ApplicationInstallAction
set
	SchoolYearRef = (Select top 1 SchoolYearRef from ApplicationInstall where ApplicationInstall.AppInstallActionRef = ApplicationInstallAction.Id),
	InstallDate = (Select top 1 InstallDate from ApplicationInstall where ApplicationInstall.AppInstallActionRef = ApplicationInstallAction.Id)	
GO

Update 
	ApplicationInstallAction
Set
	OwnerRoleId = 2
Where OwnerRef in
	(Select Id From Staff)	
GO

Update 
	ApplicationInstallAction
Set
	OwnerRoleId = 3
Where OwnerRoleId is null
GO

Alter Table ApplicationInstallAction
	Alter Column SchoolYearRef int not null
GO

Alter Table 
	ApplicationInstallAction
Alter Column InstallDate DateTime2 not null
GO

Alter Table 
	ApplicationInstallAction
Alter Column OwnerRoleId int not null
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
		ApplicationInstallAction.OwnerRoleId as OwnerRoleId,
		ApplicationInstallAction.InstallDate as InstallDate,
		(Select count(*) From ApplicationInstall where ApplicationInstall.AppInstallActionRef = ApplicationInstallAction.Id) as InstalledCount
	From
		ApplicationInstallAction
		join Person on ApplicationInstallAction.OwnerRef = Person.Id
		join SchoolYear on SchoolYear.Id = ApplicationInstallAction.SchoolYearRef
		join School on SchoolYear.SchoolRef = School.Id
	Where 
		ApplicationInstallAction.ApplicationRef = @applicationid
