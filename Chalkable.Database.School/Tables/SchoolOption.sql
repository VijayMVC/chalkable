CREATE TABLE [dbo].[SchoolOption] (
    [Id]                                      INT           NOT NULL,
    [AllowSectionAverageModification]         BIT           NOT NULL,
    [EarliestPaymentDate]                     DATETIME2 (7) NULL,
    [NextReceiptNumber]                       INT           NULL,
    [DefaultCombinationIndex]                 INT           NULL,
    [TimeZoneName]                            NVARCHAR (32) NULL,
    [BaseHoursOffset]                         INT           NULL,
    [BaseMinutesOffset]                       INT           NULL,
    [ObservesDst]                             BIT           NULL,
    [AllowScoreEntryForUnexcused]             BIT           NOT NULL,
    [DisciplineOverwritesAttendance]          BIT           NOT NULL,
    [AllowDualEnrollment]                     BIT           NOT NULL,
    [CompleteStudentScheduleDefinition]       NVARCHAR (1)  NOT NULL,
    [AveragingMethod]                         NVARCHAR (1)  NOT NULL,
    [CategoryAveraging]                       BIT           NOT NULL,
    [StandardsGradingScaleRef]                INT           NULL,
    [StandardsCalculationMethod]              NVARCHAR (1)  NOT NULL,
    [StandardsCalculationRule]                NVARCHAR (1)  NOT NULL,
    [StandardsCalculationWeightMaximumValues] BIT           NOT NULL,
    [LockCategories]                          BIT           NOT NULL,
    [IncludeReportCardCommentsInGradebook]    BIT           NOT NULL,
    [MergeRostersForAttendance]               BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SchoolOption_GradinScale] FOREIGN KEY ([StandardsGradingScaleRef]) REFERENCES [dbo].[GradingScale] ([Id]),
    CONSTRAINT [FK_SchoolOption_School] FOREIGN KEY ([Id]) REFERENCES [dbo].[School] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_SchoolOption_StandardsGradingScaleRef
	ON dbo.SchoolOption( StandardsGradingScaleRef )
GO