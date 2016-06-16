CREATE TABLE [dbo].[ApplicationGradeLevel] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [ApplicationRef] UNIQUEIDENTIFIER NOT NULL,
    [GradeLevel]     INT              NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationGradeLevel_Application] FOREIGN KEY ([ApplicationRef]) REFERENCES [dbo].[Application] ([Id]),
    CONSTRAINT [UQ_ApplicationGradeLevel_ApplicationRef_GradeLevel] UNIQUE NONCLUSTERED ([ApplicationRef] ASC, [GradeLevel] ASC)
);

