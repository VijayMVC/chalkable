CREATE TYPE [dbo].[TPreference] AS TABLE (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Key]      NVARCHAR (256)   NOT NULL,
    [Value]    NVARCHAR (2046)  NULL,
    [IsPublic] BIT              NOT NULL,
    [Category] INT              NOT NULL,
    [Type]     INT              NOT NULL,
    [Hint]     NVARCHAR (MAX)   NULL);

