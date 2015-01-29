CREATE TABLE [dbo].[Infraction](
	[Id] [int] NOT NULL primary key,
	[Code] [varchar](5) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[Demerits] [tinyint] NOT NULL,
	[StateCode] [varchar](10) NOT NULL,
	[SIFCode] [varchar](10) NOT NULL,
	[NCESCode] [varchar](10) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSystem] [bit] NOT NULL
)

alter table [Infraction]
add constraint [UQ_Infraction_Code] unique([Code]) 
go
  
alter table [Infraction]
add constraint [UQ_Infraction_Name] unique([Name])
go