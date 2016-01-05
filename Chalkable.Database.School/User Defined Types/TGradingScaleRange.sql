CREATE TYPE [dbo].[TGradingScaleRange] AS TABLE (
    [GradingScaleRef]     INT            NOT NULL,
    [AlphaGradeRef]       INT            NOT NULL,
    [LowValue]            DECIMAL (9, 6) NOT NULL,
    [HighValue]           DECIMAL (9, 6) NOT NULL,
    [AveragingEquivalent] INT            NOT NULL,
    [AwardGradCredit]     BIT            NOT NULL,
    [IsPassing]           BIT            NOT NULL);

