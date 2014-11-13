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

