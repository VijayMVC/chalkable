CREATE TYPE [dbo].[TGradingStyle] AS TABLE (
    [Id]                INT NOT NULL,
    [GradingStyleValue] INT NOT NULL,
    [MaxValue]          INT NOT NULL,
    [StyledValue]       INT NOT NULL);

