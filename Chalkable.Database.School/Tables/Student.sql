CREATE TABLE [dbo].[Student] (
    [Id]                  INT            NOT NULL,
    [FirstName]           NVARCHAR (255) NOT NULL,
    [LastName]            NVARCHAR (255) NOT NULL,
    [BirthDate]           DATETIME2 (7)  NULL,
    [Gender]              NVARCHAR (255) NULL,
    [HasMedicalAlert]     BIT            NOT NULL,
    [IsAllowedInetAccess] BIT            NOT NULL,
    [SpecialInstructions] NVARCHAR (MAX) NOT NULL,
    [SpEdStatus]          NVARCHAR (256) NULL,
    [PhotoModifiedDate]   DATETIME2 (7)  NULL,
    [UserId]              INT            NOT NULL,
	[IsHispanic]		  BIT			 NOT NULL DEFAULT 0,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Student_Person] FOREIGN KEY ([Id]) REFERENCES [dbo].[Person] ([Id])
);

