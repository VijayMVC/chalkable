CREATE TABLE [dbo].[ClassroomOption] (
    [Id]                                      INT          NOT NULL,
    [DefaultActivitySortOrder]                NVARCHAR (1) NOT NULL,
    [GroupByCategory]                         BIT          NOT NULL,
    [AveragingMethod]                         NVARCHAR (1) NOT NULL,
    [CategoryAveraging]                       BIT          NOT NULL,
    [IncludeWithdrawnStudents]                BIT          NOT NULL,
    [DisplayStudentAverage]                   BIT          NOT NULL,
    [DisplayTotalPoints]                      BIT          NOT NULL,
    [RoundDisplayedAverages]                  BIT          NOT NULL,
    [DisplayAlphaGrade]                       BIT          NOT NULL,
    [DisplayStudentNames]                     BIT          NOT NULL,
    [DisplayMaximumScore]                     BIT          NOT NULL,
    [StandardsGradingScaleRef]                INT          NULL,
    [StandardsCalculationMethod]              NVARCHAR (1) NOT NULL,
    [StandardsCalculationRule]                NVARCHAR (1) NOT NULL,
    [StandardsCalculationWeightMaximumValues] BIT          NOT NULL,
    [DefaultStudentSortOrder]                 NVARCHAR (1) NOT NULL,
    [SeatingChartRows]                        INT          NOT NULL,
    [SeatingChartColumns]                     INT          NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassroomOption_Class] FOREIGN KEY ([Id]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_ClassroomOption_GradingScale] FOREIGN KEY ([StandardsGradingScaleRef]) REFERENCES [dbo].[GradingScale] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_ClassroomOption_StandardsGradingScaleRef
	ON dbo.ClassroomOption( StandardsGradingScaleRef )
GO