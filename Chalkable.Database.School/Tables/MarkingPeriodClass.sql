CREATE TABLE [dbo].[MarkingPeriodClass] (
    [ClassRef]         INT NOT NULL,
    [MarkingPeriodRef] INT NOT NULL,
    CONSTRAINT [PK_MarkingPeriodClass] PRIMARY KEY CLUSTERED ([ClassRef] ASC, [MarkingPeriodRef] ASC),
    CONSTRAINT [FK_MarkingPeriodClass_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_MarkingPeriodClass_MarkingPeriod] FOREIGN KEY ([MarkingPeriodRef]) REFERENCES [dbo].[MarkingPeriod] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MarkingPeriodClass_Class]
    ON [dbo].[MarkingPeriodClass]([ClassRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MarkingPeriodClass_MarkingPeriod]
    ON [dbo].[MarkingPeriodClass]([MarkingPeriodRef] ASC);

