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



GO


