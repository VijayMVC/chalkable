CREATE TABLE [dbo].[ABToCCMapping] (
    [CCStandardRef]       UNIQUEIDENTIFIER NOT NULL,
    [AcademicBenchmarkId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AbToCCMappingID] PRIMARY KEY CLUSTERED ([CCStandardRef] ASC, [AcademicBenchmarkId] ASC),
    CONSTRAINT [FK_ABToCCMapping_CommonCoreStandard] FOREIGN KEY ([CCStandardRef]) REFERENCES [dbo].[CommonCoreStandard] ([Id])
);



