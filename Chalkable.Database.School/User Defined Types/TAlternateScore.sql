CREATE TYPE [dbo].[TAlternateScore] AS TABLE (
    [Id]                    INT            NOT NULL,
    [Name]                  NVARCHAR (3)   NOT NULL,
    [Description]           NVARCHAR (255) NOT NULL,
    [IncludeInAverage]      BIT            NOT NULL,
    [PercentOfMaximumScore] DECIMAL (6, 2) NULL);

