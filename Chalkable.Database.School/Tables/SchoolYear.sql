CREATE TABLE [dbo].[SchoolYear] (
    [Id]          INT             NOT NULL,
    [SchoolRef]   INT             NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [StartDate]   DATETIME2 (7)   NULL,
    [EndDate]     DATETIME2 (7)   NULL,
    [ArchiveDate] DATETIME2 (7)   NULL,
    [AcadYear]    INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SchoolYear_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id])
);


GO	
CREATE NONCLUSTERED INDEX IX_SchoolYear_SchoolRef
	ON dbo.SchoolYear( SchoolRef )
GO

