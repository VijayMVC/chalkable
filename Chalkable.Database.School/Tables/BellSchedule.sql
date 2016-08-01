CREATE TABLE [dbo].[BellSchedule] (
    [Id]              INT             NOT NULL,
    [SchoolYearRef]   INT             NOT NULL,
    [Name]            NVARCHAR (255)  NULL,
    [Description]     NVARCHAR (1024) NULL,
    [TotalMinutes]    INT             NOT NULL,
    [Code]            NVARCHAR (255)  NULL,
    [IsActive]        BIT             NOT NULL,
    [IsSystem]        BIT             NOT NULL,
    [UseStartEndTime] BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BellSchedule_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_BellSchedule_SchoolYearRef
	ON dbo.BellSchedule( SchoolYearRef )
GO