﻿CREATE TABLE [dbo].[ClassAnnouncement] (
    [Id]                       INT            NOT NULL,
    [Expires]                  DATETIME2 (7)  NOT NULL,
    [ClassRef]                 INT            NOT NULL,
    [ClassAnnouncementTypeRef] INT            NULL,
    [MayBeDropped]             BIT            NOT NULL,
    [VisibleForStudent]        BIT            NOT NULL,
    [Order]                    INT            NOT NULL,
    [Dropped]                  BIT            NOT NULL,
    [MaxScore]                 DECIMAL (18)   NULL,
    [WeightAddition]           DECIMAL (9, 6) NULL,
    [WeightMultiplier]         DECIMAL (9, 6) NULL,
    [SisActivityId]            INT            NULL,
    [SchoolYearRef]            INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassAnnouncement_Announcement] FOREIGN KEY ([Id]) REFERENCES [dbo].[Announcement] ([Id]),
    CONSTRAINT [FK_ClassAnnouncement_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_ClassAnnouncement_SchoolYear] FOREIGN KEY ([SchoolYearRef]) REFERENCES [dbo].[SchoolYear] ([Id])
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UQ_ClassAnnouncement_SisActivityId]
    ON [dbo].[ClassAnnouncement]([SisActivityId] ASC) WHERE ([SisActivityId] IS NOT NULL);

