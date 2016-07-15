CREATE TYPE [dbo].[TScheduledTimeSlotVariation] AS TABLE (
    [Id]              INT            NOT NULL,
    [BellScheduleRef] INT            NOT NULL,
    [PeriodRef]       INT            NOT NULL,
    [Name]            NVARCHAR (50)  NOT NULL,
    [Description]     NVARCHAR (255) NOT NULL,
    [StartTime]       INT            NOT NULL,
    [EndTime]         INT            NOT NULL);

