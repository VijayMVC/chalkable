
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