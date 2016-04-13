CREATE TYPE [dbo].[TReportMailDelivery] AS TABLE (
    [Id]                       INT           NOT NULL,
    [ReportType]               INT           NOT NULL,
    [Format]                   INT           NOT NULL,
    [Frequency]                INT           NOT NULL,
    [PersonRef]                INT           NOT NULL,
    [SendHour]                 INT           NULL,
    [SendDay]                  INT           NULL,
    [LastSentMarkingPeriodRef] INT           NULL,
    [LastSentTime]             DATETIME2 (7) NULL);

