CREATE TYPE [dbo].[TAttendanceLevelReason] AS TABLE (
    [Id]                  INT            NOT NULL,
    [Level]               NVARCHAR (255) NOT NULL,
    [AttendanceReasonRef] INT            NOT NULL,
    [IsDefault]           BIT            NOT NULL);

