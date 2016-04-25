CREATE TABLE [dbo].[Group] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    [OwnerRef] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Group_Person] FOREIGN KEY ([OwnerRef]) REFERENCES [dbo].[Person] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_Group_OwnerRef
	ON dbo.[Group]( OwnerRef )
GO