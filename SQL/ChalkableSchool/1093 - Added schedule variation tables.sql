CREATE TABLE ScheduledTimeSlotVariation
(
 Id [int] NOT NULL PRIMARY KEY,
 [BellScheduleRef] [int] NOT NULL Constraint FK_ScheduledTimeSlotVariation_BellSchedule Foreign Key References BellSchedule(Id),
 [PeriodRef] [int] NOT NULL Constraint FK_ScheduledTimeSlotVariation_Period Foreign Key References Period(Id),
 [Name] [varchar](50) NOT NULL,
 [Description] [varchar](255) NOT NULL,
 [StartTime] [int] NOT NULL,
 [EndTime] [int] NOT NULL,
 CONSTRAINT [UQ_BellSchedule_TimeSlot_Name] UNIQUE NONCLUSTERED 
	(
	 [BellScheduleRef] ASC,
	 [PeriodRef] ASC,
	 [Name] ASC
	)
)

GO

Create Type TScheduledTimeSlotVariation As Table
(
	Id [int] NOT NULL,
	[BellScheduleRef] [int] NOT NULL,
	[PeriodRef] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[StartTime] [int] NOT NULL,
	[EndTime] [int] NOT NULL
)

GO

CREATE TABLE SectionTimeSlotVariation
(
 [ClassRef] [int] NOT NULL Constraint FK_SectionTimeSlotVariation_Class Foreign Key References Class(Id),
 [ScheduledTimeSlotVariationRef] [int] NOT NULL Constraint FK_SectionTimeSlotVariation_ScheduledTimeSlotVariation Foreign Key References ScheduledTimeSlotVariation(Id),
	 CONSTRAINT [PK_SectionTimeSlotVariation] PRIMARY KEY CLUSTERED 
	(
	 [ClassRef] ASC,
	 [ScheduledTimeSlotVariationRef] ASC
	)
)

GO

Create Type TSectionTimeSlotVariation As Table
(
	[ClassRef] [int] NOT NULL,
	[ScheduledTimeSlotVariationRef] [int] NOT NULL
)

GO