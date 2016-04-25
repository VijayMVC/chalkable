CREATE TABLE [dbo].[GradedItem] (
    [Id]                      INT            NOT NULL,
    [GradingPeriodRef]        INT            NOT NULL,
    [Name]                    NVARCHAR (20)  NOT NULL,
    [Description]             NVARCHAR (255) NOT NULL,
    [AlphaOnly]               BIT            NOT NULL,
    [AppearsOnReportCard]     BIT            NOT NULL,
    [DetGradePoints]          BIT            NOT NULL,
    [DetGradeCredit]          BIT            NOT NULL,
    [PostToTranscript]        BIT            NOT NULL,
    [AllowExemption]          BIT            NOT NULL,
    [DisplayAsAvgInGradebook] BIT            NOT NULL,
    [PostRoundedAverage]      BIT            NOT NULL,
    [Sequence]                INT            NOT NULL,
    [AveragingRule]           NCHAR (1)      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([GradingPeriodRef]) REFERENCES [dbo].[GradingPeriod] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_GradedItem_GradingPeriodRef
	ON dbo.GradedItem( GradingPeriodRef )
GO