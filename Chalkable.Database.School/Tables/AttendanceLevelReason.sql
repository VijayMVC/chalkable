CREATE TABLE [dbo].[AttendanceLevelReason] (
    [Id]                  INT            NOT NULL,
    [Level]               NVARCHAR (255) NOT NULL,
    [AttendanceReasonRef] INT            NOT NULL,
    [IsDefault]           BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AttendanceLevelReason_AttendanceReason] FOREIGN KEY ([AttendanceReasonRef]) REFERENCES [dbo].[AttendanceReason] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_AttendanceLevelReason_AttendanceReasonRef
	ON dbo.AttendanceLevelReason( AttendanceReasonRef )
GO
