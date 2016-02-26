
Alter Table ApplicationInstall
Add	AppUninstallActionRef int null constraint FK_ApplicationInstall_AppUninstallAction foreign key references ApplicationInstallAction(Id)
Go

Alter Table ApplicationInstallAction 
Add Installed bit  null 
Go 
 
Update ApplicationInstallAction
Set Installed = 1

Alter Table ApplicationInstallAction 
Alter Column Installed bit not null 
Go 

exec sp_rename 'ApplicationInstallAction.InstallDate' , 'Date' , 'Column'
Go


Alter Table ApplicationInstallAction 
drop constraint FK_ApplicationInstallAction_SchoolYear

Alter Table ApplicationInstallAction 
Drop column SchoolYearRef


Drop Table ApplicationInstallActionRole
Go
Drop Table ApplicationInstallActionDepartment
Go
Drop Table ApplicationInstallActionGradeLevel
Go

Drop Type TApplicationInstallActionRole
Go
Drop Type TApplicationInstallActionDepartment
Go
Drop Type TApplicationInstallActionGradeLevel
Go





Drop Type [TApplicationInstall]
Go

CREATE TYPE [dbo].[TApplicationInstall] AS TABLE(
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[InstallDate] [datetime2](7) NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[OwnerRef] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[AppInstallActionRef] [int] NOT NULL,
	[AppUninstallActionRef] [int] NULL
)
GO


Drop Type [TApplicationInstallAction]
Go

CREATE TYPE [dbo].[TApplicationInstallAction] AS TABLE(
	[Id] [int] NOT NULL,
	[OwnerRef] [int] NOT NULL,
	[PersonRef] [int] NULL,
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Date] datetime2 not null,
	Installed bit not null
)
GO






Alter Procedure [dbo].[spGetApplicationInstallHistory] 
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
GO
