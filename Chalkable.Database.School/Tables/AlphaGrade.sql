CREATE TABLE [dbo].[AlphaGrade] (
    [Id]          INT            NOT NULL,
    [SchoolRef]   INT            NOT NULL,
    [Name]        NVARCHAR (5)   NOT NULL,
    [Description] NVARCHAR (255) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AlphaGrade_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id])
);

GO

CREATE NONCLUSTERED INDEX IX_AlphaGrade_SchoolRef
	ON dbo.AlphaGrade( SchoolRef )
GO
