CREATE TABLE [dbo].[PrivateMessage] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [FromPersonRef]   INT             NOT NULL,
    [Sent]            DATETIME2 (7)   NULL,
    [Subject]         NVARCHAR (1024) NOT NULL,
    [Body]            NVARCHAR (MAX)  NOT NULL,
    [DeletedBySender] BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PrivateMessage_FromPerson] FOREIGN KEY ([FromPersonRef]) REFERENCES [dbo].[Person] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_PrivateMessage_FromPersonRef
	ON dbo.PrivateMessage( FromPersonRef )
GO




