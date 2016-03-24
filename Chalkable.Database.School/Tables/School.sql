CREATE TABLE [dbo].[School] (
    [Id]                 INT             NOT NULL,
    [Name]               NVARCHAR (1024) NULL,
    [IsActive]           BIT             NOT NULL,
    [IsPrivate]          BIT             NOT NULL,
    [IsChalkableEnabled] BIT             NOT NULL,
    [IsLEEnabled]        BIT             NOT NULL,
    [IsLESyncComplete]   BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

