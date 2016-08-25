CREATE TABLE [dbo].[Subject] (
    [Code]        NVARCHAR (10)  NOT NULL,
    [Description] NVARCHAR (256) NULL,
    [Broad]       NVARCHAR (10)  NULL,
    CONSTRAINT [PK_Subject] PRIMARY KEY CLUSTERED ([Code] ASC)
);

