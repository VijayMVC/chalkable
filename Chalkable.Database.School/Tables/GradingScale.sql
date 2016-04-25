CREATE TABLE [dbo].[GradingScale] (
    [Id]                 INT            NOT NULL,
    [SchoolRef]          INT            NOT NULL,
    [Name]               NVARCHAR (20)  NOT NULL,
    [Description]        NVARCHAR (255) NOT NULL,
    [HomeGradeToDisplay] INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_GradingScale_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [UQ_GradingScale_SchoolRef_Name] UNIQUE NONCLUSTERED ([SchoolRef] ASC, [Name] ASC)
);
