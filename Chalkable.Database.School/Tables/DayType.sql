CREATE TABLE [dbo].[DayType] (
    [Id]            INT             NOT NULL,
    [Number]        INT             NOT NULL,
    [Name]          NVARCHAR (1024) NOT NULL,
    [SchoolYearRef] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DayType_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

