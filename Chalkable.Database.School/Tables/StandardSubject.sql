CREATE TABLE [dbo].[StandardSubject] (
    [Id]           INT            NOT NULL,
    [Name]         NVARCHAR (100) NOT NULL,
    [Description]  NVARCHAR (200) NOT NULL,
    [AdoptionYear] INT            NULL,
    [IsActive]     BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

