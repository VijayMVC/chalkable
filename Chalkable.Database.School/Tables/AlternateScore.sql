CREATE TABLE [dbo].[AlternateScore] (
    [Id]                    INT            NOT NULL,
    [Name]                  NVARCHAR (3)   NOT NULL,
    [Description]           NVARCHAR (255) NOT NULL,
    [IncludeInAverage]      BIT            NOT NULL,
    [PercentOfMaximumScore] DECIMAL (6, 2) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

