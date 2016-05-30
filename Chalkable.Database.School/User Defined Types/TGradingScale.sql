CREATE TYPE [dbo].[TGradingScale] AS TABLE (
    [Id]                 INT            NOT NULL,
    [SchoolRef]          INT            NOT NULL,
    [Name]               NVARCHAR (20)  NOT NULL,
    [Description]        NVARCHAR (255) NOT NULL,
    [HomeGradeToDisplay] INT            NULL);

