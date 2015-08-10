CREATE TABLE [dbo].[ABToCCMapping] (
    [CCStandardRef]       UNIQUEIDENTIFIER NOT NULL,
    [AcademicBenchmarkId] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ABToCCMappingID] PRIMARY KEY CLUSTERED ([AcademicBenchmarkId] ASC, [CCStandardRef] ASC),
    CONSTRAINT [FK_ABToCCMapping_CommonCoreStandard] FOREIGN KEY ([CCStandardRef]) REFERENCES [dbo].[CommonCoreStandard] ([Id])
);

