CREATE TABLE [dbo].[GradeLevel] (
    [Id]          INT             NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [Number]      INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [QU_GradeLevel_Name] UNIQUE NONCLUSTERED ([Name] ASC),
    CONSTRAINT [QU_GradeLevel_Number] UNIQUE NONCLUSTERED ([Number] ASC)
);

