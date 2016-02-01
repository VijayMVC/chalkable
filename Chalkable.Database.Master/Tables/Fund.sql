CREATE TABLE [dbo].[Fund] (
    [Id]                  UNIQUEIDENTIFIER NOT NULL,
    [PerformedDateTime]   DATETIME2 (7)    NOT NULL,
    [Amount]              MONEY            NOT NULL,
    [Description]         NVARCHAR (MAX)   NULL,
    [FromSchoolRef]       UNIQUEIDENTIFIER NULL,
    [ToSchoolRef]         UNIQUEIDENTIFIER NULL,
    [FromUserRef]         UNIQUEIDENTIFIER NULL,
    [ToUserRef]           UNIQUEIDENTIFIER NULL,
    [AppInstallActionRef] UNIQUEIDENTIFIER NULL,
    [IsPrivate]           BIT              NOT NULL,
    [FundRequestRef]      UNIQUEIDENTIFIER NULL,
    [SchoolRef]           UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Fund_FromSchool] FOREIGN KEY ([FromSchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [FK_Fund_FromUser] FOREIGN KEY ([FromUserRef]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Fund_FundRequest] FOREIGN KEY ([FundRequestRef]) REFERENCES [dbo].[FundRequest] ([Id]),
    CONSTRAINT [FK_Fund_School] FOREIGN KEY ([SchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [FK_Fund_ToSchool] FOREIGN KEY ([ToSchoolRef]) REFERENCES [dbo].[School] ([Id]),
    CONSTRAINT [FK_Fund_ToUser] FOREIGN KEY ([ToUserRef]) REFERENCES [dbo].[User] ([Id])
);

