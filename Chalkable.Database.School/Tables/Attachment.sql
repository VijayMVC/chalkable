CREATE TABLE [dbo].[Attachment] (
    [Id]                  INT            IDENTITY (1000000000, 1) NOT NULL,
    [PersonRef]           INT            NOT NULL,
    [Name]                NVARCHAR (256) NOT NULL,
    [MimeType]            NVARCHAR (256) NULL,
    [UploadedDate]        DATETIME2 (7)  NOT NULL,
    [LastAttachedDate]    DATETIME2 (7)  NOT NULL,
    [Uuid]                NVARCHAR (MAX) NULL,
    [SisAttachmentId]     INT            NULL,
    [RelativeBlobAddress] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Attachment_Person] FOREIGN KEY ([PersonRef]) REFERENCES [dbo].[Person] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Attachment_PersonRef]
    ON [dbo].[Attachment]([PersonRef] ASC);

