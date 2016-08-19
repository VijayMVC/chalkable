CREATE TABLE [dbo].[DayType] (
    [Id]            INT             NOT NULL,
    [Number]        INT             NOT NULL,
    [Name]          NVARCHAR (1024) NOT NULL,
    [SchoolYearRef] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DayType_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);

GO

CREATE TRIGGER DayTypeDeleteTrigger
ON DayType
INSTEAD OF DELETE
AS
	Update [Date] Set DayTypeRef = null where DayTypeRef in (Select Id From Deleted)
    Delete From DayType where Id in (Select Id From Deleted)

GO
CREATE NONCLUSTERED INDEX IX_DayType_SchoolYearRef
	ON dbo.DayType( SchoolYearRef )
GO