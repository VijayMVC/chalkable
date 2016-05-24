CREATE TABLE [dbo].[LPGalleryCategory] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (255) NOT NULL,
    [OwnerRef] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [PK_LPGalleryCategory_Person] FOREIGN KEY ([OwnerRef]) REFERENCES [dbo].[Person] ([Id]),
    CONSTRAINT [UQ_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

GO	
CREATE NONCLUSTERED INDEX IX_LPGalleryCategory_OwnerRef
	ON dbo.LPGalleryCategory( OwnerRef )
GO