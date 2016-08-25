CREATE TABLE [dbo].[GradeLevel] (
    [Code]        NVARCHAR (5)  NOT NULL,
    [Description] NVARCHAR (32) NULL,
    [Low]         NVARCHAR (10) NULL,
    [High]        NVARCHAR (10) NULL,
    CONSTRAINT [PK_GradeLevel] PRIMARY KEY CLUSTERED ([Code] ASC)
);

