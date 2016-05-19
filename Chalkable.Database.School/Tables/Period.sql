CREATE TABLE [dbo].[Period] (
    [Id]            INT           NOT NULL,
    [SchoolYearRef] INT           NOT NULL,
    [Order]         INT           NOT NULL,
    [Name]          NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Period_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO
CREATE NONCLUSTERED INDEX IX_Period_SchoolYearRef
	ON dbo.Period( SchoolYearRef )
GO
