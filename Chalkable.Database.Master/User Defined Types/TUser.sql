CREATE TYPE [dbo].[TUser] AS TABLE (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [Login]                 NVARCHAR (256)   NOT NULL,
    [Password]              NVARCHAR (256)   NOT NULL,
    [FullName]              NVARCHAR (1024)  NULL,
    [IsSysAdmin]            BIT              NOT NULL,
    [IsDeveloper]           BIT              NOT NULL,
    [IsAppTester]           BIT              NOT NULL,
    [IsDistrictRegistrator] BIT              NOT NULL,
    [ConfirmationKey]       NVARCHAR (256)   NULL,
    [SisUserName]           NVARCHAR (256)   NULL,
    [SisUserId]             INT              NULL,
    [DistrictRef]           UNIQUEIDENTIFIER NULL);

