CREATE TYPE [dbo].[TSchool] AS TABLE (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [DistrictRef]            UNIQUEIDENTIFIER NOT NULL,
    [Name]                   NVARCHAR (256)   NOT NULL,
    [LocalId]                INT              NOT NULL,
    [IsChalkableEnabled]     BIT              NOT NULL,
    [IsLEEnabled]            BIT              NOT NULL,
    [IsLESyncComplete]       BIT              NOT NULL,
    [StudyCenterEnabledTill] DATETIME2 (7)    NULL);

