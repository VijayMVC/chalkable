﻿CREATE TABLE [dbo].[User] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [Login]                 NVARCHAR (256)   NOT NULL,
    [Password]              NVARCHAR (256)   NOT NULL,
    [IsSysAdmin]            BIT              NOT NULL,
    [IsDeveloper]           BIT              NOT NULL,
    [ConfirmationKey]       NVARCHAR (256)   NULL,
    [SisUserName]           NVARCHAR (256)   NULL,
    [DistrictRef]           UNIQUEIDENTIFIER NULL,
    [SisUserId]             INT              NULL,
    [FullName]              NVARCHAR (1024)  NULL,
    [IsAppTester]           BIT              NOT NULL,
    [IsDistrictRegistrator] BIT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_District] FOREIGN KEY ([DistrictRef]) REFERENCES [dbo].[District] ([Id]),
    CONSTRAINT [UQ_Login] UNIQUE NONCLUSTERED ([Login] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_USER_LOGIN_PASSWORD]
    ON [dbo].[User]([Login] ASC, [Password] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_User_SisUserId_District]
    ON [dbo].[User]([SisUserId] ASC, [DistrictRef] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_User_DistrictRef_SisUserId]
    ON [dbo].[User]([SisUserId] ASC, [DistrictRef] ASC) WHERE ([SisUserId] IS NOT NULL);

