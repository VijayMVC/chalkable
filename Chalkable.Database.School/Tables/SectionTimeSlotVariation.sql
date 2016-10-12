CREATE TABLE [dbo].[SectionTimeSlotVariation] (
    [ClassRef]                      INT NOT NULL,
    [ScheduledTimeSlotVariationRef] INT NOT NULL,
    CONSTRAINT [PK_SectionTimeSlotVariation] PRIMARY KEY CLUSTERED ([ClassRef] ASC, [ScheduledTimeSlotVariationRef] ASC),
    CONSTRAINT [FK_SectionTimeSlotVariation_Class] FOREIGN KEY ([ClassRef]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_SectionTimeSlotVariation_ScheduledTimeSlotVariation] FOREIGN KEY ([ScheduledTimeSlotVariationRef]) REFERENCES [dbo].[ScheduledTimeSlotVariation] ([Id])
);

GO	
CREATE NONCLUSTERED INDEX IX_SectionTimeSlotVariation_ScheduledTimeSlotVariationRef
	ON dbo.SectionTimeSlotVariation( ScheduledTimeSlotVariationRef )
GO