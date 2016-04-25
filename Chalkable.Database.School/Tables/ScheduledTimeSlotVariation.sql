CREATE TABLE [dbo].[ScheduledTimeSlotVariation] (
    [Id]              INT            NOT NULL,
    [BellScheduleRef] INT            NOT NULL,
    [PeriodRef]       INT            NOT NULL,
    [Name]            NVARCHAR (50)  NOT NULL,
    [Description]     NVARCHAR (255) NOT NULL,
    [StartTime]       INT            NOT NULL,
    [EndTime]         INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ScheduledTimeSlotVariation_BellSchedule] FOREIGN KEY ([BellScheduleRef]) REFERENCES [dbo].[BellSchedule] ([Id]),
    CONSTRAINT [FK_ScheduledTimeSlotVariation_Period] FOREIGN KEY ([PeriodRef]) REFERENCES [dbo].[Period] ([Id]),
    CONSTRAINT [UQ_BellSchedule_TimeSlot_Name] UNIQUE NONCLUSTERED ([BellScheduleRef] ASC, [PeriodRef] ASC, [Name] ASC)
);

GO	
CREATE NONCLUSTERED INDEX IX_ScheduledTimeSlotVariation_PeriodRef
	ON dbo.ScheduledTimeSlotVariation( PeriodRef )
GO