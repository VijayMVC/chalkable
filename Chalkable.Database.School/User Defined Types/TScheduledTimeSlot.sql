CREATE TYPE [dbo].[TScheduledTimeSlot] AS TABLE (
    [BellScheduleRef]         INT            NOT NULL,
    [PeriodRef]               INT            NOT NULL,
    [StartTime]               INT            NULL,
    [EndTime]                 INT            NULL,
    [Description]             NVARCHAR (255) NOT NULL,
    [IsDailyAttendancePeriod] BIT            NOT NULL);

