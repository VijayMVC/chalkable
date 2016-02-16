CREATE TABLE [dbo].[Staff] (
    [Id]        INT            NOT NULL,
    [FirstName] NVARCHAR (255) NOT NULL,
    [LastName]  NVARCHAR (255) NOT NULL,
    [BirthDate] DATETIME2 (7)  NULL,
    [Gender]    NVARCHAR (255) NULL,
    [UserId]    INT            NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Staff_Person] FOREIGN KEY ([Id]) REFERENCES [dbo].[Person] ([Id])
);

