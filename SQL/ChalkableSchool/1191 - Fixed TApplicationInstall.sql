Drop Type TApplicationInstall
GO
Create Type TApplicationInstall As Table(
	[ApplicationRef] [uniqueidentifier] NOT NULL,
	[PersonRef] [int] NOT NULL,
	[InstallDate] [datetime2](7) NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[OwnerRef] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[AppInstallActionRef] [int] NOT NULL
)
GO


