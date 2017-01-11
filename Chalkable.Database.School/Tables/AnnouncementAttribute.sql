CREATE TABLE [dbo].[AnnouncementAttribute] (
    [Id]          INT            NOT NULL,
    [Code]        NVARCHAR (5)   NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NOT NULL,
    [StateCode]   NVARCHAR (10)  NOT NULL,
    [SIFCode]     NVARCHAR (10)  NOT NULL,
    [NCESCode]    NVARCHAR (10)  NOT NULL,
    [IsActive]    BIT            NOT NULL,
    [IsSystem]    BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [QA_AnnouncementAttribute_Code] UNIQUE NONCLUSTERED ([Code] ASC),
    CONSTRAINT [QA_AnnouncementAttribute_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

