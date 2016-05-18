CREATE TABLE [dbo].[ClassPeriod] (
    [PeriodRef]  INT NOT NULL,
    [DayTypeRef] INT NOT NULL,
    [ClassRef]   INT NOT NULL,
    CONSTRAINT [PK_ClassPeriod] PRIMARY KEY CLUSTERED ([ClassRef] ASC, [PeriodRef] ASC, [DayTypeRef] ASC),
    CONSTRAINT [FK_ClassPeriod_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_ClassPeriod_DayType] FOREIGN KEY ([DayTypeRef]) REFERENCES [dbo].[DayType] ([Id]),
    CONSTRAINT [FK_ClassPeriod_Period] FOREIGN KEY ([PeriodRef]) REFERENCES [dbo].[Period] ([Id]),
    CONSTRAINT [UQ_Class_Period_DayType] UNIQUE NONCLUSTERED ([ClassRef] ASC, [PeriodRef] ASC, [DayTypeRef] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ClassPeriod_Class]
    ON [dbo].[ClassPeriod]([ClassRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ClassPeriod_Period]
    ON [dbo].[ClassPeriod]([PeriodRef] ASC);

GO	
CREATE NONCLUSTERED INDEX IX_ClassPeriod_DayTypeRef
	ON dbo.ClassPeriod( DayTypeRef )
GO