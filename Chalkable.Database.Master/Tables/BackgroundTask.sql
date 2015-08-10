CREATE TABLE [dbo].[BackgroundTask] (
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    [DistrictRef] UNIQUEIDENTIFIER NULL,
    [Type]        INT              NOT NULL,
    [State]       INT              NOT NULL,
    [Created]     DATETIME2 (7)    NOT NULL,
    [Scheduled]   DATETIME2 (7)    NOT NULL,
    [Started]     DATETIME2 (7)    NULL,
    [Data]        NVARCHAR (MAX)   NULL,
    [Completed]   DATETIME2 (7)    NULL,
    [Domain]      NVARCHAR (256)   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BackgroundTask_District] FOREIGN KEY ([DistrictRef]) REFERENCES [dbo].[District] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_BackgroundTas_Domain]
    ON [dbo].[BackgroundTask]([Domain] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BackgroundTask_District]
    ON [dbo].[BackgroundTask]([DistrictRef] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BackgroundTask_Scheduled]
    ON [dbo].[BackgroundTask]([Scheduled] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BackgroundTask_State]
    ON [dbo].[BackgroundTask]([State] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_BackgroundTask_State_Scheduled]
    ON [dbo].[BackgroundTask]([State] ASC, [Scheduled] ASC)
    INCLUDE([Id], [Domain]);


GO
CREATE NONCLUSTERED INDEX [nci_wi_BackgroundTask_9BE38EE6-2E55-45B4-ADC4-8A83BD0583A5]
    ON [dbo].[BackgroundTask]([DistrictRef] ASC, [Completed] ASC)
    INCLUDE([Id]);

