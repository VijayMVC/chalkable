CREATE TYPE [dbo].[TRoom] AS TABLE (
    [Id]          INT             NOT NULL,
    [RoomNumber]  NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [Size]        NVARCHAR (255)  NULL,
    [Capacity]    INT             NULL,
    [PhoneNumber] NVARCHAR (255)  NULL,
    [SchoolRef]   INT             NOT NULL);

