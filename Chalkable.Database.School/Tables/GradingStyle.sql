CREATE TABLE [dbo].[GradingStyle] (
    [Id]                INT NOT NULL,
    [GradingStyleValue] INT NOT NULL,
    [MaxValue]          INT NOT NULL,
    [StyledValue]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

