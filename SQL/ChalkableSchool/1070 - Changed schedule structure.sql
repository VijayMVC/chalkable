Create Table BellSchedule
(
		Id int not null Primary Key,
        SchoolYearRef int not null constraint FK_BellSchedule_SchoolYear Foreign Key References SchoolYear(Id),
        Name nvarchar(255),
        Description nvarchar(1024),
        TotalMinutes int not null,
        Code nvarchar(255),
        IsActive bit not null,
        IsSystem bit not null,
        UseStartEndTime bit not null
)
GO
Alter Table Period
	Drop FK_Period_School
GO
Alter Table Period
	Drop Column SchoolRef
GO
Alter Table Period
	Drop Column StartTime
GO
Alter Table Period
	Drop Column EndTime
GO
Alter Table ClassPeriod
	Drop FK_ClassPeriod_School
GO
Alter Table ClassPeriod
	Drop Column SchoolRef
GO
EXEC sp_rename 'ScheduledTimeSlot.BellScheduleID', 'BellScheduleRef';
GO
EXEC sp_rename 'ScheduledTimeSlot.TimeSlotID', 'PeriodRef';
GO
Delete From ScheduledTimeSlot
GO
Alter Table ScheduledTimeSlot
	Add Constraint FK_ScheduledTimeSlot_Period Foreign Key (PeriodRef) References Period(Id)
GO
Alter Table ScheduledTimeSlot
	Add Constraint FK_ScheduledTimeSlot_BellSchedule Foreign Key (BellScheduleRef) References BellSchedule(Id)
GO
Delete From SyncVersion where TableName = 'ScheduledTimeSlot'
GO
Delete from [Date]
GO
Alter Table [Date]
	Add BellScheduleRef int Constraint FK_Date_BellSchedule Foreign Key References BellSchedule(Id)
GO
Alter Table [Date]
	Drop FK_Date_School
GO
Alter Table [Date]
	Drop Column SchoolRef
GO
Delete From SyncVersion where TableName = 'CalendarDay'
GO

Create Type [dbo].[TBellSchedule] as Table
(
		Id int not null,
        SchoolYearRef int not null,
        Name nvarchar(255),
        Description nvarchar(1024),
        TotalMinutes int not null,
        Code nvarchar(255),
        IsActive bit not null,
        IsSystem bit not null,
        UseStartEndTime bit not null
)
GO
Drop Type [dbo].[TDate]
GO
Create Type [dbo].[TDate] AS TABLE(
	[Day] [datetime2](7) NOT NULL,
	[DayTypeRef] [int] NULL,
	[SchoolYearRef] [int] NOT NULL,
	[IsSchoolDay] [bit] NOT NULL,
	BellScheduleRef int
)
GO

Drop Type [dbo].[TScheduledTimeSlot]
GO
Create Type [dbo].[TScheduledTimeSlot] AS TABLE(
	[BellScheduleRef] [int] NOT NULL,
	[PeriodRef] [int] NOT NULL,
	[StartTime] [int] NULL,
	[EndTime] [int] NULL,
	[Description] [varchar](255) NOT NULL,
	[IsDailyAttendancePeriod] [bit] NOT NULL
)
GO

Drop Type [dbo].[TPeriod]
GO
Create Type [dbo].[TPeriod] AS TABLE(
	[Id] [int] NOT NULL,
	[SchoolYearRef] [int] NOT NULL,
	[Order] [int] NOT NULL
)
GO



