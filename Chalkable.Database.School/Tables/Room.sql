CREATE TABLE [dbo].[Room] (
    [Id]          INT             NOT NULL,
    [RoomNumber]  NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [Size]        NVARCHAR (255)  NULL,
    [Capacity]    INT             NULL,
    [PhoneNumber] NVARCHAR (255)  NULL,
    [SchoolRef]   INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Room_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_Room_SchoolRef
	ON dbo.Room( SchoolRef )
GO