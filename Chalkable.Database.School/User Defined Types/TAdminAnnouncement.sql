CREATE TYPE [dbo].[TAdminAnnouncement] AS TABLE (
    [Id]           INT            NOT NULL,
    [Created]      DATETIME2 (7)  NOT NULL,
    [State]        INT            NOT NULL,
    [Content]      NVARCHAR (MAX) NULL,
    [Title]        NVARCHAR (30) NULL,
    [Expires]      DATETIME2 (7)  NOT NULL,
    [AdminRef]     INT            NOT NULL,
    [AdminName]    NVARCHAR (MAX) NULL,
    [AdminGender] NVARCHAR (MAX) NULL,
    [IsOwner]      BIT            NULL,
    [Complete]     BIT            NULL,
    [AllCount]     INT            NULL);

