CREATE TYPE [dbo].[TReportDownload] AS TABLE (
    [Id]           INT             NOT NULL,
    [Format]       INT             NOT NULL,
    [PersonRef]    INT             NOT NULL,
    [ReportType]   INT             NOT NULL,
    [DownloadDate] DATETIME2 (7)   NOT NULL,
    [FriendlyName] NVARCHAR (1024) NOT NULL);

