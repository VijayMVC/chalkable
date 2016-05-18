CREATE TABLE [dbo].[Developer] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NULL,
    [WebSite]     NVARCHAR (255)   NULL,
    [DistrictRef] UNIQUEIDENTIFIER NOT NULL,
    [PayPalLogin] NVARCHAR (256)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Developer_User] FOREIGN KEY ([Id]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [UQ_Developer_DistrctRef] UNIQUE NONCLUSTERED ([DistrictRef] ASC)
);

