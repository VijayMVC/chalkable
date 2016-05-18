CREATE TABLE [dbo].[ScheduledTimeSlot] (
    [BellScheduleRef]         INT            NOT NULL,
    [PeriodRef]               INT            NOT NULL,
    [StartTime]               INT            NULL,
    [EndTime]                 INT            NULL,
    [Description]             NVARCHAR (255) NOT NULL,
    [IsDailyAttendancePeriod] BIT            NOT NULL,
    CONSTRAINT [CPK_ScheduledTimeSlot] PRIMARY KEY CLUSTERED ([BellScheduleRef] ASC, [PeriodRef] ASC),
    CONSTRAINT [FK_ScheduledTimeSlot_BellSchedule] FOREIGN KEY ([BellScheduleRef]) REFERENCES [dbo].[BellSchedule] ([Id]),
    CONSTRAINT [FK_ScheduledTimeSlot_Period] FOREIGN KEY ([PeriodRef]) REFERENCES [dbo].[Period] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_ScheduledTimeSlot_PeriodRef
	ON dbo.ScheduledTimeSlot( PeriodRef )
GO