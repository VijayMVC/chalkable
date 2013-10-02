CREATE TABLE [SisSync](
	Id uniqueidentifier not null primary key,
	[LastAttendanceSync] [datetime2] NULL,
	[AttendanceSyncFrequency] [int] NULL,
	[LastDisciplineSync] [datetime2] NULL,
	[DisciplineSyncFrequency] [int] NULL,
	[LastPersonSync] [datetime2] NULL,
	[PersonSyncFrequency] [int] NULL,
	[LastScheduleSync] [datetime2] NULL,
	[ScheduleSyncFrequency] [int] NULL,
	[SisSchoolId] [int] NULL,
	[SisDatabaseUrl] [nvarchar](1024) NULL,
	[SisDatabaseName] [nvarchar](1024) NULL,
	[SisDatabaseUserName] [nvarchar](1024) NULL,
	[SisDatabasePassword] [nvarchar](1024) NULL
)
GO
Alter Table [SisSync]
	Add Constraint FK_SisSyncInfo_School foreign key (Id) references School(Id)
GO


