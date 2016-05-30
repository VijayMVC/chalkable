CREATE TABLE [dbo].[UserLoginInfo] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [SisToken]          NVARCHAR (MAX)   NULL,
    [SisTokenExpires]   DATETIME2 (7)    NULL,
    [LastPasswordReset] DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserLoginInfo_User] FOREIGN KEY ([Id]) REFERENCES [dbo].[User] ([Id])
);

