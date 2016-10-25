CREATE TYPE [dbo].[TCommonCoreStandard] AS TABLE (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [ParentStandardRef]   UNIQUEIDENTIFIER NULL,
    [StandardCategoryRef] UNIQUEIDENTIFIER NOT NULL,
    [Code]                NVARCHAR (255)   NULL,
    [DEscription]         NVARCHAR (MAX)   NULL);

