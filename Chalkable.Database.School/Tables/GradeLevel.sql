CREATE TABLE [dbo].[GradeLevel] (
    [Id]          INT             NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [Number]      INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

