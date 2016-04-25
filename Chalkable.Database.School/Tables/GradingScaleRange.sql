CREATE TABLE [dbo].[GradingScaleRange] (
    [GradingScaleRef]     INT            NOT NULL,
    [AlphaGradeRef]       INT            NOT NULL,
    [LowValue]            DECIMAL (9, 6) NOT NULL,
    [HighValue]           DECIMAL (9, 6) NOT NULL,
    [AveragingEquivalent] INT            NOT NULL,
    [AwardGradCredit]     BIT            NOT NULL,
    [IsPassing]           BIT            NOT NULL,
    CONSTRAINT [PK_GradingScaleRange] PRIMARY KEY CLUSTERED ([GradingScaleRef] ASC, [AlphaGradeRef] ASC),
    CONSTRAINT [FK_GradingScaleRange_Alphagrade] FOREIGN KEY ([AlphaGradeRef]) REFERENCES [dbo].[AlphaGrade] ([Id]),
    CONSTRAINT [FK_GradingScaleRange_GradingScale] FOREIGN KEY ([GradingScaleRef]) REFERENCES [dbo].[GradingScale] ([Id])
);


GO	
CREATE NONCLUSTERED INDEX IX_GradingScaleRange_AlphaGradeRef
	ON dbo.GradingScaleRange( AlphaGradeRef )
GO
