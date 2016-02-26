





CREATE Procedure [dbo].[spGetApplicationInstallHistory]
@applicationid uniqueidentifier
as
Select
School.Id as SchoolId,
School.Name as SchoolName,
Person.Id as PersonId,
Person.FirstName as FirstName,
Person.LastName as LastName,
x.Id as ApplicationInstallActionId,
x.OwnerRef as OwnerId,
x.OwnerRoleId as OwnerRoleId,
x.[Date] as [Date],
x.Installed as Installed,
x.PersonCount
From
(
Select
ApplicationInstallAction.Id,
ApplicationInstallAction.OwnerRef,
ApplicationInstallAction.OwnerRoleId,
ApplicationInstallAction.[Date],
ApplicationInstallAction.Installed,
ApplicationInstall.SchoolYearRef as SchoolYearRef,
Count(*) as PersonCount
From
ApplicationInstallAction
join .ApplicationInstall on ApplicationInstall.AppInstallActionRef = ApplicationInstallAction.Id  and Active = 1
Where
ApplicationInstallAction.ApplicationRef = @applicationid and Installed = 1
Group By
ApplicationInstallAction.Id,
ApplicationInstallAction.OwnerRef,
ApplicationInstallAction.OwnerRoleId,
ApplicationInstallAction.[Date],
ApplicationInstallAction.Installed,
ApplicationInstall.SchoolYearRef
Union
Select
ApplicationInstallAction.Id,
ApplicationInstallAction.OwnerRef,
ApplicationInstallAction.OwnerRoleId,
ApplicationInstallAction.[Date],
ApplicationInstallAction.Installed,
ApplicationInstall.SchoolYearRef as SchoolYearRef,
Count(*) as PersonCount
From
ApplicationInstallAction
join ApplicationInstall on ApplicationInstall.AppUninstallActionRef = ApplicationInstallAction.Id and Active = 0
Where
ApplicationInstallAction.ApplicationRef = @applicationid and Installed = 0
Group By
ApplicationInstallAction.Id,
ApplicationInstallAction.OwnerRef,
ApplicationInstallAction.OwnerRoleId,
ApplicationInstallAction.[Date],
ApplicationInstallAction.Installed,
ApplicationInstall.SchoolYearRef

) x
Join Person on Person.Id = x.OwnerRef
Join SchoolYear on SchoolYear.Id = x.SchoolYearRef
Join School on School.Id = SchoolYear.SchoolRef
Order By x.[Date] Desc